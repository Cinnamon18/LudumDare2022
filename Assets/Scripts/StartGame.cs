using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class StartGame : MonoBehaviour {
	// Start is called before the first frame update
	public Button startButton, quitButton;
	void Start() {
		startButton.onClick.AddListener(startScene);
		quitButton.onClick.AddListener(quitGame);
	}



	void startScene() {
		SceneManager.LoadScene("AliceSampleScene");
	}

	void quitGame() {
		Application.Quit();
	}
}
