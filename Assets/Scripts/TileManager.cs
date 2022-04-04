using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour {

	[SerializeField] private float timeBetweenTileCracks = 5f;
	[SerializeField] private float percentDecreaseAfterCracks = 0.95f;
	[SerializeField] private Pengu pengu;
	[SerializeField] private AudioClip iceBreakSound;

	[Header("Melting")]
	[SerializeField] private float secondsToMelt = 1f;
	[SerializeField] private List<Tile> meltIceTiles;
	[SerializeField] private List<Tile> meltBorderTiles;

	[Header("Tilemaps")]
	[SerializeField] private Tilemap iceMap;
	[SerializeField] private Tilemap borderMap;
	[SerializeField] private Tilemap waterMap;
	[SerializeField] private Tilemap collisionMap;

	[Header("Tiles")]
	[SerializeField] private TileBase iceTile;
	[SerializeField] private TileBase shallowBorderTile;
	[SerializeField] private TileBase shallowRegularTile;
	[SerializeField] private TileBase shallowRippleTile;
	[SerializeField] private TileBase deepRegularTile;
	[SerializeField] private TileBase deepRippleTile;

	private float meltTimer = 0;
	private List<AudioSource> audioSources = new List<AudioSource>();

	void Start() {
		meltTimer = timeBetweenTileCracks;

		for (int i = 0; i < 10; i++) {
			StartCoroutine(MakeRipples());
		}
	}

	void Update() {
		meltTimer -= Time.deltaTime;

		if (meltTimer < 0 && !pengu.isDead) {
			timeBetweenTileCracks *= percentDecreaseAfterCracks;
			meltTimer = timeBetweenTileCracks;

			StartCoroutine(MeltTile(GetRandomTile(iceMap)));
		}
	}

	private void playIceBreakSoundEffect() {
		AudioSource freeAudioSource = audioSources.Find(anAudioSource => anAudioSource.isPlaying == false);
		if (freeAudioSource == null) {
			freeAudioSource = gameObject.AddComponent<AudioSource>();
			freeAudioSource.pitch = Random.Range(0.8f, 1.05f);
			freeAudioSource.volume = Random.Range(0.15f, 0.25f);
			freeAudioSource.clip = iceBreakSound;
			audioSources.Add(freeAudioSource);
		}
		freeAudioSource.Play(0);
	}

	public IEnumerator MakeRipples() {
		while (true) {
			var tileToRipple = GetRandomTile(waterMap);
			if (waterMap.GetTile<RuleTile>(tileToRipple)?.name == "WaterRuleTile") {
				Debug.Log("Rippling water tile!");
				waterMap.SetTile(tileToRipple, shallowRippleTile);
				waterMap.RefreshTile(tileToRipple);
				yield return new WaitForSeconds(3f);
				waterMap.SetTile(tileToRipple, shallowRegularTile);
				waterMap.RefreshTile(tileToRipple);
			} else if (waterMap.GetTile<RuleTile>(tileToRipple)?.name == "DeepWaterRuleTile") {
				Debug.Log("rippling deep tile!");
				waterMap.SetTile(tileToRipple, deepRippleTile);
				waterMap.RefreshTile(tileToRipple);
				yield return new WaitForSeconds(3f);
				waterMap.SetTile(tileToRipple, deepRegularTile);
				waterMap.RefreshTile(tileToRipple);
			} else {
				Debug.Log($"unrecognized tile name {waterMap.GetTile<RuleTile>(tileToRipple)?.name}");
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	public IEnumerator MeltTile(Vector3Int tilePos) {
		SetTileNonIcey(tilePos);
		// Let player walk on the crumbling tile until its fully crumbled!
		collisionMap.SetTile(tilePos, null);

		playIceBreakSoundEffect();

		for (int i = 0; i < meltIceTiles.Count; i++) {
			iceMap.SetTile(tilePos, meltIceTiles[i]);
			borderMap.SetTile(tilePos + Vector3Int.down, meltBorderTiles[i]);
			yield return new WaitForSeconds(secondsToMelt / meltIceTiles.Count);
		}

		SetTileNonIcey(tilePos);

		pengu.KillIfOnWater(tilePos);
	}

	public Vector3Int GetRandomTile(Tilemap tileMap) {
		var localTilePoses = new List<Vector3Int>();

		foreach (var pos in tileMap.cellBounds.allPositionsWithin) {
			Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
			if (tileMap.HasTile(localPlace) && !meltIceTiles.Contains(tileMap.GetTile<Tile>(localPlace))) {
				localTilePoses.Add(localPlace);
			}
		}

		if (localTilePoses.Count == 0) {
			return Vector3Int.zero;
		}

		return localTilePoses[Random.Range(0, localTilePoses.Count)];
	}

	public void SetTileIcey(Vector3Int tileCoords) {
		iceMap.SetTile(tileCoords, iceTile);
		borderMap.SetTile(tileCoords, null);
		borderMap.SetTile(tileCoords + Vector3Int.down, shallowBorderTile);
		collisionMap.SetTile(tileCoords, null);

		iceMap.RefreshTile(tileCoords);
		borderMap.RefreshTile(tileCoords);
	}

	public void SetTileNonIcey(Vector3Int tileCoords) {
		iceMap.SetTile(tileCoords, null);
		borderMap.SetTile(tileCoords + Vector3Int.down, null);
		if (iceMap.GetTile(tileCoords + Vector3Int.up) != null) {
			borderMap.SetTile(tileCoords, shallowBorderTile);
		}
		collisionMap.SetTile(tileCoords, iceTile);

		iceMap.RefreshTile(tileCoords);
		borderMap.RefreshTile(tileCoords);
	}
}
