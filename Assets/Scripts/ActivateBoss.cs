using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBoss : MonoBehaviour
{
    Rigidbody rb;
    Vector3 resetPos;
    Animator anim;
    GameObject boss;
    GameObject[] collectables;

    // Start is called before the first frame update
    private void Start()
    {
        boss = GameObject.FindWithTag("Boss");
        // Debug.Log("Active Self: " + boss.activeSelf);
        // boss.SetActive(false);
    }

    private void FixedUpdate()
    {
        GameObject[] collectables = GameObject.FindGameObjectsWithTag("PickUp");
        if (collectables.Length == 0)
        {
            // boss.SetActiveRecursively(true);
            boss.SetActive(true);
        }
    }
}
