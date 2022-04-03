using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pengu : MonoBehaviour {

	[SerializeField] private Rigidbody2D rigidbody;
	[SerializeField] private Animator anim;

	[SerializeField] private Vector2 speed = new Vector2(3, 3);

	private PenguDirection direction = PenguDirection.IDLE;

	void Start() {

	}

	// Update is called once per frame
	void Update() {
		var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		this.rigidbody.velocity = speed * input;

		var right = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
		var left = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
		var up = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
		var down = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);

		if (right) {
			direction = PenguDirection.RIGHT;
			anim.SetTrigger("IsRight");
		}

		if (left) {
			direction = PenguDirection.LEFT;
			anim.SetTrigger("IsLeft");
		}

		if (up) {
			direction = PenguDirection.UP;
			anim.SetTrigger("IsUp");
		}

		if (down) {
			direction = PenguDirection.DOWN;
			anim.SetTrigger("IsDown");
		}

		if (
			(input == Vector2.zero)
				&& direction != PenguDirection.IDLE
		) {
			Debug.Log("wuwuw");
			direction = PenguDirection.IDLE;

			anim.SetTrigger("IsIdle");
		}

	}
}
