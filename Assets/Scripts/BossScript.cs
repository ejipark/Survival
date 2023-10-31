using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossScript : MonoBehaviour
{
    public Camera spotCam;
    public Collider playerCollider;
    public GameObject supplyBox;

    // Private values
    NavMeshAgent bossAgent;
    Collider capCollider;
    Animator animator;
    AudioSource audioSource;
    Plane[] cameraFrostum;
    private float currHealth;
    private bool wakeUp;
    bool prevAttack;

    [Header("Boss")]
    public float bossHealth;
    public float bossDamage;
    public float attackRadius;
    public float wakeUpTime;
    public float timePerAttack;
    public float bossSpeed;
    public float acceleration;
    public AudioClip dieSound;

    [Header("Trackers")]
    public bool playerVisibleRadius;
    public bool playerAttackRadius;
    public GameTrackerScript gameTracker;

    [Header("Player")]
    public Transform lookPoint;
    public GameObject player;
    public LayerMask playerLayer;
    public PlayerScript playerScript;

    [Header("Music")]
    public GameObject normalMusic;
    public GameObject intenseMusic;

    public enum AIState
    {
        Chase,
        Attack,
        Death,
    };
    public AIState aiState;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = bossHealth;
        bossAgent = GetComponent<NavMeshAgent>();
        capCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        wakeUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        cameraFrostum = GeometryUtility.CalculateFrustumPlanes(spotCam);
        if (!wakeUp && GeometryUtility.TestPlanesAABB(cameraFrostum, playerCollider.bounds))
        {
            normalMusic.SetActive(false);
            intenseMusic.SetActive(true);
            StartCoroutine(WakeUp());
        }

        if (wakeUp)
        {
            playerAttackRadius = Physics.CheckSphere(transform.position, attackRadius, playerLayer);

            switch (aiState)
            {
                case AIState.Chase:
                    if (playerAttackRadius)
                    {
                        Attack();
                        aiState = AIState.Attack;
                    }
                    else
                    {
                        if (bossAgent.remainingDistance < 3 && bossAgent.pathPending != true)
                        {
                            SetNextWaypoint();
                        }
                    }
                    break;
                case AIState.Attack:
                    if (playerAttackRadius)
                    {
                        Attack();
                    }
                    else
                    {
                        SetNextWaypoint();
                    }
                    break;
                case AIState.Death:
                    StopBoss();
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator WakeUp()
    {
        animator.SetBool("Spot", true);
        transform.LookAt(lookPoint);
        yield return new WaitForSeconds(wakeUpTime);
        wakeUp = true;
        SetNextWaypoint();
    }

    private void SetNextWaypoint()
    {
        animator.SetBool("Run", true);
        animator.SetBool("Attack", false);
        // now will start moving
        SetDestination(lookPoint.position);
        aiState = AIState.Chase;
    }

    private void Attack()
    {
        if (!prevAttack)
        {
            StartCoroutine(AttackMotion());
            SetDestination(transform.position);
            prevAttack = true;
            Invoke(nameof(ActiveAttack), 1f);
        }
    }

    private void ActiveAttack()
    {
        prevAttack = false;
    }

    IEnumerator AttackMotion()
    {
        StopBoss();
        transform.LookAt(lookPoint);
        animator.SetBool("Attack", true);
        animator.SetBool("Run", false);

        playerScript.PlayerTakeDamage(bossDamage);
        yield return new WaitForSeconds(timePerAttack);

        bossAgent.isStopped = false;
        bossAgent.speed = bossSpeed;
        bossAgent.acceleration = acceleration;
    }

    private void StopBoss()
    {
        bossAgent.ResetPath();
        bossAgent.speed = 0f;
        bossAgent.acceleration = 0f;
        bossAgent.velocity = Vector3.zero;
        bossAgent.isStopped = true;
    }

    private void SetDestination(Vector3 target)
    {
        if (bossAgent.hasPath)
        {
            bossAgent.destination = target;
        }
        else
        {
            bossAgent.SetDestination(target);
        }
    }

    public void BossTakeDamage(float amount)
    {
        currHealth -= amount;
        if (currHealth <= 0)
        {
            animator.SetBool("Die", true);
            BossDie();
        }
    }

    private void BossDie()
    {
        StopBoss();
        aiState = AIState.Death;
        attackRadius = 0f;
        playerAttackRadius = false;
        playerVisibleRadius = false;
        capCollider.enabled = false;

        audioSource.PlayOneShot(dieSound);
        Object.Destroy(gameObject, 10f);
        gameTracker.miniBossDead += 1;
        intenseMusic.SetActive(false);
        normalMusic.SetActive(true);
        supplyBox.transform.position = transform.position;
        supplyBox.SetActive(true);
    }
}
