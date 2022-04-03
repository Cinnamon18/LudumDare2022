using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pengu : MonoBehaviour {

	[SerializeField] private Animator anim;
	[SerializeField] private AudioManager audioManager;
	[SerializeField] private TileManager tileManager;

	[SerializeField] private Vector2 speed = new Vector2(3, 3);

	[Header("My monos")]
	[SerializeField] private Rigidbody2D rigidbody;
	[SerializeField] private GameObject headIceBlock;

	[Header("Tilemaps")]
	[SerializeField] private Tilemap iceMap;
	[SerializeField] private Tilemap waterMap;
	[SerializeField] private Tilemap collisionMap;

	private PenguDir direction = PenguDir.IDLE;
	private bool isCaryingIce = false;

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

		if (input == Vector2.zero && direction != PenguDir.IDLE) {
			direction = PenguDir.IDLE;

			anim.SetTrigger("IsIdle");
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			TryHoistOrPlaceIceBlock();
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
		Debug.Log(tilesInFront[0]?.sprite);
		if ((tilesInFront[0]?.sprite.name.Contains("ice") ?? false)
			|| (tilesInFront[0]?.sprite.name.Contains("border") ?? false)) {
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
		if (direction == PenguDir.LEFT) { tileInFront += Vector3Int.left; }
		if (direction == PenguDir.RIGHT) { tileInFront += Vector3Int.right; }
		if (direction == PenguDir.UP) { tileInFront += Vector3Int.up; }
		if (direction == PenguDir.DOWN || direction == PenguDir.IDLE) { tileInFront += Vector3Int.down; }

		return tileInFront;
	}

	private List<Tile> GetTilesInFront() {
		var tileInFrontPos = GetTileInFrontPos();
		return new List<Tile>() {
			iceMap.GetTile<Tile>(tileInFrontPos),
			waterMap.GetTile<Tile>(tileInFrontPos),
			collisionMap.GetTile<Tile>(tileInFrontPos)
		};
	}
}
