using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    private float speed = 50f; // The speed at which the text scrolls up the screen
    public float scrollDuration; // The amount of time to scroll before stopping

    private RectTransform rectTransform;
    private float elapsedTime = 0f;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < scrollDuration)
        {
            rectTransform.position += new Vector3(0, speed * Time.deltaTime, 0); // Move the text up the screen
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
