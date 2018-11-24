using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TouchScript.Gestures;

public class RootSceneController : MonoBehaviour {

    [SerializeField]
    private Text m_topScoreValueText;

    [SerializeField]
    private Text m_beforScoreValueText;

	// Use this for initialization
	void Start () {
		int topScore = PlayerPrefs.GetInt ("topScore", 0);
        m_topScoreValueText.text = string.Format("{0}", topScore);

		int beforScore = PlayerPrefs.GetInt ("beforScore", 0);
        m_beforScoreValueText.text = string.Format("{0}", beforScore);
	}
	
	// Update is called once per frame
	void Update () {
		
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
    }

    void UnsubscribeEvent()
    {
        // 登録を解除
        GetComponent<TapGesture>().Tapped -= tappedHandle;
    }

    void tappedHandle(object sender, System.EventArgs e)
    {
        Debug.Log("RootScene -> GameScene");

        // RootSceneへ
        SceneManager.LoadScene("GameScene");
    }

}
