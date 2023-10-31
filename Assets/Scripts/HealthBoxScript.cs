using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBoxScript : MonoBehaviour
{
    Animator animator;
    private float nextItem = 0f;

    public PlayerScript player;
    public UIScript ui;
    public AudioSource audioSource;
    public AudioClip recoverSound;
    public float giveHealth;
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

                float totalHealth = player.currHealth + giveHealth;
                if (totalHealth > player.playerHealth)
                {
                    player.currHealth = player.playerHealth;
                    ui.UpdateHealth(player.playerHealth);
                }
                else
                {
                    player.currHealth = totalHealth;
                    ui.UpdateHealth(totalHealth);
                }

                audioSource.PlayOneShot(recoverSound);
                Object.Destroy(gameObject, 2f);
            }
        }
    }
}
