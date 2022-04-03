using System.Collections;
using System.Collections.Generic;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Pengu : MonoBehaviour {

	[SerializeField] private Animator anim;
	[SerializeField] private AudioManager audioManager;
	[SerializeField] private TileManager tileManager;
	[SerializeField] private Canvas popUp;
	[SerializeField] private TMP_Text popupText;

	[SerializeField] private Vector2 speed = new Vector2(3, 3);

	[Header("My monos")]
	[SerializeField] private Rigidbody2D rigidbody;
	[SerializeField] private GameObject headIceBlock;

	[Header("Tilemaps")]
	[SerializeField] private Tilemap iceMap;
	[SerializeField] private Tilemap borderMap;
	[SerializeField] private Tilemap waterMap;
	[SerializeField] private Tilemap collisionMap;

	private PenguDir direction = PenguDir.IDLE_DOWN;
	private bool isCaryingIce = false;
	private float timeSurvived = 0;

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
			direction = PenguDir.RIGHT;
			anim.SetTrigger("IsRight");
		}

		if (left) {
			direction = PenguDir.LEFT;
			anim.SetTrigger("IsLeft");
		}

		if (up) {
			direction = PenguDir.UP;
			anim.SetTrigger("IsUp");
		}

		if (down) {
			direction = PenguDir.DOWN;
			anim.SetTrigger("IsDown");
		}

		if (!(right || left || up || down)) {
			switch(direction) {
				case PenguDir.RIGHT:
					direction = PenguDir.IDLE_RIGHT;
					anim.SetTrigger("IsIdleRight");
					break;
				case PenguDir.LEFT:
					direction = PenguDir.IDLE_LEFT;
					anim.SetTrigger("IsIdleLeft");
					break;
				case PenguDir.UP:
					direction = PenguDir.IDLE_UP;
					anim.SetTrigger("IsIdleUp");
					break;
				case PenguDir.DOWN:
					direction = PenguDir.IDLE_DOWN;
					anim.SetTrigger("IsIdleDown");
					break;
			}
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			TryHoistOrPlaceIceBlock();
		}

		timeSurvived += Time.deltaTime;
	}

	public void KillIfOnWater() {
		if (iceMap.GetTile<Tile>(iceMap.WorldToCell(transform.position)) == null && popUp.gameObject.activeSelf == false) {
			anim.SetTrigger("Die");
			// Game over
			Debug.Log("You lose :(");
			Time.timeScale = 0;
			//SceneManager.LoadScene(0);
			popupText.text = popupText.text + " " + timeSurvived + "s";
			popUp.gameObject.SetActive(true);
		}
	}

	private void TryHoistOrPlaceIceBlock() {
		if (isCaryingIce) {
			// audioManager.PlaySound(Sound.INVALID_ACTION);
			TryPlaceIceBlock();
		} else {
			TryHoistIceBlock();
		}
	}

	private void TryHoistIceBlock() {
		var tilesInFront = GetTilesInFront();
		if (tilesInFront[0] != null) {
			isCaryingIce = true;
			headIceBlock.SetActive(true);

			tileManager.SetTileNonIcey(GetTileInFrontPos());
		} else {
			audioManager.PlaySound(Sound.INVALID_ACTION);
		}
	}

	private void TryPlaceIceBlock() {
		var tilesInFront = GetTilesInFront();
		if (tilesInFront[0] == null) {
			isCaryingIce = false;
			headIceBlock.SetActive(false);

			tileManager.SetTileIcey(GetTileInFrontPos());
		} else {
			audioManager.PlaySound(Sound.INVALID_ACTION);
		}
	}

	private Vector3Int GetTileInFrontPos() {
		Vector3Int tileInFront = iceMap.WorldToCell(transform.position);
		if (direction == PenguDir.LEFT || direction == PenguDir.IDLE_LEFT) { tileInFront += Vector3Int.left; }
		if (direction == PenguDir.RIGHT || direction == PenguDir.IDLE_RIGHT) { tileInFront += Vector3Int.right; }
		if (direction == PenguDir.UP || direction == PenguDir.IDLE_UP) { tileInFront += Vector3Int.up; }
		if (direction == PenguDir.DOWN || direction == PenguDir.IDLE_DOWN) { tileInFront += Vector3Int.down; }

		return tileInFront;
	}

	private List<TileBase> GetTilesInFront() {
		var tileInFrontPos = GetTileInFrontPos();
		return new List<TileBase>() {
			iceMap.GetTile<TileBase>(tileInFrontPos),
			borderMap.GetTile<TileBase>(tileInFrontPos),
			waterMap.GetTile<TileBase>(tileInFrontPos),
			collisionMap.GetTile<TileBase>(tileInFrontPos)
		};
	}
}
