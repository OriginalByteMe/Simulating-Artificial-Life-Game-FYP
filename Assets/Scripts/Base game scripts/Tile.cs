using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile 
{
    public bool isAlive = false;
    public int numNeighbors = 0;
    private Vector3Int position;
    private TileBase groundTile, caveTile;
    private Tilemap groundMap, caveMap;
    OreDatabase ores;
    public Tile(Vector3Int pos, TileBase groundTile, TileBase caveTile, Tilemap groundMap, Tilemap caveMap, OreDatabase ores)
    {
        this.position = pos;
        this.groundTile = groundTile;
        this.caveTile = caveTile;
        this.caveMap = caveMap;
        this.groundMap = groundMap;
        this.ores = ores;
    }

    //TODO: Change the alive if statement to choose random ore instead
    public void SetAlive(bool alive)
    {
        TileBase selectedTile = ores.chooseOre();
        isAlive = alive;
        if (alive)
        {
            caveMap.SetTile(position, null);
            groundMap.SetTile(position, selectedTile);
           
        }
        else
        {
            groundMap.SetTile(position, null);
            caveMap.SetTile(position, caveTile);
        }
    }

    public Vector3Int getPosition()
    {
        return position;
    }
}
