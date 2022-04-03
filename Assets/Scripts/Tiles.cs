using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tiles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;
        Tile[,] allTileMap = new Tile[bounds.size.x, bounds.size.y];

        //  TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                allTileMap[x, y] = tilemap.GetTile<Tile>(new Vector3Int(x, y, 0));
                // TileBase tile = allTiles[x + y * bounds.size.x];
                // if (tile != null)
                // {
                //     Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                // }
                // else
                // {
                //     Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                // }
            }
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
}
