using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{

    [SerializeField] private float timeBetweenTileCracks = 5f;
    [SerializeField] private float percentDecreaseAfterCracks = 0.95f;
    [SerializeField] private Pengu pengu;

    [Header("Melting")]
    [SerializeField] private AnimatedTile meltTileIce;
    [SerializeField] private AnimatedTile meltTileBorder;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap iceMap;
    [SerializeField] private Tilemap borderMap;
    [SerializeField] private Tilemap waterMap;
    [SerializeField] private Tilemap collisionMap;

    [Header("Tiles")]
    [SerializeField] private TileBase iceTile;
    [SerializeField] private TileBase shallowBorderTile;

    private float meltTimer = 0;
    AudioSource ac;

    void Start()
    {
        meltTimer = timeBetweenTileCracks;
        ac = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        meltTimer -= Time.deltaTime;

        if (meltTimer < 0)
        {
            ac.Play(0);
            timeBetweenTileCracks *= percentDecreaseAfterCracks;
            meltTimer = timeBetweenTileCracks;

            StartCoroutine(MeltTile(GetRandomIceTile()));
        }
    }

    public IEnumerator MeltTile(Vector3Int tilePos)
    {
        SetTileNonIcey(tilePos);
        // Let player walk on the crumbling tile until its fully crumbled!
        collisionMap.SetTile(tilePos, null);

        iceMap.SetTile(tilePos, meltTileIce);
        borderMap.SetTile(tilePos + Vector3Int.down, meltTileBorder);
        yield return new WaitForSeconds(1);

        SetTileNonIcey(tilePos);

        pengu.KillIfOnWater(tilePos);
    }

    public Vector3Int GetRandomIceTile()
    {
        var localTilePoses = new List<Vector3Int>();

        foreach (var pos in iceMap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            if (iceMap.HasTile(localPlace))
            {
                localTilePoses.Add(localPlace);
            }
        }

        return localTilePoses[Random.Range(0, localTilePoses.Count)];
    }

    public void SetTileIcey(Vector3Int tileCoords)
    {
        iceMap.SetTile(tileCoords, iceTile);
        borderMap.SetTile(tileCoords, null);
        borderMap.SetTile(tileCoords + Vector3Int.down, shallowBorderTile);
        collisionMap.SetTile(tileCoords, null);

        iceMap.RefreshTile(tileCoords);
        borderMap.RefreshTile(tileCoords);
    }

    public void SetTileNonIcey(Vector3Int tileCoords)
    {
        iceMap.SetTile(tileCoords, null);
        borderMap.SetTile(tileCoords + Vector3Int.down, null);
        if (iceMap.GetTile(tileCoords + Vector3Int.up) != null)
        {
            borderMap.SetTile(tileCoords, shallowBorderTile);
        }
        collisionMap.SetTile(tileCoords, iceTile);

        iceMap.RefreshTile(tileCoords);
        borderMap.RefreshTile(tileCoords);
    }
}