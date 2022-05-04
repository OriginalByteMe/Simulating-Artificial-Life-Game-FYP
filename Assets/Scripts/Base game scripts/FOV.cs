using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    public float chaseRadius;
    public float attackRadius;
    [Range(1,360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool chasePlayer { get; private set; }
    public bool attackPlayer { get; private set; }

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while(true)
        {
            yield return wait;
            FieldOfViewChecK();
        }
    }


    private void FieldOfViewChecK()
    {
        Collider2D[] chaseRangeChecks = Physics2D.OverlapCircleAll(transform.position, chaseRadius, targetMask);
        Collider2D[] attackRangeChecks = Physics2D.OverlapCircleAll(transform.position, chaseRadius, targetMask);
        if (chaseRangeChecks.Length > 0)
        {
            Transform target = chaseRangeChecks[0].transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;

            if (Vector2.Angle(transform.up, directionToTarget) < angle / 2)
            {

                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    chasePlayer = true;
                else
                    chasePlayer = false;
            }
            else
                chasePlayer = false;
        }
        else if (chasePlayer)
            chasePlayer = false;


        if(attackRangeChecks.Length > 0)
        {
            for(int i = 0; i < attackRangeChecks.Length; i++)
            {
                Transform target = attackRangeChecks[i].transform;
                Vector2 directionToTarget = (target.position - transform.position).normalized;

                if (Vector2.Angle(transform.up, directionToTarget) < angle / 2)
                {

                    float distanceToTarget = Vector2.Distance(transform.position, target.position);

                    if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                        attackPlayer = true;
                    else
                        attackPlayer = false;
                }
                else
                    attackPlayer = false;
            }
            
        }
        else if (attackPlayer)
            attackPlayer = false;
    }

    
}
