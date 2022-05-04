using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cell : MonoBehaviour
{
    public bool isAlive = false;
    public int numNeighbors = 0;

    private void OnMouseOver()
    {
        Debug.Log("Mouse is over GameObject.");
    }
    public void SetAlive(bool alive)
    {
        isAlive = alive;
        if(alive)
        {
            GetComponent<TilemapRenderer>().enabled = true;
        }
        else
        {
            GetComponent<TilemapRenderer>().enabled = false;
        }
    }
}
