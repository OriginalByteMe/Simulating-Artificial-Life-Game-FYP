using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding; // A* pathfinding by arongranberg

public class GroundAi : MonoBehaviour
{
    public Transform target;
    private FOV fov;
    public float speed = 200f;
    public float walkSpeed = 100f;
    public float nextWaypointDistance = 3f;

    public Transform enemyGFX;

    public Animator EnemyAnim;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    private Vector2 force, direction;

    private bool mustTurn = false;
    public BoidLevel level;



    public Transform groundCheckPos;
    public LayerMask groundLayer;
    public Collider2D collider;


    private Seeker seeker;
    private Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        fov = GetComponent<FOV>();
        level = FindObjectOfType<BoidLevel>();
        target = GameObject.FindWithTag("Player").transform;
        InvokeRepeating("UpdatePath", 0f, .5f);
 
    }

    // Update is called once per frame
    void FixedUpdate()
    {



        if (fov.chasePlayer)
        {
            pathFinding();
            if (fov.attackPlayer)
            {
                //Invoke("spawnEnemies", 5);
            }
        }
        else
        {
            mustTurn = Physics2D.OverlapCircle(groundCheckPos.position, 0.1f, groundLayer);
            Patrol();
        }


        if (force.x >= 0.01f)
        {
            enemyGFX.localScale = new Vector3(6f, 6f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            enemyGFX.localScale = new Vector3(-6f, 6f, 1f);
        }

        if (force.x != 0f && force.y != 0f)
        {
            EnemyAnim.SetBool("Run", true);
        }
        //else
        //{
        //    EnemyAnim.SetBool("Run", false);
        //}

    }

    //private void spawnEnemies()
    //{
    //    EnemyAnim.SetTrigger("Ability");
    //    StartCoroutine(level.Spawn(level.boidPrefab, 5, transform.position));
    //}

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

    void pathFinding()
    {
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
        direction.y = 0f;
        force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void Patrol()
    {
        EnemyAnim.SetBool("Run", true);
        if (mustTurn || collider.IsTouchingLayers(groundLayer))
        {
            Flip();
        }
        rb.velocity = new Vector2(walkSpeed * Time.fixedDeltaTime, rb.velocity.y);
    }

    void Flip()
    {
        //EnemyAnim.SetBool("Run", false);
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        walkSpeed *= -1;
    }
}
