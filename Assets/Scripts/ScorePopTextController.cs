using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePopTextController : MonoBehaviour {

	private enum Stat { Wait = 0, Combo, Shuffle, Score };
	private Stat m_stat = Stat.Wait;

	private int m_step;

	private int m_score = 0;
	private int m_combo = 0;	

	private string m_redScoreFormat = "<size=28><color=#ff0000>{0}</color></size>";
	private string m_orangeScoreFormat = "<size=24><color=#ff6347>{0}</color></size>";
	private string m_yellowScoreFormat = "<size=20><color=#ffff00>{0}</color></size>";
	private string m_whiteScoreFormat = "<size=18><color=#ffffff>{0}</color></size>";

	[SerializeField]
	private float m_comboPopTime = 0.4f;

	[SerializeField]
	private float m_shufflePopTime = 0.4f;

	[SerializeField]
	private float m_scorePopTime = 0.6f;

	private float m_popSeconds = 0f;

	// Use this for initialization
	void Start () {
		GetComponent<TextMesh> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		GetComponent<TextMesh> ().text = "";
	}
	
	// Update is called once per frame
	void Update () {
		if(m_stat == Stat.Combo) {
			StatCombo();
		}
		else if(m_stat == Stat.Shuffle) {
			StatShuffle();
		}
		else if(m_stat == Stat.Score) {
			StatScore();
		}
	}

	void ChangeStat(Stat stat) {
		m_stat = stat;
		m_step = 0;
	}

	void StatCombo() {
		if(m_step == 0)
		{
			GetComponent<TextMesh> ().text = GetComboString(m_combo);
			m_popSeconds = 0;
			m_step++;
		}
		else if (m_step == 1)
		{
			m_popSeconds += Time.deltaTime;
			if(m_popSeconds > m_comboPopTime)
			{
				m_popSeconds = 0.0f;
				ChangeStat(Stat.Shuffle);
			}
		}

		GrowMoving();
	}

	void StatShuffle() {
		if(m_step == 0)
		{
			m_popSeconds = 0;
			m_step++;
		}
		else if (m_step == 1)
		{
			GetComponent<TextMesh> ().text = GetGrowingString(m_combo);
			m_popSeconds += Time.deltaTime;
			if(m_popSeconds > m_shufflePopTime)
			{
				m_popSeconds = 0.0f;
				ChangeStat(Stat.Score);
			}
		}

		GrowMoving();
	}

	void StatScore() {
		if(m_step == 0)
		{
			GetComponent<TextMesh> ().text = GetScoreString(m_combo, m_score);
			m_popSeconds = 0;
			m_step++;
		}
		else if(m_step == 1)
		{
			m_popSeconds += Time.deltaTime;
			if(m_popSeconds > m_scorePopTime)
			{
				m_popSeconds = 0.0f;
				ChangeStat(Stat.Wait);
				Destroy(this.gameObject);
			}
		}

		GrowMoving();
	}

	void GrowMoving() {
		float speed = 0.5f;
		this.transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
	}

	string GetComboString(int combo) {
		string scoreFormat = m_whiteScoreFormat;
		if(combo > 7) {
			scoreFormat = m_redScoreFormat;
		}
		else if(combo > 5) {
			scoreFormat = m_orangeScoreFormat;
		}
		else if(combo > 3) {
			scoreFormat = m_yellowScoreFormat;
		}
		return string.Format(scoreFormat + "<size=18><color=#fffff>combo</color></size>", combo);
	}

	string GetGrowingString(int combo)
	{
		int shuffleValue = UnityEngine.Random.Range(1, 9999);
		string scoreFormat = m_whiteScoreFormat;
		if(combo > 7) {
			scoreFormat = m_redScoreFormat;
		}
		else if(combo > 5) {
			scoreFormat = m_orangeScoreFormat;
		}
		else if(combo > 3) {
			scoreFormat = m_yellowScoreFormat;
		}
		return string.Format(scoreFormat, shuffleValue);
	}

	string GetScoreString(int combo, int score) {
		string scoreFormat = m_whiteScoreFormat;
		if(combo > 7) {
			scoreFormat = m_redScoreFormat;
		}
		else if(combo > 5) {
			scoreFormat = m_orangeScoreFormat;
		}
		else if(combo > 3) {
			scoreFormat = m_yellowScoreFormat;
		}
		return string.Format(scoreFormat + "<size=18><color=#fffff> score get</color></size>", score);
	}

	public void PopText(int score, int combo) {
		m_score = score;
		m_combo = combo;
		ChangeStat(Stat.Combo);
	}

}
