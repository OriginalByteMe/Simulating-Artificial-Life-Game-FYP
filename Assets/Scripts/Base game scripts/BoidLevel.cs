using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoidLevel : MonoBehaviour
{  
    public Transform boidPrefab;
    [SerializeField]public int spawnLimit = 10;
    public List<Boid> members;
    private EnemyDatabase enemyDatabase;
    private GameObject player;
    private ProceduralGeneration generation;
    private Tilemap borderMap;
    private Tilemap[] tileMaps;
    public Vector3 bounds;
    public float spawnRadius;

    // Start is called before the first frame update
    void Start()
    {
        enemyDatabase = FindObjectOfType<EnemyDatabase>();
        members = enemyDatabase.BoidList;

        // Setting bounds for boids
        generation = FindObjectOfType<ProceduralGeneration>();
        bounds.x   = generation.width;
        bounds.y   = generation.perlinHeight;

        // Locating 
        //members.AddRange(FindObjectsOfType<Boid>());
        player     = GameObject.FindGameObjectWithTag("Player");

        //Getting all tilemaps for finding border later
        tileMaps   = GameObject.FindObjectsOfType<Tilemap>();
    }

    //public IEnumerator Spawn(Transform prefab, int count, Vector3 spawnLocation)
    //{
    //    yield return new WaitForSeconds(1f);
    //    if (members.Count >= spawnLimit)
    //        yield break;

    //    // Note: May have to be more detailed incase it spawns in the top or right, we will see though   
    //    for (int i = 0; i < count; i++)
    //    {
    //        Instantiate(prefab, spawnLocation, Quaternion.identity);
    //        members.AddRange(FindObjectsOfType<Boid>());
    //    }      
    //}

    public List<Boid> GetNeighbors(Boid member, float radius)
    {
        List<Boid> neighborsFound = new List<Boid>();

        foreach (var otherMember in members)
        {
            if (otherMember == member)
                continue;
            if (Vector3.Distance(member.position, otherMember.position) <= radius)
            {
                neighborsFound.Add(otherMember);
            }
        }
        return neighborsFound;
    }

    public Transform GetPlayer(Boid member, float radius)
    {  
        if (Vector3.Distance(member.position, player.transform.position) <= radius)
        {
            return player.transform;
        }
        return null;
    }
}
