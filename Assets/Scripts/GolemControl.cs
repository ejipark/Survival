using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GolemControl : MonoBehaviour
{
    [SerializeField] private Camera spotCam;
    [SerializeField] private Collider playerCollider;
    private AudioSource audioSource;
    private Plane[] cameraFrostum;
    private Animator anim;
    private UnityEngine.AI.NavMeshAgent agent;

    [Header("Boss")]
    private Rigidbody rbody;
    [SerializeField] private float bossDamage;
    [SerializeField] private float wakeUpTime;
    [SerializeField] private float timePerAttack;
    [SerializeField] private float visionRadius;
    [SerializeField] private float attackRadius;
    [SerializeField] private float bossSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private AudioClip dieSound;
    private BossHealth bossHp;
    private Vector3 initialPosition;
    private Transform boss = null;

    [Header("Music")]
    [SerializeField] private GameObject normalMusic;
    [SerializeField] private GameObject intenseMusic;

    [Header("Player")]
    [SerializeField] private Transform lookPoint;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Camera AttackRaycastArea;
    [SerializeField] private PlayerScript playerScript;
    private Transform player = null;

    [Header("Trackers")]
    [SerializeField] private bool playerVisibleRadius;
    [SerializeField] private bool playerAttackRadius;
    [SerializeField] private MenuScript menu;

    private bool prevAttack;
    private bool wakeUp;
    private bool damageFlag = false;
    private bool rageFlag = false;
    private string targetTag = "Player";
    private float speed = 2.0f;
    private string[] actions1 = new string[] { "Jump", "Rage", "Hit", "Hit2"};
    private string[] actions2 = new string[] { "Fly", "Land" };
    private string[] attackPatterns = new string[] { "Hit", "Hit2" };
    private string prevAction;
    private int idx = 0;
    private int prevIdx = 0;
    //private int idx1 = 0;
    //private int idx2 = 0;
    private float damage = 0f;
    private enum AIState
    {
        Idle,
        Jump, 
        Fly, 
        Land, 
        Chase, 
        Walk,
        Attack,
        Rage, 
        Damage,
        Die
    };
    private AIState aiState;
    private AnimatorStateInfo animState;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        bossHp = GetComponent<BossHealth>();
        playerCollider = GetComponent<Collider>();
        initialPosition = rbody.transform.position;
        agent.speed = speed;
        agent.angularSpeed = 500;
        agent.acceleration = speed * 20;
        player = GameObject.FindWithTag(targetTag).transform;
        damage = bossDamage;
        wakeUp = false;
    }

    //void OnTriggerEnter(Collider c)
    //{
    //    // if (c.attachedRigidbody != null && c.gameObject.tag == "Bomb")
    //    // if (c.gameObject.tag == "Bomb" && c.attachedRigidbody.velocity.magnitude > 0.0f)
    //    if (c.attachedRigidbody.velocity.magnitude > 0.0f)
    //    {
    //        Debug.Log("velocity " + c.attachedRigidbody.velocity.magnitude);
    //        damage = (Random.Range(10, 30)) * (-1);
    //        bossHp.UpdateHealth(damage);
    //        aiState = AIState.Damage;
    //    }
    //}

    void OnCollisionEnter(Collision c)
    {
        if (c.rigidbody)
        {
            c.rigidbody.AddForce(c.rigidbody.velocity * -10, ForceMode.Impulse);
        }
    }

    public void GolemTakeDamage(float amount)
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    bossHp.UpdateHealth(-1000);
        //    aiState = AIState.Damage;
        //}

        if (bossHp.hp <= 0)
        {
            // aiState = AIState.Die;
            BossDie();
        }
        else
        {
            damageFlag = true;
            bossHp.UpdateHealth(-amount);
            aiState = AIState.Damage;
            if ((bossHp.hp <= 0.5 * bossHp.maxHp) && !rageFlag)
            {
                aiState = AIState.Rage;
                damage = 1.5f * bossDamage;
            }
        }
    }

    void FixedUpdate()
    {
        boss = rbody.transform;
        animState = anim.GetCurrentAnimatorStateInfo(0);
        playerVisibleRadius = Physics.CheckSphere(transform.position, visionRadius, playerLayer);
        playerAttackRadius = Physics.CheckSphere(transform.position, attackRadius, playerLayer);
        aiState = AIState.Idle;

        cameraFrostum = GeometryUtility.CalculateFrustumPlanes(spotCam);
        if (!wakeUp && GeometryUtility.TestPlanesAABB(cameraFrostum, playerCollider.bounds))
        {
            damageFlag = false;
            StartCoroutine(WakeUp());
        }
        
        if (!wakeUp && GeometryUtility.TestPlanesAABB(cameraFrostum, playerCollider.bounds) && damageFlag)
        {
            StartCoroutine(WakeUp());
            aiState = AIState.Jump;
        }

        if (wakeUp)
        {
            if (animState.IsName("Walk"))
            {
                if (Vector3.Distance(boss.position, initialPosition) < 1.0f)
                {
                    aiState = AIState.Idle;
                }
            }

            if (bossHp.hp <= 0)
            {
                aiState = AIState.Die;
            }
            else if (Vector3.Distance(boss.position, player.position) < visionRadius || damageFlag)
            {
                normalMusic.SetActive(false);
                intenseMusic.SetActive(true);
                transform.LookAt(lookPoint);
                if (Vector3.Distance(boss.position, player.position) > visionRadius && damageFlag)
                {
                    aiState = AIState.Chase; 
                }
                else if (aiState == AIState.Chase && Vector3.Distance(boss.position, player.position) >= 1.0f)
                {
                    aiState = AIState.Chase;
                }
                else 
                {
                    aiState = AIState.Walk;
                }
                if (Vector3.Distance(boss.position, player.position) < 10.0f)
                {
                    if (AttackMeleeRadius())
                    {
                        aiState = AIState.Attack;
                    }
                    else
                    {
                        aiState = AIState.Walk;
                    }
                    agent.SetDestination(player.position);
                }
                agent.SetDestination(player.position);
            }
            else if (boss.position != initialPosition && damageFlag == false)
            {
                agent.SetDestination(initialPosition);

            }
            else
            {
                aiState = AIState.Idle;
            }

            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            switch (aiState)
            {
                case AIState.Idle:
                    anim.SetTrigger("IdleAction");
                    break;
                case AIState.Rage:
                    anim.SetTrigger("Rage");
                    aiState = AIState.Jump;
                    break;
                case AIState.Chase:
                    anim.SetFloat("Walk", 2.5f);
                    break;
                case AIState.Walk:
                    anim.SetFloat("Walk", 1.0f);
                    break;
                case AIState.Jump:
                    anim.SetTrigger("Jump");
                    aiState = AIState.Fly;
                    break;
                case AIState.Fly:
                    anim.SetTrigger("Fly");
                    // agent.SetDestination(player.position);
                    aiState = AIState.Land;
                    break;
                case AIState.Land:
                    anim.SetTrigger("Land");
                    aiState = AIState.Walk;
                    break;
                case AIState.Attack:
                    idx = Random.Range(0, 2);
                    anim.ResetTrigger(attackPatterns[prevIdx]);
                    anim.SetTrigger(attackPatterns[idx]);
                    Attack();
                    aiState = AIState.Idle;
                    prevIdx = idx;
                    break;
                case AIState.Damage:
                    anim.SetTrigger("Damage");
                    aiState = AIState.Walk;
                    break;
                case AIState.Die:
                    BossDie();
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator WakeUp()
    {
        transform.LookAt(lookPoint);
        yield return new WaitForSeconds(wakeUpTime);
        wakeUp = true;
        SetNextWaypoint();
    }

    private void SetNextWaypoint()
    {
        SetDestination(lookPoint.position);
        aiState = AIState.Walk;
    }


    bool AttackMeleeRadius()
    {
        if (Vector3.Distance(boss.position, player.position) < 3.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StopBoss()
    {
        agent.ResetPath();
        agent.speed = 0f;
        agent.acceleration = 0f;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
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
        playerScript.PlayerTakeDamage(damage);
        yield return new WaitForSeconds(timePerAttack);

        agent.isStopped = false;
        agent.speed = bossSpeed;
        agent.acceleration = acceleration;
    }

    private void SetDestination(Vector3 target)
    {
        if (agent.hasPath)
        {
            agent.destination = target;
        }
        else
        {
            agent.SetDestination(target);
        }
    }

    private void BossDie()
    {
        StopBoss();
        anim.SetFloat("Walk", 0f);
        aiState = AIState.Die;
        attackRadius = 0f;
        playerAttackRadius = false;
        playerVisibleRadius = false;
        playerCollider.enabled = false;

        StartCoroutine(MissionComplete());
    }

    IEnumerator MissionComplete()
    {
        audioSource.PlayOneShot(dieSound);
        anim.SetTrigger("Die");
        anim.ResetTrigger("Die");
        anim.SetTrigger("SleepStart");
        Object.Destroy(gameObject, 10f);
        intenseMusic.SetActive(false);
        normalMusic.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("EndScene");
    }
}
