using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class OreDatabase: MonoBehaviour {
    private List<int> probList = new List<int>();
    private List<Ore> oreList = new List<Ore>();
    [Header("Ores")]

    [Range(0, 1000)]
    [SerializeField] int groundTile_Probability;
    [SerializeField] TileBase ground_Tile;
    [Range(0, 1000)]

    [SerializeField] int Iron_Probability;
    [SerializeField] int Iron_Value;
    [SerializeField] TileBase Iron_Tile;

    [Range(0, 1000)]
    [SerializeField] int Coppere_Probability;
    [SerializeField] int Copper_Value;
    [SerializeField] TileBase Copper_Tile;

    [Range(0, 1000)]
    [SerializeField] int Gold_Probability;
    [SerializeField] int Gold_Value;
    [SerializeField] TileBase Gold_Tile;

    [Range(0, 1000)]
    [SerializeField] int Diamond_Probability;
    [SerializeField] int Diamond_Value;
    [SerializeField] TileBase Diamond_Tile;

    [Range(0, 1000)]
    [SerializeField] int Emerald_Probability;
    [SerializeField] int Emerald_Value;
    [SerializeField] TileBase Emerald_Tile;

    [Range(0, 1000)]
    [SerializeField] int Ruby_Probability;
    [SerializeField] int Ruby_Value;
    [SerializeField] TileBase Ruby_Tile;

    [Range(0, 1000)]
    [SerializeField] int Fossil_Probability;
    [SerializeField] int Fossil_Value;
    [SerializeField] TileBase Fossil_Tile;

    [Range(0, 1000)]
    [SerializeField] int Energy_Crystal_Probability;
    [SerializeField] int Energy_Value;
    [SerializeField] TileBase Energy_Crystal_Tile;


    void Start()
    {
        oreFiller();
    }


    private void oreFiller()
    {
        Ore Dirt = new Ore("Dirt", groundTile_Probability, 0, ground_Tile);
        Ore Iron = new Ore("Iron", Iron_Probability, Iron_Value, Iron_Tile);
        Ore Copper = new Ore("Copper", Coppere_Probability, Copper_Value, Copper_Tile);
        Ore Gold = new Ore("Gold", Gold_Probability, Gold_Value, Gold_Tile);
        Ore Diamond = new Ore("Diamond", Diamond_Probability, Diamond_Value, Diamond_Tile);
        Ore Emerald = new Ore("Emerald", Emerald_Probability, Emerald_Value, Emerald_Tile);
        Ore Ruby = new Ore("Ruby", Ruby_Probability, Ruby_Value, Ruby_Tile);
        Ore Fossil = new Ore("Fossil", Fossil_Probability, Fossil_Value, Fossil_Tile);
        Ore Energy_Crystal = new Ore("Energy_Crystal", Energy_Crystal_Probability, Energy_Value, Energy_Crystal_Tile);

        oreList.Add(Dirt);
        oreList.Add(Iron);
        oreList.Add(Copper);
        oreList.Add(Gold);
        oreList.Add(Diamond);
        oreList.Add(Emerald);
        oreList.Add(Ruby);
        oreList.Add(Fossil);
        oreList.Add(Energy_Crystal);
    }



    public TileBase chooseOre()
    {
        // Get the total sum of all weights
        int weightSum = 0;
        int index = 0;
        foreach(var prob in oreList)
        {
            weightSum += prob.OreProbability;
            probList.Add(prob.OreProbability);
        }
        
        TileBase chosenOre = null;
        foreach(var Ore in oreList)
        {
            if(Random.Range(0,weightSum) < Ore.OreProbability)
            {
                chosenOre = Ore.Tile;
                
                return chosenOre;
            }
            weightSum -= probList[index++];
        }
        return null;
    }

    public int getOreValue(string oreName)
    {
        int value = 0;
        foreach(var ore in oreList)
        {
            if(ore.OreName == oreName)
            {
                value = ore.OreValue;
                Debug.Log("Value = " + value);
                return value;
            }
        }

        return value;
    }
}
