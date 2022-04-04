using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AttractVideoPlayer : MonoBehaviour {
	[SerializeField] private VideoPlayer videoPlayer;

	// Start is called before the first frame update
	void Start() {
		videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "BackgroundVid.mp4");
		videoPlayer.Play();
	}
}
