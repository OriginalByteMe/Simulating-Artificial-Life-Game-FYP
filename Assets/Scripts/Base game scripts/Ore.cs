using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ore
{
    private string oreName;
    private int oreProbability;
    private int oreValue = 0;
    private TileBase tile;

    public Ore(string oreName, int oreProbability, TileBase tile)
    {
        this.oreName = oreName;
        this.oreProbability = oreProbability;
        this.tile = tile;
    }

    public Ore(string oreName, int oreProbability, int oreValue, TileBase tile)
    {
        this.oreName = oreName; 
        this.oreProbability = oreProbability;
        this.oreValue = oreValue;
        this.tile= tile;
    }

    public string OreName
    {
        get
        {
            return oreName;
        }
        set
        {
            oreName = value;
        }
    }


    public int OreProbability { 
        get
        {
            return oreProbability;
        }
        set
        {
            oreProbability = value;
        }
    }

    public int OreValue
    {
        get
        {
            return oreValue;
        }
        set
        {
            oreValue = value;
        }
    }

    public TileBase Tile
    {
        get
        {
            return tile;
        }
        set
        {
            tile = value;
        }
    }





}
