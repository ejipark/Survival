using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDropScript : MonoBehaviour
{
    public PlayerScript player;
    public GameObject playerGun;
    public GameObject pickupGun;
    GunScript gun;

    // Start is called before the first frame update
    void Start()
    {
        gun = GetComponent<GunScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Dropping gun
        if (gun.animator.GetBool("Reload") == false)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                pickupGun.SetActive(true);
                playerGun.SetActive(false);

                pickupGun.transform.position = player.transform.position;
            }
        }
    }
}
