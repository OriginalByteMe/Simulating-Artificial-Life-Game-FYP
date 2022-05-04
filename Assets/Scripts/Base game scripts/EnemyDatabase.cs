using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyDatabase : MonoBehaviour
{
    [Range(0, 1000)]
    [SerializeField] int GolemProbability, GolemPlusProabability, SlimeProababilty, RatPorbability, BatProbability, RockProbability;
    [SerializeField] Transform GolemPrefab, GolemPlusPrefab, SlimePrefab, RatPrefab, BatPrefab, RockPrefab;
    private List<Enemy> enemyList = new List<Enemy>();
    public int GolemSpawnLimit = 3;
    public int SlimeSpawnLimit = 3;
    public int RatSpawnLimit = 3;
    public int BoidSpawnLimit = 5;
    public int GolemPlusSpawnLimit = 2;

    int numPrefabs = 2;
    private List<int> probList = new List<int>();
    public List<EnemyGolemAI> golemMembers = new List<EnemyGolemAI>();
    public List<GroundAi> groundMembers = new List<GroundAi>();
    public List<Boid> boidMembers = new List<Boid>();

    void Awake()
    {
        enemyDicitonaryFiller();
    }


    // Populate dictionary with enemy types
    void enemyDicitonaryFiller()
    {
        Enemy Golem = new Enemy("Golem", GolemProbability, GolemPrefab);
        Enemy GolemPlus = new Enemy("Golem+", GolemPlusProabability, GolemPlusPrefab);
        Enemy Slime = new Enemy("Slime", SlimeProababilty, SlimePrefab);
        Enemy Bat = new Enemy("Bat", BatProbability, BatPrefab);
        Enemy Rat = new Enemy("Rat", RatPorbability, RatPrefab);
        Enemy Rock = new Enemy("Rock", RockProbability, RockPrefab);
        enemyList.Add(Golem);
        enemyList.Add(GolemPlus);
        enemyList.Add(Slime);
        enemyList.Add(Bat);
        enemyList.Add(Rat);
        enemyList.Add(Rock);
    }

    // Spawn a random enemy at a set locaiton
    public void SpawnEnemy(Vector3 spawnLocation)
    {
        Enemy enemy = chooseEnemy();
        
        if(enemy.EnemyName == "Golem")
        {
            if (golemMembers.Count >= GolemSpawnLimit)
                return; 
            Instantiate(GolemPrefab, spawnLocation, Quaternion.identity);
            golemMembers.AddRange(FindObjectsOfType<EnemyGolemAI>());
            
        }
        else if (enemy.EnemyName == "Golem+")
        {
            if (golemMembers.Count >= GolemPlusSpawnLimit)
                return;
            Instantiate(GolemPlusPrefab, spawnLocation, Quaternion.identity);
            golemMembers.AddRange(FindObjectsOfType<EnemyGolemAI>());
        }
        else if(enemy.EnemyName == "Slime")
        {
            if (groundMembers.Count >= SlimeSpawnLimit)
                return;
            Instantiate(SlimePrefab, spawnLocation, Quaternion.identity);
            groundMembers.AddRange(FindObjectsOfType<GroundAi>());
        }
        else if (enemy.EnemyName == "Rat")
        {
            if (groundMembers.Count >= RatSpawnLimit)
                return;
            Instantiate(RatPrefab, spawnLocation, Quaternion.identity);
            groundMembers.AddRange(FindObjectsOfType<GroundAi>());
        }
        
    }

    // Spawn a boids on boid spawners
    public IEnumerator SpawnMinion( string minionType, Vector3 pos)
    {
        yield return new WaitForSeconds(1f);
        if (boidMembers.Count >= BoidSpawnLimit)
            yield break;
        if (minionType == "Rock")
        {
            
            Instantiate(RockPrefab, pos, Quaternion.identity);
            boidMembers.AddRange(FindObjectsOfType<Boid>());

        }
        else if (minionType == "Bat")
        {
            Instantiate(BatPrefab, pos, Quaternion.identity);
            boidMembers.AddRange(FindObjectsOfType<Boid>());
        }
    }

    // Choose random enemy according to propability 
    private Enemy chooseEnemy()
    {
        // Get the total sum of all weights
        int weightSum = 0;
        int index = 0;
        foreach (var prob in enemyList)
        {
            weightSum += prob.Probability;
            probList.Add(prob.Probability);
        }
        foreach (var enemy in enemyList)
        {
            if (Random.Range(0, weightSum) < enemy.Probability)
            {  
                return enemy;
            }
            weightSum -= probList[index++];
        }
        return null;
    }

    public List<Boid> BoidList { 
        get
        {
            return boidMembers;
        }
        
    }
}
