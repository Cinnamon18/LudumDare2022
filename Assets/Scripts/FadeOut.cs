using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {

	[SerializeField] private CanvasGroup canvasToFade;
	[SerializeField] private AnimationCurve fadeCurve;

	private float startTime = 0;
	private Color fadedColor;

	void Start() {
		startTime = Time.time;
	}

	void Update() {
		canvasToFade.alpha = fadeCurve.Evaluate(Time.time - startTime);
	}
}
