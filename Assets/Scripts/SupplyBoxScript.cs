using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyBoxScript : MonoBehaviour
{
    Animator animator;
    private float nextItem = 0f;

    public PlayerScript player;
    public UIScript ui;
    public AudioSource audioSource;
    public AudioClip supplySound;
    public GameTrackerScript gameTracker;
    public float radius;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            if (Input.GetKeyDown(KeyCode.T) && Time.time >= nextItem)
            {
                nextItem = Time.time + 2f;
                animator.SetBool("Open", true);

                gameTracker.supplyCount += 1;
                ui.UpdateSupply(gameTracker.supplyCount);

                audioSource.PlayOneShot(supplySound);
                Object.Destroy(gameObject, 2f);
            }
        }
    }
}
