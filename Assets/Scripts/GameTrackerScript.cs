using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTrackerScript : MonoBehaviour
{
    // Private values
    Animator animator;
    AudioSource audioSource;
    Collider doorCollider;
    private Light nextStageLight;
    private Scene currentScene;
    private string sceneName;
    private bool isDown;

    public MenuScript menu;
    public int supplyCount;
    public int maxSupply;
    public GameObject stageLight;
    public GameObject questUI;
    public GameObject gunQuest;
    public GameObject bossQuest;
    public GameObject playerUI;
    public AudioClip doorSound;
    public GameObject protector;

    [Header("Boss tracker")]
    public int miniBossDead;
    private static bool isQuestShown = false;


    // Start is called before the first frame update
    void Start()
    {
        doorCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        nextStageLight = stageLight.GetComponent<Light>();

        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        isDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (supplyCount >= maxSupply)
        {
            nextStageLight.color = Color.green;
            if (!isDown)
            {
                doorCollider.enabled = true;
                if (!isQuestShown && sceneName != "LevelThree")
                {
                    isQuestShown = true;
                    //pause and show instructions
                    questUI.SetActive(true);
                    gunQuest.SetActive(false);
                    bossQuest.SetActive(true);
                    playerUI.SetActive(false);

                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;

                }
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && supplyCount >= maxSupply)
        {
            audioSource.PlayOneShot(doorSound);
            if (sceneName != "LevelThree")
            {
                StartCoroutine(ShowMenu());
            } 
            else
            {
                animator.SetBool("Down", true);
                doorCollider.enabled = false;
                isDown = true;
                protector.SetActive(false);
            }
        }
    }

    IEnumerator ShowMenu()
    {
        animator.SetBool("Next", true);
        yield return new WaitForSeconds(1f);
        menu.NextLevel();
    }
}
