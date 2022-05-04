using Pathfinding; // A* pathfinding by arongranberg
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public enum RuleString
{
    Maze,
    Item2,
    Item3
};

public class Game : MonoBehaviour
{

    [Header("Procedural Generation Variables")]
    public Tilemap groundTileMap, caveTileMap, borderTileMap;
    public Tilemap[] tileMaps;
    private int perlinHeight;

    [Header("Cave generation")]
    [Range(0, 1)]
    [SerializeField] float modifier;
    [SerializeField] TileBase caveTile;
    [SerializeField] TileBase borderTile;


    [Header("Ground Tile for Conway sim")]

    [SerializeField] TileBase ground_Tile;

    [Space(20)]

    [Header("Rule String")]
    //public RuleString ruleString;
    private List<string> rules = new List<string>();
    [Header("Conway Sim Variables")]
    public List<int> Survive = new List<int>();
    public List<int> Birth = new List<int>();
    public float speedOfSim = 0.1f;
    public int SimDuration = 100;

    [Space(10)]
    [Header("Misc")]
    private ProceduralGeneration generate;

    public EnemyDatabase enemyDatabase;

    // Ore probability and database variables
    private OreDatabase oreDatabase;

    // The entire conways grid mapped out
    Tile[,] grid;

    private Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        var guo = new GraphUpdateObject();
        enemyDatabase.GetComponent<EnemyDatabase>();
        oreDatabase = GetComponent<OreDatabase>();
        timer = FindObjectOfType<Timer>();
        tileMaps = GameObject.FindObjectsOfType<Tilemap>();
        // Seperatating all tilemaps
        groundTileMap = getGroundMap();
        caveTileMap = getCaveMap();
        borderTileMap = getBorderMap();

        generate = GetComponent<ProceduralGeneration>();
        generate.Generation();
        perlinHeight = generate.perlinHeight;
        grid = createGrid();
        StartCoroutine(GridScanRoutine());
        populateRuleString();
    }

    // Update is called once per frame
    void Update()
    {
        CountNeighbors();

        if (!timer.getCountDownTimeFinished)
        {
            CountNeighbors();
            PopulationControl();
        }

        UserInput();
    }

    private IEnumerator GridScanRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        yield return wait;
        AstarPath.active.Scan();
    }


    // TODO: MIGHT BE UNUSED PLEASE CHECK AND REMOVE 
    public ProceduralGeneration getGeneration()
    {
        return generate;
    }

    // -----------------------------------
    // Get Tilemaps from the list of tilemaps
    // -----------------------------------
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

    // Create a reference grid for the conways rules to act upon
    public Tile[,] createGrid()
    {
        groundTileMap.CompressBounds();
        BoundsInt bounds = groundTileMap.cellBounds;
        Tile[,] grid = new Tile[bounds.size.x, bounds.size.y];

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                var px = bounds.xMin + x;
                var py = bounds.yMin + y;


                if (groundTileMap.GetTile(new Vector3Int(px, py, 0)))
                {
                    grid[x, y] = new Tile(new Vector3Int(px, py, 0), ground_Tile, caveTile, groundTileMap, caveTileMap, oreDatabase);
                    grid[x, y].SetAlive(true);
                }
                else
                {
                    grid[x, y] = new Tile(new Vector3Int(px, py, 0), ground_Tile, caveTile, groundTileMap, caveTileMap, oreDatabase);
                }
            }
        }
        return grid;
    }

    void UserInput()
    {

        // For debug purposes
        if (Input.GetKeyUp(KeyCode.R))
        {
            generate.Generation();
            perlinHeight = generate.perlinHeight;
            grid = createGrid();

        }
    }

    // Counts all neighbouring cells (helps determine how rules are to be activated)
    void CountNeighbors()
    {
        groundTileMap.CompressBounds();
        BoundsInt bounds = groundTileMap.cellBounds;
        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {
                int numNeighbors = 0;

                // First check for out of bounds condition, if not out of bounds then execute 

                // North
                if (y + 1 < bounds.size.y)
                {
                    if (grid[x, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // South
                if (y - 1 >= bounds.yMin)
                {
                    if (grid[x, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // East
                if (x + 1 < bounds.size.x)
                {
                    if (grid[x + 1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // West
                if (x - 1 >= bounds.xMin)
                {
                    if (grid[x - 1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // North East
                if (x + 1 < bounds.size.x && y + 1 < bounds.size.y)
                {
                    if (grid[x + 1, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // North West
                if (x - 1 >= bounds.xMin && y + 1 < bounds.size.y)
                {
                    if (grid[x - 1, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // South East
                if (y - 1 >= bounds.yMin && x + 1 < bounds.size.x)
                {
                    if (grid[x + 1, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                // South West
                if (y - 1 >= bounds.yMin && x - 1 >= bounds.xMin)
                {
                    if (grid[x - 1, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                grid[x, y].numNeighbors = numNeighbors;
                //Debug.Log( "Num neighbours for this cell: "+ numNeighbors);
            }
        }

    }

    private void populateRuleString()
    {
        rules.Add("3/12345");
        rules.Add("34/456");
    }

    private string chooseRandomRuleString()
    {
        int randomNum = Random.Range(0, rules.Count);

        return rules[randomNum];
    }
    // Translates the rulestring into an array usable by the program
    private void translateRuleString(string ruleString)
    {
        string[] tokens = ruleString.Split('/');
        int Btemp = Int32.Parse(tokens[0]);
        int Stemp = Int32.Parse(tokens[1]);

        while (Btemp > 0)
        {
            Birth.Add(Btemp % 10);
            Btemp = Btemp / 10;
        }
        Birth.Reverse();

        while (Stemp > 0)
        {
            Survive.Add(Stemp % 10);
            Stemp = Stemp / 10;
        }
        Survive.Reverse();

    }

    void PopulationControl()
    {
        translateRuleString(chooseRandomRuleString());
        groundTileMap.CompressBounds();
        BoundsInt bounds = groundTileMap.cellBounds;
        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {

                // Alive cell
                if (grid[x, y].isAlive)
                {
                    //bool ExistsInSruvive = Array.IndexOf(Survive, grid[x, y].numNeighbors) != -1;
                    bool ExistsInSruvive = Survive.IndexOf(grid[x, y].numNeighbors) != -1;

                    if (!ExistsInSruvive)
                    {
                        grid[x, y].SetAlive(false);
                    }
                }
                // Dead cell
                else
                {
                    bool ExistsInBirth = Birth.IndexOf(grid[x, y].numNeighbors) != -1;
                    if (ExistsInBirth)
                    {
                        grid[x, y].SetAlive(true);
                    }
                }
            }
        }
    }
}
