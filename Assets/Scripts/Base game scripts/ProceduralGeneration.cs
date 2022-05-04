using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding; // A* by arongranberg

public class ProceduralGeneration : MonoBehaviour {

    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] float smoothness;
    [SerializeField] float seed;
    [Range(0, 1)]
    [SerializeField] float modifier;
    [SerializeField] TileBase caveTile;
    [SerializeField] TileBase borderTile;
    public int perlinHeight;

    private Transform Player;
    private Transform Portal;
    private GameObject[] enemies;
    private Tilemap groundTileMap, caveTileMap, borderTileMap;
    private Tilemap[] tileMaps;
    private OreDatabase oreDatabase;
    public EnemyDatabase enemyDatabase;
    private GridGraph gridGraph;
    private Vector3 center;
    int[,] map;



    void Start()
    {
        tileMaps = GameObject.FindObjectsOfType<Tilemap>();
        oreDatabase = GetComponent<OreDatabase>();
        enemyDatabase = GetComponent<EnemyDatabase>();
        
        // Seperatating all tilemaps
        groundTileMap = getGroundMap();
        caveTileMap = getCaveMap();
        borderTileMap = getBorderMap();

        
        // Finding all necessary transforms
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Portal = GameObject.FindGameObjectWithTag("Portal").transform;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        gridGraph = AstarPath.active.data.gridGraph;
        Generation();
        clearanceForObject(Player);
        clearanceForObject(Portal);
        clearanceForObjects(enemies);
    }

    private Tilemap getBorderMap()
    {
        Tilemap border = null;
        for (var i = 0; i < tileMaps.Length; i++)
        {
            if (tileMaps[i].gameObject.name == "Border")
            {
                border = tileMaps[i];
                return border;
            }
        }
        return border;
    }

    private Tilemap getGroundMap()
    {
        Tilemap ground = null;
        for (var i = 0; i < tileMaps.Length; i++)
        {
            if (tileMaps[i].gameObject.name == "GroundTileMap")
            {
                ground = tileMaps[i];
                return ground;
            }
        }
        return ground;
    }

    private Tilemap getCaveMap()
    {
        Tilemap cave = null;
        for (var i = 0; i < tileMaps.Length; i++)
        {
            if (tileMaps[i].gameObject.name == "CaveTileMap")
            {
                cave = tileMaps[i];
                return cave;
            }
        }
        return cave;
    }


    // Main function to call outside of class in order to activate world generation
    public void Generation()
    {
        seed = Random.Range(-10000, 10000);
        clearMap();
        groundTileMap.ClearAllTiles();
        map = GenerateArray(width, height, true);
        map = TerrainGeneration(map);
        RenderMap(map);
        RenderBorder();
        center = groundTileMap.cellBounds.center;
        gridGraph.SetDimensions(width, perlinHeight, 1f);
        gridGraph.center = center;
    }


    int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y< height; y++)
            {
                map[x, y] = (empty) ? 0 : 1;
            }
        }
        return map;
    }

    int[,] TerrainGeneration(int[,] map)
    {
       
        //int perlinHeight;
        for(int x = 0; x < width; x++)
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x, seed) * height / 2);
            perlinHeight += height / 2;
            for(int y = 0; y < perlinHeight; y++)
            {
                int caveValue = Mathf.RoundToInt(Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed));
                map[x, y] = (caveValue == 1) ? 2 : 1; 
            }
        }
        return map;
    }

    void RenderMap(int[,] map)
    {
        TileBase selectedOre = oreDatabase.chooseOre();
        int numSpawn = Random.Range(1, 5);
        int randomXRange = Random.Range(0, width);
        int randomYRange = Random.Range(0, height);
        int count = 0;
        Debug.Log("Number of enemies to be spawned: " + numSpawn);
        for(int x = 0; x <width; x++)
        {
            for (int y = 0; y < height; y++) 
            {
                if(map[x,y] == 1)
                {
                    groundTileMap.SetTile(new Vector3Int(x, y, 0), selectedOre);
                } 
                else if(map[x,y] == 2)
                {
                    caveTileMap.SetTile(new Vector3Int(x, y, 0), caveTile);
                    if(count < numSpawn)
                    {
                        if(x >= randomXRange && y >= randomYRange)
                        {
                            enemyDatabase.SpawnEnemy(new Vector3Int(x, y, 0));
                            count++;
                        }
                    }
                }
            }
        }
    }

    void RenderBorder()
    {
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y< perlinHeight; y++)
            {
                borderTileMap.SetTile(new Vector3Int(-1, y, 0), borderTile);
                borderTileMap.SetTile(new Vector3Int(width, y, 0), borderTile);
                borderTileMap.SetTile(new Vector3Int(x, -1, 0), borderTile);
                borderTileMap.SetTile(new Vector3Int(x, perlinHeight, 0), borderTile);
            }
        }
    }

    public void clearanceForObject(Transform gameObject)
    {
        Vector3 pos = gameObject.position;
        Vector3Int location = groundTileMap.WorldToCell(pos);
        groundTileMap.SetTile(location, null);
        caveTileMap.SetTile(location, caveTile);
        Debug.Log("Cleared tile for " , gameObject);
    }

    public void clearanceForObjects(GameObject[] gameObjects)
    {
        foreach(GameObject obj in gameObjects)
        {
            Vector3 pos = obj.transform.position;
            Vector3Int location = groundTileMap.WorldToCell(pos);
            groundTileMap.SetTile(location, null);
            caveTileMap.SetTile(location, caveTile);
            Debug.Log("Cleared tile for " + gameObject.ToString());
        }    
    }

    void clearMap()
    {
        groundTileMap.ClearAllTiles();
        caveTileMap.ClearAllTiles();
    }

    public void changeTile(Vector3Int tileLocation)
    {
        TileBase selectedOre = oreDatabase.chooseOre();
        groundTileMap.SetTile(tileLocation, selectedOre);
    }
}
