using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Camera AttackPoint;
    public UIScript ui;

    // Private values
    private float shootCharge = 15f;
    private float nextShoot = 0f;
    private float reloadTime = 3f;
    private bool setReload = false;
    
    [Header("Configurables")]
    public float shootDamage;
    public float shootRange;
    public int maxAmmo;
    public int currAmmo;
    public int gunMag;

    [Header("Player")]
    public Animator animator;
    public PlayerScript player;
    public Transform hand;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip shootingSound;
    public AudioClip reloadSound;
    public AudioClip noAmmoSound;

    [Header("Effects")]
    public ParticleSystem muzzleSpark;
    public GameObject metalHitSpark;
    public GameObject zombieHitSpark;

    [Header("Effects")]
    public GameObject me;
    private Vector3 oldPos;
    public float shakeTimer;
    public float shakeFrequency;
    public float runShakeIntensity;
    public float walkShakeIntensity;
    public float idleShakeIntensity;

    private void Awake()
    {
        currAmmo = maxAmmo;
        transform.SetParent(hand);
        ui.SetFullAmmo(maxAmmo);
        ui.UpdateMag(gunMag);
    }

    // Start is called before the first frame update
    void Start()
    {
        oldPos = me.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (setReload)
        {
            return;
        }

        if (currAmmo <= 0 && gunMag >= 1)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && gunMag >= 2 && currAmmo != maxAmmo)
        {
            gunMag--;
            ui.UpdateMag(gunMag);
            StartCoroutine(Reload());
        }

        if (Input.GetButton("Fire") && Time.time >= nextShoot)
        {
            nextShoot = Time.time + 1f / shootCharge;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (gunMag == 0)
        {
            audioSource.PlayOneShot(noAmmoSound);
            return;
        }

        currAmmo--;
        if (currAmmo <= 0)
        {
            gunMag--;
            ui.UpdateMag(gunMag);
        }

        // Check player movement
        if (oldPos != me.transform.position)
        {
            if (Input.GetButton("Run")) // Player is running
            {
                CinemachineShake.Instance.ShakeCamera(runShakeIntensity, shakeFrequency, shakeTimer);
            }
            else
            {
                CinemachineShake.Instance.ShakeCamera(walkShakeIntensity, shakeFrequency, shakeTimer);
            }
        } 
        else
        {
            // Player is not moving
            CinemachineShake.Instance.ShakeCamera(idleShakeIntensity, shakeFrequency, shakeTimer);
        }
        oldPos = me.transform.position;

        // Update UI
        UIScript.occurrence.UpdateAmmo(currAmmo);

        muzzleSpark.Play();
        audioSource.PlayOneShot(shootingSound);
        RaycastHit hitInfo;

        if (Physics.Raycast(AttackPoint.transform.position, AttackPoint.transform.forward, out hitInfo, shootRange)) 
        {
            ZombieScript zombieHit = hitInfo.transform.GetComponent<ZombieScript>();
            BossScript bossHit = hitInfo.transform.GetComponent<BossScript>();
            GolemControl golemBossHit = hitInfo.transform.GetComponent<GolemControl>();

            if (zombieHit != null)
            {
                zombieHit.ZombieTakeDamage(shootDamage);
                GameObject zombieSpark = Instantiate(zombieHitSpark, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(zombieSpark, 1f);
            }
            else if (bossHit != null)
            {
                bossHit.BossTakeDamage(shootDamage);
                GameObject zombieSpark = Instantiate(zombieHitSpark, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(zombieSpark, 1f);
            }
            else if (golemBossHit != null)
            {
                golemBossHit.GolemTakeDamage(shootDamage);
                GameObject zombieSpark = Instantiate(zombieHitSpark, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(zombieSpark, 1f);
            }
            else
            {
                GameObject metalSpark = Instantiate(metalHitSpark, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(metalSpark, 1f);
            }
        }
    }

    IEnumerator Reload()
    {
        setReload = true;

        animator.SetBool("Reload", true);
        audioSource.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(reloadTime);
        animator.SetBool("Reload", false);

        currAmmo = maxAmmo;
        ui.UpdateAmmo(currAmmo);
        setReload = false;
    }
}
