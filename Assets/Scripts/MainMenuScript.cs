using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject mainScreen;
    public GameObject infoScreen;
    public GameObject playScreen;

    [Header("For play")]
    public string backgroundStory;
    public float wordDelay;
    public Text storyText;
    private string[] words;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayButton()
    {
        // Hide everything else
        mainScreen.SetActive(false);
        infoScreen.SetActive(false);
        playScreen.SetActive(true);    

        audioSource.Play();
        playScreen.GetComponent<Image>().color = Color.black;
        StartCoroutine(DisplayWords());
    }

    public IEnumerator DisplayWords()
    {
        words = backgroundStory.Split(' ');
        foreach (string word in words)
        {
            storyText.text += word + " ";
            yield return new WaitForSeconds(wordDelay);
        }

        yield return new WaitForSeconds(3f);
        storyText.text +=  "\n\n";

        words = "Your game will be starting from ".Split(' ');
        foreach (string word in words)
        {
            storyText.text += word + " ";
            yield return new WaitForSeconds(wordDelay);
        }

        words = "3... 2... 1... NOW! ... Loading...".Split(' ');
        foreach (string word in words)
        {
            storyText.text += word + " ";
            yield return new WaitForSeconds(1.8f);
        }

        SceneManager.LoadScene("LevelOne");
    }

    public void OnInfoButton()
    {
        mainScreen.SetActive(false);
        infoScreen.SetActive(true);
    }

    public void OnQuitButton()
    {
        Application.Quit();

        /***
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        ***/
    }

    public void skipButton() {
        SceneManager.LoadScene("LevelOne");
    }

    public void LoadMenu()
    {
        infoScreen.SetActive(false);
        mainScreen.SetActive(true);
    }
}
