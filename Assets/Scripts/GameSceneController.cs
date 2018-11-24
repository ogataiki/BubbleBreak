using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TouchScript.Gestures;

public class GameSceneController : MonoBehaviour {
	
	private enum Stat {
		Wait = 0,
		Countdown = 1,
		Game ,
		Score,
	};
	Stat m_stat;
	int m_step;
	
	[SerializeField]
	private Text m_ScoreText;

	[SerializeField]
	private Text m_CountDownText;

	private float m_seconds = 0f;
	private float m_nextSeconds = 0f;
		
	[SerializeField]
	private Text m_LastTimeText;	

	public float m_gameTimeSeconds = 30f;
	private float m_lastTimeSeconds = 0f;

	[SerializeField]
	private Text m_FinishText;	
	
	[SerializeField]
	private GameObject m_bubblePrefab;
	
	private Dictionary<int, GameObject> m_bubbles = new Dictionary<int, GameObject>();
	private int m_bubble_key_count = 0;
	private Queue<GameObject> m_bubbles_key_trash = new Queue<GameObject>();

	public float m_bubble_xpos_min = -2.5f;
	public float m_bubble_xpos_max = 2.5f;
	public float m_bubble_ypos_min = -3f;
	public float m_bubble_ypos_max = 2f;

	public float m_bubble_scale_min = 0.1f;
	public float m_bubble_scale_max = 5f;

	public float m_bubble_scaleadd_min = 0.01f;
	public float m_bubble_scaleadd_max = 0.05f;
	
	public float m_bubble_next_min = 0.1f;
	public float m_bubble_next_max = 0.5f;
	
	public int m_score_seed = 3;
	
	private int m_score = 0;
	
	[SerializeField]
	private GameObject m_popScoreText;

	// Use this for initialization
	void Start () {
		
		m_FinishText.gameObject.SetActive(false);
		m_LastTimeText.gameObject.SetActive(false);
		
		m_seconds = 0f;
		m_stat = Stat.Countdown;

		// debug
		PlayerPrefs.SetInt("topScore", 0);
	}
	
	// Update is called once per frame
	void Update () {
		switch(m_stat)
		{
		case Stat.Countdown:
			StatCountDown();
			break;
		
		case Stat.Game:
			StatGame();
			break;
		
		case Stat.Score:
			StatScore();
			break;
		
		default:
			break;
		}
	}
	
	void ChangeStat(Stat next)
	{
		if(m_stat != next)
		{
			m_step = 0;
		}
		m_stat = next;
	}
	
	void StatCountDown()
	{
		m_seconds += Time.deltaTime;
		if(m_seconds < 3.0f) {
			int lastSecond = 3 - (int)m_seconds;
			m_CountDownText.text = string.Format("{0}", lastSecond);
		}
		else
		{
			m_CountDownText.gameObject.SetActive(false);
			
			m_lastTimeSeconds = 0f;
			m_LastTimeText.gameObject.SetActive(true);
			
			m_bubble_key_count = 1;
			
			ChangeStat(Stat.Game);
		}
	}
	
	void StatGame()
	{
		m_lastTimeSeconds += Time.deltaTime;
		m_LastTimeText.text = string.Format("あと {0:F0} 秒", m_gameTimeSeconds - m_lastTimeSeconds);
		if(m_lastTimeSeconds > m_gameTimeSeconds)
		{
			foreach (GameObject bubble in m_bubbles.Values) {
				bubble.GetComponent<BubbleController> ().m_isGame = false;
			}
			ChangeStat(Stat.Score);
			return;
		}

		if(m_step == 0)
		{
			// 泡を出す
			GameObject bubble;
			float x = UnityEngine.Random.Range(m_bubble_xpos_min, m_bubble_xpos_max);
			float y = UnityEngine.Random.Range(m_bubble_ypos_min, m_bubble_ypos_max);
			if(m_bubbles_key_trash.Count > 0) {
				// ゴミ箱にあるならそれを再利用
				bubble = m_bubbles_key_trash.Dequeue();
				bubble.transform.localPosition = new Vector3 (x, y, 0.0f);
			}
			else {
				// ゴミ箱が空ならしょうがないので新規作成
				bubble = Instantiate (m_bubblePrefab, new Vector3 (x, y, 0.0f), Quaternion.identity) as GameObject;				
				bubble.GetComponent<BubbleController> ().m_key = m_bubble_key_count;
				m_bubble_key_count++;
			}
			bubble.transform.localScale = new Vector3(m_bubble_scale_min, m_bubble_scale_min, 1);
			
			float sadd = UnityEngine.Random.Range(m_bubble_scaleadd_min, m_bubble_scaleadd_max);
			bubble.GetComponent<BubbleController> ().m_scale_add = sadd;

			Color newColor = new Color(1, 1, 1, 0.5f - (bubble.transform.localScale.x * 0.1f));
			bubble.GetComponent<MeshRenderer> ().material.color = newColor;
			
			bubble.GetComponent<BubbleController> ().m_isGame = true;
			bubble.SetActive(true);
			m_bubbles.Add(bubble.GetComponent<BubbleController> ().m_key, bubble);
			
			m_seconds = 0;
			m_nextSeconds = UnityEngine.Random.Range(m_bubble_next_min, m_bubble_next_max);
			m_step = 1;
		}
		else if(m_step == 1)
		{
			m_seconds += Time.deltaTime;
			if(m_seconds > m_nextSeconds) {
				m_step = 0;
			}
		}
		
		List<int> keyList = new List<int>(m_bubbles.Keys);
		foreach (int key in keyList) {
			GameObject bubble = m_bubbles[key];
			if(bubble.transform.localScale.x > m_bubble_scale_max || bubble.transform.localScale.y > m_bubble_scale_max)
			{
				// 無効化してゴミ箱にポイー
				trashBubble(key);
			}
		}
	}
	
