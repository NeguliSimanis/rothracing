using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    private int halfScreenHeight = 6;
    private int halfScreenWidth = 11;
    Vector3Int lastGeneratedTileCoordinate;

    private Transform playerTransform;
    [HideInInspector]
    public int pathLength = 10;
    [HideInInspector]
    public int tilesNotCollected;
    [SerializeField]
    private GridLayout gridLayout;

    [Header("PATH")]
    [SerializeField]
    private Tilemap pathTilemap;
    [SerializeField]
    private Tile pathTile;
    private List<Tile> pathTiles = new List<Tile>();
    private Vector3Int lastCollectedTileCoordinates;

    #region FINISH LINE
    [Header("FINISH LINE")]
    [SerializeField]
    private Tilemap finishTilemap;
    [SerializeField]
    private Tile finishTile;

    private Vector3Int finishLineCellCoordinates;
    private Vector3 finishLineWorldCoordinates;
    #endregion

    public void GenerateLevel()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        pathTiles.Clear();
        GeneratePath();
        tilesNotCollected = pathLength; 
    }

    private void PlacePlayerAtStartOfPath(Vector3Int playerCellCoordinates)
    {
        playerTransform.position = gridLayout.CellToWorld(playerCellCoordinates);
    }

    private void GeneratePath()
    {
        
        for (int i = 0; i < halfScreenHeight*2-2; i++)
        {
            Tile newTile = pathTile;
            Vector3Int newTileCoordinates = new Vector3Int(GetNextXCoordinate(), -halfScreenHeight+1+i, 0);
            lastGeneratedTileCoordinate = newTileCoordinates;

            if (i == 0)
                PlacePlayerAtStartOfPath(newTileCoordinates);


            pathTilemap.SetTile(newTileCoordinates, newTile);
            pathTiles.Add(newTile);
        }
        SpawnFinishTile();
    }

    //private int GetNextYCoordinate()
    //{
    //    if (lastGeneratedTileCoordinate == null)
    //    {
    //        return Random.Range(-halfScreenWidth + 1, halfScreenWidth - 1);
    //    }
    //    else
    //    {
    //        return lastGeneratedTileCoordinate.x;
    //    }
    //    return 0;
    //}

    private int GetNextXCoordinate()
    {
        if(lastGeneratedTileCoordinate == null)
        {
            return Random.Range(-halfScreenWidth+1, halfScreenWidth-1);
        }
        else
        {
            bool nextXFound = false;
            while (!nextXFound)
            {
                int nextXID = Random.Range(0, 3);
                switch(nextXID)
                {
                    // decrease x
                    case 0:
                        if (nextXID > -halfScreenWidth + 2)
                            return lastGeneratedTileCoordinate.x - 1;
                        break;
                    // don't change x
                    case 1:
                        return lastGeneratedTileCoordinate.x;
                    // increase x
                    case 2:
                        if (nextXID < halfScreenWidth - 2)
                            return lastGeneratedTileCoordinate.x + 1;
                        break;
                }
            }
            return lastGeneratedTileCoordinate.x;
        }
    }

    private void SpawnFinishTile()
    {
        finishLineCellCoordinates = new Vector3Int(GetNextXCoordinate(), halfScreenHeight - 1, 0);
        finishLineWorldCoordinates = gridLayout.CellToWorld(finishLineCellCoordinates);
        GameManager.instance.finishLineCoordinates = finishLineWorldCoordinates;
        finishTilemap.SetTile(finishLineCellCoordinates, finishTile);
    }

    public void CollectPathTile()
    {
        Vector3Int tileGridLocation = gridLayout.WorldToCell(playerTransform.position);
        if (lastCollectedTileCoordinates == null || lastCollectedTileCoordinates != tileGridLocation)
        {
            Debug.Log("player loc " + playerTransform);
            Debug.Log("tile grid location " + tileGridLocation);

            if (pathTilemap.GetTile(tileGridLocation) != null)
            {
                tilesNotCollected--;
                GameManager.instance.UpdateRemainingPathTilesText(tilesNotCollected, pathLength);
                pathTilemap.SetTile(tileGridLocation, null);
            }
        }
        lastCollectedTileCoordinates = tileGridLocation;
    }
}
