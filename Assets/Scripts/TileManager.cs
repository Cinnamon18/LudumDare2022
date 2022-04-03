using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour {

	[SerializeField] private float timeBetweenTileCracks = 5f;
	[SerializeField] private float percentDecreaseAfterCracks = 0.95f;

	[SerializeField] private Pengu pengu;

	[Header("Melting")]
	[SerializeField] private float timeForTileToCrackPerFrame = 0.2f;
	[SerializeField] private List<Tile> meltTiles;

	[Header("Tilemaps")]
	[SerializeField] private Tilemap iceMap;
	[SerializeField] private Tilemap waterMap;
	[SerializeField] private Tilemap collisionMap;
	[SerializeField] private Tile iceTile;

	static System.Random rnd = new System.Random();

	private float meltTimer = 0;

	void Start() {
		meltTimer = timeBetweenTileCracks;
	}

	void Update() {
		meltTimer -= Time.deltaTime;

		if (meltTimer < 0) {
			timeBetweenTileCracks *= percentDecreaseAfterCracks;
			meltTimer = timeBetweenTileCracks;

			StartCoroutine(MeltTile(GetRandomIceTile()));
		}
	}

	public IEnumerator MeltTile(Vector3Int tilePos) {
		SetTileNonIcey(tilePos);
		foreach (var meltTile in meltTiles) {
			waterMap.SetTile(tilePos, meltTile);
			yield return new WaitForSeconds(timeForTileToCrackPerFrame);
		}

		pengu.KillIfOnWater();
	}

	public Vector3Int GetRandomIceTile() {
		var localTilePoses = new List<Vector3Int>();

		foreach (var pos in iceMap.cellBounds.allPositionsWithin) {
			Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
			if (iceMap.HasTile(localPlace)) {
				localTilePoses.Add(localPlace);
			}
		}

		return localTilePoses[rnd.Next(localTilePoses.Count)];
	}

	public void SetTileIcey(Vector3Int tileCoords) {
		iceMap.SetTile(tileCoords, iceTile);
		collisionMap.SetTile(tileCoords, null);
	}

	public void SetTileNonIcey(Vector3Int tileCoords) {
		iceMap.SetTile(tileCoords, null);
		collisionMap.SetTile(tileCoords, iceTile);
	}
}
