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
	[SerializeField] private List<TileBase> meltTiles;

	[Header("Tilemaps")]
	[SerializeField] private Tilemap iceMap;
	[SerializeField] private Tilemap borderMap;
	[SerializeField] private Tilemap waterMap;
	[SerializeField] private Tilemap collisionMap;

	[Header("Tiles")]
	[SerializeField] private TileBase iceTile;
	[SerializeField] private TileBase shallowBorderTile;

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
		// iceMap.SetTile(tilePos, null);
		// borderMap.SetTile(tilePos, shallowBorderTile);
		foreach (var meltTile in meltTiles) {
			waterMap.SetTile(tilePos, meltTile);
			yield return new WaitForSeconds(timeForTileToCrackPerFrame);
		}
		SetTileNonIcey(tilePos);

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
