using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pengu : MonoBehaviour {

	[SerializeField] private Rigidbody2D rigidbody;

	[SerializeField] private Vector2 speed = new Vector2(3, 3);

	void Start() {

	}

	// Update is called once per frame
	void Update() {
		var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		this.rigidbody.velocity = speed * input;
	}

	void FixedUpdate() {

	}
}