	void StatScore()
	{
		if(m_step == 0)
		{
			m_FinishText.gameObject.SetActive(true);
			PlayerPrefs.SetInt("beforScore", m_score);
			int topScore = PlayerPrefs.GetInt ("topScore", 0);
			if(m_score > topScore) {
				PlayerPrefs.SetInt("topScore", m_score);
				m_step = 1;
			}
			else {
				m_step = 3;
			}
		}
		else if(m_step == 1) {
			// 最高スコア更新演出タップ待ち
		}
		else if(m_step == 2) {
			// 最高スコア更新演出
			m_FinishText.text = "<color=#ff0000>High Score!!</color>";
			m_ScoreText.text = string.Format("Score <color=#ff0000>{0}</color>", m_score);
			m_ScoreText.fontSize = 48;
			m_step = 3;
		}
		else if(m_step == 3) {
			ChangeStat(Stat.Wait);
		}
	}
	
    void OnEnable()
    {
        // TapGestureのdelegateに登録
        GetComponent<TapGesture>().Tapped += tappedHandle;
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
        
		List<int> keyList = new List<int>(m_bubbles.Keys);
		foreach (int key in keyList) {
			GameObject bubble = m_bubbles[key];
			m_bubbles.Remove(key);
			if(bubble != null) 
			{
				Destroy(bubble);
			}
		}
		int trashCount = m_bubbles_key_trash.Count;
		for(int i=0; i<trashCount; ++i)
		{
			Destroy(m_bubbles_key_trash.Dequeue());
		}
    }

    void UnsubscribeEvent()
    {
        // 登録を解除
        GetComponent<TapGesture>().Tapped -= tappedHandle;
    }

    void tappedHandle(object sender, System.EventArgs e)
    {
        if(m_stat == Stat.Game)
        {
        	TapGesture gesture = sender as TapGesture;
        	Ray ray = Camera.main.ScreenPointToRay (gesture.ScreenPosition);
        	// Rayが衝突した全てのコライダーの情報を得る。＊順序は保証されない
    		RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
    		
    		int combo = 0;
    		foreach(RaycastHit hit in hits) {
    			
        		GameObject obj = hit.collider.gameObject;
        		if(obj.tag == "Bubble")
        		{
					// 無効化してゴミ箱にポイー
        			trashBubble(obj.GetComponent<BubbleController> ().m_key);
        			combo++;
        		}
    		}
			if(combo > 0) {
				// 今回発生したスコアをpop
				Vector3 touchPos = new Vector3(gesture.ScreenPosition.x, gesture.ScreenPosition.y, -10);
				Vector3 wpos = Camera.main.ScreenToWorldPoint(touchPos);
				Vector3 textPos = new Vector3 (wpos.x, wpos.y, 0.0f);
				GameObject scoreText = Instantiate (m_popScoreText, textPos, Quaternion.identity) as GameObject;
			    int score = addScore(m_score_seed, combo);
				scoreText.GetComponent<ScorePopTextController>().PopText(score, combo);
        		      			
				// 今回の累計スコアを更新
        		m_ScoreText.text = string.Format("Score {0}", m_score);

				// 消したら増やす
      			m_step = 0;
			}
        }
        else if(m_stat == Stat.Score)
        {
			m_step++;
        }
        else if(m_stat == Stat.Wait)
        {
        	Debug.Log("GameScene -> RootScene");        	
       		SceneManager.LoadScene("RootScene");
        }
    }

	private void trashBubble(int key)
	{
		GameObject bubble = m_bubbles[key];
		bubble.GetComponent<BubbleController> ().m_isGame = false;
		bubble.SetActive(false);
		m_bubbles.Remove(key);
		m_bubbles_key_trash.Enqueue(bubble);
	}
	
	private int addScore(int seed, int combo) {
		int calcScore = 0;
		for( int i=1; i<=combo; ++i) {
			calcScore += (seed * i);
		}
		m_score += calcScore;
		return calcScore;
	}
}
