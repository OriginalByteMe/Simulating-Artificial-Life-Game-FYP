using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class Boid : MonoBehaviour
{
    [SerializeField] private Animator EnemyAnim;
    private Tilemap[] tileMaps;
    private Tilemap obstacles;
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 accelaration;

    public BoidLevel level;
    public BoidConfig cfg;
    private ProceduralGeneration generation;
    private Game game;

    private Vector3 wanderTarget;
    private bool checkTile = false;
    private Vector3Int previousTilePosition;


    // A* Pathfinding variables

    public GameObject targetRef;
    private Transform target;
    private FOV fov;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    private Vector2 force, direction;

    Path path;
    Seeker seeker;
    Rigidbody2D rb;

    void Start()
    {
        game = FindObjectOfType<Game>();
        generation = game.getGeneration(); 
        EnemyAnim = GetComponent<Animator>();
        level = FindObjectOfType<BoidLevel>();
        cfg = FindObjectOfType<BoidConfig>();
        tileMaps = GameObject.FindObjectsOfType<Tilemap>();
        obstacles = getObstacles();
        position = transform.position;
        velocity = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);

        // Initialiazing A* objects
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        fov = GetComponent<FOV>();
        targetRef = GameObject.FindGameObjectWithTag("Player");
        target = targetRef.transform;
        InvokeRepeating("UpdatePath", 0f, 3f);
    }

    void FixedUpdate()
    {
        if (fov.chasePlayer)
        {
            Chase();
            if (fov.attackPlayer)
            {
                EnemyAnim.Play("Ability");
            }
        }
        else
        {
            DefaultFlockMovement();
        }
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }
    
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }


    void DefaultFlockMovement()
    {
        Vector3Int currentTilePosition = obstacles.WorldToCell(position);
        TileBase currentTile = obstacles.GetTile(currentTilePosition);
        if (previousTilePosition != currentTilePosition)
        {
            checkTile = false;
        }
        if (currentTile == null)
        {
            if (velocity != null)
            {
                EnemyAnim.SetBool("Run", true);
            }
            else
            {
                EnemyAnim.SetBool("Run", false);
                Debug.Log("Idle");
            }
            accelaration = Combine();
            accelaration = Vector3.ClampMagnitude(accelaration, cfg.maxAcceleration);
            velocity = velocity + accelaration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, cfg.maxVelocity);
            position = position + velocity * Time.deltaTime;
            WrapAround(ref position, level.bounds.x, level.bounds.y);
            transform.position = position;
            //checkTile = false;
        }
        else
        {
            if (velocity != null)
            {
                EnemyAnim.SetBool("Run", true);
            }
            else
            {
                EnemyAnim.SetBool("Run", false);
                Debug.Log("Idle");
            }
            accelaration = Combine();
            accelaration = Vector3.ClampMagnitude(accelaration, cfg.maxAcceleration);
            velocity = velocity + accelaration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, cfg.maxVelocity);
            position = position + (velocity / 2) * Time.deltaTime;
            WrapAround(ref position, level.bounds.x, level.bounds.y);
            transform.position = position;
            //getOutOfBorder(ref position); // Don't work rn
            if (checkTile == false)
            {
                generation.changeTile(currentTilePosition);
                checkTile = true;
                previousTilePosition = currentTilePosition;
            }
        }
    }

    // To prevent spawns INSIDE border
    private Tilemap getBorder()
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

    private Tilemap getObstacles()
    {
        Tilemap obstacle = null;
        for (var i = 0; i < tileMaps.Length; i++)
        {
            if (tileMaps[i].gameObject.name == "GroundTileMap")
            {
                obstacle = tileMaps[i];
                return obstacle;
            }
        }
        return obstacle;
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
        if (countMembers == 0)
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

        foreach (var member in members)
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

        foreach (var member in members)
        {
            if (isInFOV(member.position))
            {
                Vector3 movingTowards = this.position - member.position;
                if (movingTowards.magnitude > 0)
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
        var player = level.GetPlayer(this, cfg.avoidanceRadius);
        if (player == null)
            return avoidVector;
        avoidVector += RunAway(player.position);
        
        return avoidVector.normalized;
    }

    void Chase()
    {
        position = transform.position;
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void Attack()
    {

    }

    Vector3 RunAway(Vector3 target)
    {
        Vector3 neededVelocity = (position - target).normalized * cfg.maxVelocity;
        return neededVelocity - velocity;
    }


    virtual protected Vector3 Combine()
    {
        Vector3 finalVec = cfg.cohesionPriority * Cohesion() + cfg.wanderPriority * Wander()
            + cfg.alignmentPriority * Alignment() + cfg.separationPriority * Seperation();
        return finalVec;
    }

    void getOutOfBorder(ref Vector3 vector)
    {
        Tilemap border = getBorder();
        Vector3Int pos = new Vector3Int(Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.x), 0);
        if (border.GetTile(pos) != null)
        {
            vector.x += 10;
            vector.y += 10;
        }
    }
    // This will cause the boids to wrap around the bounds of the screen, e.g. if it goes out of bounds on left it will wrap around to the right and appear there
    void WrapAround(ref Vector3 vector, float x, float y)
    {
        vector.x = WrapAroundFloat(vector.x, 0, x);
        vector.y = WrapAroundFloat(vector.y, 0, y);
        vector.z = WrapAroundFloat(vector.z, 0, 0);
    }

    float WrapAroundFloat(float value, float min, float max)
    {
        if (value > max)
        {
            value = min;
        }
        else if (value < min)
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
