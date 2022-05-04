using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    private string enemyName;
    private int propbability;
    private Transform prefab;
    private int enemy_value = 0;

    public Enemy(string name, int probability, Transform prefab)
    {
        this.enemyName = name;
        this.propbability = probability;
        this.prefab = prefab;
    }

    public Enemy(string name, int probability,int value, Transform prefab)
    {
        this.enemyName = name;
        this.propbability = probability;
        this.enemy_value = value;
        this.prefab = prefab;
    }

    public string EnemyName
    {
        get
        {
            return enemyName;
        }
        set
        {
            enemyName = value;
        }
    }


    public int Probability
    {
        get
        {
            return propbability;
        }
        set
        {
            propbability = value;
        }
    }

    public int Value
    {
        get
        {
            return enemy_value;
        }
        set
        {
            enemy_value = value;
        }
    }

    public Transform Prefab
    {
        get
        {
            return prefab;
        }
        set
        {
            prefab = value;
        }
    }
}
