using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxScript : MonoBehaviour
{
    Animator animator;
    private GunScript gunScript;
    private float nextItem = 0f;

    public GameObject gun;
    public PlayerScript player;
    public UIScript ui;
    public AudioSource audioSource;
    public AudioClip chargeSound;
    public float radius;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        gunScript = gun.GetComponent<GunScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            if (gun.activeSelf == true)
            {
                if (Input.GetKeyDown(KeyCode.T) && Time.time >= nextItem)
                {
                    nextItem = Time.time + 2f;
                    animator.SetBool("Open", true);

                    int totalMag = gunScript.gunMag + 1;
                    gunScript.gunMag = totalMag;
                    ui.UpdateMag(totalMag);

                    audioSource.PlayOneShot(chargeSound);
                    Object.Destroy(gameObject, 2f);
                }
            }
        }
    }
}
