using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchScript : MonoBehaviour
{
    // Private values
    Animator animator;
    private float nextPunch = 0f;

    [Header("Configurables")]
    public float punchDamage;
    public float punchRange;
    public float punchCharge;

    [Header("Player")]
    public PlayerScript player;
    public GameObject playerGun;
    public Camera AttackPoint;

    [Header("Effects")]
    public AudioSource audioSource;
    public AudioClip[] punchSounds;
    public GameObject zombieHitDust;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerGun.activeSelf == false && Input.GetButtonDown("Fire") && Time.time >= nextPunch)
        {
            animator.SetBool("Punch", true);
            
            nextPunch = Time.time + 1f / punchCharge;
            Punch();
        }
        else
        {
            animator.SetBool("Punch", false);
        }
    }

    public void Punch()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(AttackPoint.transform.position, AttackPoint.transform.forward, out hitInfo, punchRange))
        {
            ZombieScript zombieHit = hitInfo.transform.GetComponent<ZombieScript>();
            GolemControl golemeHit = hitInfo.transform.GetComponent<GolemControl>();

            if (zombieHit != null)
            {
                AudioClip clip = punchSounds[Random.Range(0, punchSounds.Length)];
                audioSource.PlayOneShot(clip);
                zombieHit.ZombieTakeDamage(punchDamage);
                GameObject zombieDust = Instantiate(zombieHitDust, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(zombieDust, 2f);
            }

            if (golemeHit != null)
            {
                AudioClip clip = punchSounds[Random.Range(0, punchSounds.Length)];
                audioSource.PlayOneShot(clip);
                golemeHit.GolemTakeDamage(punchDamage);
                GameObject zombieDust = Instantiate(zombieHitDust, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(zombieDust, 2f);
            }
        }
    }
}
