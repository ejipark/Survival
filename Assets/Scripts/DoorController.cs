using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Private values
    Animator animator;
    Collider doorCollider;

    public GameTrackerScript gameTracker;
    public int miniBossID;

    // Start is called before the first frame update
    void Start()
    {
        doorCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameTracker.miniBossDead == miniBossID)
        {
            doorCollider.enabled = true;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && gameTracker.miniBossDead >= miniBossID)
        {
            animator.SetBool("Open", true);
            Object.Destroy(gameObject, 2f);
        }
    }
}
