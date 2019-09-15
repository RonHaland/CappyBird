using UnityEngine;
using UnityEngine.UI;

public class HighscoreText : MonoBehaviour {

	Text score;

	void OnEnable() {
		score = GetComponent<Text>();
		score.text = "High Score: " +PlayerPrefs.GetInt("HighScore").ToString();
	}
}
