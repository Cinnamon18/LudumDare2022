using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour {

	private float timeBetweenTileCracks = 5f;
	private float timeDecreaseAfterTileCrack = 0.2f;

	[Header("Tilemaps")]
	[SerializeField] private Tilemap iceMap;
	[SerializeField] private Tilemap waterMap;
	[SerializeField] private Tilemap collisionMap;
	[SerializeField] private Tile iceTile;

	public void SetTileIcey(Vector3Int tileCoords) {
		iceMap.SetTile(tileCoords, iceTile);
		collisionMap.SetTile(tileCoords, null);
	}

	public void SetTileNonIcey(Vector3Int tileCoords) {
		iceMap.SetTile(tileCoords, null);
		collisionMap.SetTile(tileCoords, iceTile);
	}
}
