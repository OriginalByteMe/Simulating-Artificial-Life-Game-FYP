using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    [SerializeField] private Animator EnemyAnim;
    public bool isGolem;

    // Start is called before the first frame update
    public void Start()
    {
        EnemyAnim = GetComponent<Animator>();
    }

    //Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EnemyAnim.SetBool("Run", false);
            Debug.Log("Idle");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EnemyAnim.SetBool("Run", true);
            Debug.Log("Running");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EnemyAnim.SetBool("Run", false);
            EnemyAnim.SetTrigger("Hit");
            Debug.Log("Hit");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EnemyAnim.SetBool("Run", false);
            EnemyAnim.SetTrigger("Death");
            Debug.Log("Death");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (!isGolem)
            {
                EnemyAnim.SetBool("Run", false);
                EnemyAnim.SetTrigger("Ability");
                Debug.Log("Ability");
            }
            else
            {
                EnemyAnim.SetBool("Run", false);
                EnemyAnim.SetBool("Ability", true);
                Debug.Log("Golem Ability");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (!isGolem)
            {
                EnemyAnim.SetBool("Run", false);
                EnemyAnim.SetTrigger("Attack");
                Debug.Log("Attack");
            }
            else
            {
                if (EnemyAnim.GetBool("Ability") == true)
                {
                    EnemyAnim.SetBool("Run", false);
                    EnemyAnim.SetTrigger("Attack");
                    Debug.Log("Golem Attack");
                }
                else
                {
                    Debug.Log("Ability not active");
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            EnemyAnim.SetBool("Run", false);
            EnemyAnim.SetTrigger("Attack 2");
            Debug.Log("Attack 2");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            EnemyAnim.SetBool("Run", false);
            EnemyAnim.SetTrigger("Attack 3");
            Debug.Log("Attack 3");
        }
    }
    public void GolemEndAbility()
    {
        EnemyAnim.SetBool("Ability", false);
    }
}
