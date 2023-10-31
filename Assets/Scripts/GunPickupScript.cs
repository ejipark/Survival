using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GunPickupScript : MonoBehaviour
{
    public PlayerScript player;
    public GameObject playerGun;
    public GameObject pickupGun;
    public GameObject playerUI;
    public GameObject questUI;
    public float radius;
    private static bool isFirstTime = false;
    private Scene currentScene;
    private string sceneName;

    private void Awake()
    {
        playerGun.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
    }

    // Update is called once per frame
    void Update()
    {
        // Picking up gun
        if (Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isFirstTime && sceneName == "LevelOne")
                {
                    isFirstTime = true;
                    //pause and show instructions
                    questUI.SetActive(true);                   
                    playerUI.SetActive(false);                  

                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;
                    
                }
                
                playerGun.SetActive(true);
                pickupGun.SetActive(false);
            }
        }


        
    }
}
