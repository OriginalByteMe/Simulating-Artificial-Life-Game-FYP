using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;

public class CinemachineSwitcher : MonoBehaviour
{
    private Animator animator;
    private Timer timer;
    private Vector3 center;
    private Tilemap groundTileMap;
    private Tilemap[] tileMaps;
    private bool playerCamera = true;
    [SerializeField]
    private CinemachineVirtualCamera vcam1; // Player Camera

    [SerializeField]
    private CinemachineVirtualCamera vcam2; // Sim Camera


    private void Awake()
    {
        animator = GetComponent<Animator>();
        timer = FindObjectOfType<Timer>();
        //groundTileMap = getGroundMap();
        //center = groundTileMap.cellBounds.center;
    }
    //private Tilemap getGroundMap()
    //{
    //    Tilemap ground = null;
    //    for (var i = 0; i < tileMaps.Length; i++)
    //    {
    //        if (tileMaps[i].gameObject.name == "GroundTileMap")
    //        {
    //            ground = tileMaps[i];
    //            return ground;
    //        }
    //    }
    //    return ground;
    //}
    private void SwitchPriority(string camera)
    {
        if(camera == "Sim")
        {
            if (playerCamera)
            {
                vcam1.Priority = 0;
                vcam2.Priority = 1;
            }
        }

        if(camera == "Player")
        {
            
            if (!playerCamera)
            {
                vcam1.Priority = 1;
                vcam2.Priority = 0;
            }
        }
        
        playerCamera = !playerCamera;
    }


    // Update is called once per frame
    void Update()
    {
        if(!timer.getCountDownTimeFinished)
        {
            SwitchPriority("Sim");
        }

        if(!timer.getSimTimeFinished)
        {
            SwitchPriority("Player");
        }
    }
}
