using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : MonoBehaviour
{
    // Private values
    NavMeshAgent zombieAgent;
    Collider capCollider;
    Animator animator;
    private float currHealth;
    private int currWaypoint = 0;
    bool prevAttack;

    [Header("Configurables")]
    public float zombieHealth;
    public float zombieDamage;
    public float timePerAttack;
    //public float zombieSpeed;
    public float zombieWalkSpeed;
    public float zombieRunSpeed;
    public float acceleration;
    public float visionRadius;
    public float attackRadius;
    public GameObject[] waypoints;

    [Header("Trackers")]
    public bool playerVisibleRadius;
    public bool playerAttackRadius;

    [Header("Player")]
    public Transform lookPoint;
    public PlayerScript playerScript;
    public LayerMask playerLayer;

    [Header("Sound")]
    private AudioSource audioSource;
    public AudioClip[] dieSounds;

    public enum AIState
    {
        Guard,
        Chase,
        Attack,
        Death
    };
    public AIState aiState;

    private void Awake()
    {
        currHealth = zombieHealth;
        zombieAgent = GetComponent<NavMeshAgent>();
        capCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetNextWaypoint();
        aiState = AIState.Guard;
    }

    // Update is called once per frame
    void Update()
    {
        playerVisibleRadius = Physics.CheckSphere(transform.position, visionRadius, playerLayer);
        playerAttackRadius = Physics.CheckSphere(transform.position, attackRadius, playerLayer);

        switch (aiState)
        {
            case AIState.Guard:
                if (playerVisibleRadius && playerAttackRadius)
                {
                    Attack();
                    aiState = AIState.Attack;
                }
                else if (playerVisibleRadius && !playerAttackRadius)
                {
                    Chase();
                    aiState = AIState.Chase;
                }
                else
                {
                    if (zombieAgent.remainingDistance < 2 && zombieAgent.pathPending != true)
                    {
                        SetNextWaypoint();
                    }
                }
                break;
            case AIState.Chase:
                if (playerVisibleRadius && playerAttackRadius)
                {
                    Attack();
                    aiState = AIState.Attack;
                }
                else if (playerVisibleRadius && !playerAttackRadius)
                {
                    Chase();
                }
                else
                {
                    SetNextWaypoint();
                    aiState = AIState.Guard;
                }
                break;
            case AIState.Attack:
                if (playerVisibleRadius && playerAttackRadius)
                {
                    Attack();
                }
                else if (playerVisibleRadius && !playerAttackRadius)
                {
                    Chase();
                    aiState = AIState.Chase;
                }
                else
                {
                    SetNextWaypoint();
                    aiState = AIState.Guard;
                }
                break;
            case AIState.Death:
                StopZombie();
                break;
            default:
                break;
        }
    }

    private void SetNextWaypoint()
    {
        if (currWaypoint >= waypoints.Length)
        {
            currWaypoint = 0;
        }

        animator.SetBool("Walk", true);
        animator.SetBool("Run", false);
        animator.SetBool("Attack", false);
        zombieAgent.speed = zombieWalkSpeed;

        SetDestination(waypoints[currWaypoint].transform.position);
        currWaypoint++;
    }

    private void Attack()
    {
        if (!prevAttack)
        {
            StartCoroutine(AttackMotion());
            SetDestination(transform.position);
            prevAttack = true;
            Invoke(nameof(ActiveAttack), timePerAttack);
        }
    }

    private void ActiveAttack()
    {
        prevAttack = false;
    }

    IEnumerator AttackMotion()
    {
        StopZombie();
        transform.LookAt(lookPoint);

        animator.SetBool("Attack", true); 
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);

        playerScript.PlayerTakeDamage(zombieDamage);
        yield return new WaitForSeconds(timePerAttack);

        zombieAgent.isStopped = false;
        zombieAgent.speed = zombieRunSpeed;
        zombieAgent.acceleration = acceleration;
    }

    private void StopZombie()
    {
        zombieAgent.ResetPath();
        zombieAgent.speed = 0f;
        zombieAgent.acceleration = 0f;
        zombieAgent.velocity = Vector3.zero;
        zombieAgent.isStopped = true;
    }

    private void Chase()
    {
        animator.SetBool("Run", true);
        animator.SetBool("Walk", false);
        animator.SetBool("Attack", false);
        zombieAgent.speed = zombieRunSpeed;
        SetDestination(lookPoint.position);
    }

    private void SetDestination(Vector3 target)
    {
        if (zombieAgent.hasPath)
        {
            zombieAgent.destination = target;
        }
        else
        {
            zombieAgent.SetDestination(target);
        }
    }

    public void ZombieTakeDamage(float amount)
    {
        currHealth -= amount;
        visionRadius = 100;
        if (currHealth <= 0)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Die", true);
            ZombieDie();
        }
    }

    private void ZombieDie()
    {
        StopZombie();
        aiState = AIState.Death;
        //zombieSpeed = 0f;
        attackRadius = 0f;
        visionRadius = 0f;
        playerAttackRadius = false;
        playerVisibleRadius = false;
        capCollider.enabled = false;

        AudioClip clip = dieSounds[Random.Range(0, dieSounds.Length)];
        audioSource.PlayOneShot(clip);
        Object.Destroy(gameObject, 10f);
    }
}
