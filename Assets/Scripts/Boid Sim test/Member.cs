using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member : MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 accelaration;

    public Level level;
    public MemberConfig cfg;


    private Vector3 wanderTarget;
    private void Start()
    {
        level = FindObjectOfType<Level>();
        cfg = FindObjectOfType<MemberConfig>();

        position = transform.position;
        velocity = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
    }

    private void Update()
    {
        accelaration = Combine();
        accelaration = Vector3.ClampMagnitude(accelaration, cfg.maxAcceleration);
        velocity = velocity + accelaration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, cfg.maxVelocity);
        position = position + velocity * Time.deltaTime;
        WrapAround(ref position, -level.bounds, level.bounds);
        transform.position = position;
    }


    // Causes members to wander randomly
    protected Vector3 Wander()
    {
        float jitter = cfg.wanderJitter * Time.deltaTime;
        wanderTarget += new Vector3(RandomBinomial() * jitter, RandomBinomial() * jitter, 0);
        wanderTarget = wanderTarget.normalized;
        wanderTarget *= cfg.wanderRadius;
        Vector3 targetInLocalSpace = wanderTarget + new Vector3(cfg.wanderDistance, cfg.wanderDistance, 0);
        Vector3 targetInWorldSpace = transform.TransformPoint(targetInLocalSpace);
        targetInWorldSpace -= this.position;

        return targetInWorldSpace.normalized;

    }

    Vector3 Cohesion()
    {
        Vector3 cohesionVector = new Vector3();
        int countMembers = 0;
        var neighbors = level.GetNeighbors(this, cfg.cohesionRadius);
        if (neighbors.Count == 0)
            return cohesionVector;
        foreach (var member in neighbors)
        {
            if (isInFOV(member.position))
            {
                cohesionVector += member.position;
                countMembers++;
            }
        }
        if(countMembers == 0)      
            return cohesionVector;

        cohesionVector /= countMembers;
        cohesionVector = cohesionVector - this.position;
        cohesionVector = Vector3.Normalize(cohesionVector);
        return cohesionVector;
    }

    Vector3 Alignment()
    {
        Vector3 alignVector = new Vector3();
        var members = level.GetNeighbors(this, cfg.alignmentRadius);
        if (members.Count == 0)
            return alignVector;

        foreach(var member in members)
        {
            if (isInFOV(member.position))
                alignVector += member.velocity;
        }

        return alignVector.normalized;
    }

    Vector3 Seperation()
    {
        Vector3 seperateVector = new Vector3();
        var members = level.GetNeighbors(this, cfg.separationRadius);
        if (members.Count == 0)
            return seperateVector;

        foreach(var member in members)
        {
            if(isInFOV(member.position))
            {
                Vector3 movingTowards = this.position - member.position;
                if(movingTowards.magnitude > 0)
                {
                    seperateVector += movingTowards.normalized / movingTowards.magnitude;
                }
            }
        }
        return seperateVector.normalized;
    }

    Vector3 Avoidance()
    {
        Vector3 avoidVector = new Vector3();
        var enemyList = level.GetEnemies(this, cfg.avoidanceRadius);
        if (enemyList.Count == 0)
            return avoidVector;
        foreach (var enemy in enemyList)
        {
            avoidVector += RunAway(enemy.position);
        }
        return avoidVector.normalized;
    }

    Vector3 RunAway(Vector3 target)
    {
        Vector3 neededVelocity = (position - target).normalized * cfg.maxVelocity;
        return neededVelocity - velocity;
    }


    // Combine together all of the different vector variables
    virtual protected Vector3 Combine()
    {
        Vector3 finalVec = cfg.cohesionPriority * Cohesion() + cfg.wanderPriority * Wander()
            + cfg.alignmentPriority * Alignment() + cfg.separationPriority * Seperation()
            + cfg.avoidancePriority * Avoidance();
        return finalVec;
    }

    // This will cause the boids to wrap around the bounds of the screen, e.g. if it goes out of bounds on left it will wrap around to the right and appear there
    void WrapAround(ref Vector3 vector, float min, float max)
    {
        vector.x = WrapAroundFloat(vector.x, min, max);
        vector.y = WrapAroundFloat(vector.y, min, max);
        vector.z = WrapAroundFloat(vector.z, min, max);
    }

    float WrapAroundFloat(float value, float min, float max)
    {
        if(value > max)
        {
            value = min;
        } else if(value < min)
        {
            value = max;
        }

        return value;
    }


    float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

    bool isInFOV(Vector3 vec)
    {
        return Vector3.Angle(this.velocity, vec - this.position) <= cfg.maxFOV;
    }
}
