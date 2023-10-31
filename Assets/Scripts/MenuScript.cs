using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject ui;
    public GameObject crosshair;
    public GameObject playerGun;
    public GameObject controls;

    // Private variables
    public static bool gameStopped = false;

    [Header("MenuCanvas")]
    public GameObject pauseMenu;
    public GameObject endMenu;
    public GameObject nextMenu;

    [Header("QuestUI")]
    public GameObject questUI;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        endMenu.SetActive(false);
        controls.SetActive(true);
        ui.SetActive(true);
        crosshair.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && endMenu.activeSelf == false && nextMenu.activeSelf == false)
        {
            if (gameStopped)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            if (controls.activeSelf == true)
            {
                removeInst();
            }
            else
            {
                showInst();
            }
        }
        else if (Input.GetKeyDown(KeyCode.B) && endMenu.activeSelf == false && nextMenu.activeSelf == false && questUI.activeSelf == true)
        {
            questUI.SetActive(false);
            ui.SetActive(true);
            if (playerGun.activeSelf == true)
            {
                crosshair.SetActive(true);
            }
            else
            {
                crosshair.SetActive(false);
            }
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void showInst()
    {
        controls.SetActive(true);
    }

    public void removeInst()
    {
        controls.SetActive(false);
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        controls.SetActive(false);
        ui.SetActive(false);
        crosshair.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        gameStopped = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        ui.SetActive(true);
        if (playerGun.activeSelf == true)
        {
            crosshair.SetActive(true);
        }
        else
        {
            crosshair.SetActive(false);
        }
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        gameStopped = false;
    }

    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoNextLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex + 1);
    }

    public void GameOver()
    {
        endMenu.SetActive(true);
        controls.SetActive(false);
        ui.SetActive(false);
        crosshair.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    public void NextLevel()
    {
        nextMenu.SetActive(true);
        controls.SetActive(false);
        ui.SetActive(false);
        crosshair.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }
}
