using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public AudioSource music1, music2, levelComplete;
    private Image transitionScreen;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        transitionScreen = gameObject.GetComponentInChildren<Image>(true);
    }

    // Update is called once per frame
    void Update()
    {
        // R to reset
        if (SceneManager.GetActiveScene().name != "Title Screen" && Input.GetKeyDown(KeyCode.R))
            RestartLevel();

        // N to skip
        if (SceneManager.GetActiveScene().name != "Title Screen" && Input.GetKeyDown(KeyCode.N))
            LoadNextLevel();
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartLevelCoroutine());
    }

    IEnumerator RestartLevelCoroutine()
    {
        // Fade to black
        Time.timeScale = 0.0f;
        transitionScreen.gameObject.SetActive(true);
        while (transitionScreen.color.a < 1)
        {
            transitionScreen.color =
                new Color(
                    transitionScreen.color.r,
                    transitionScreen.color.g,
                    transitionScreen.color.b,
                    transitionScreen.color.a + 0.02f);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Fade from black
        while (transitionScreen.color.a > 0)
        {
            transitionScreen.color =
                new Color(
                    transitionScreen.color.r,
                    transitionScreen.color.g,
                    transitionScreen.color.b,
                    transitionScreen.color.a - 0.02f);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        transitionScreen.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelCoroutine());
    }

    IEnumerator LoadNextLevelCoroutine()
    {
        // Fade to black
        Time.timeScale = 0.0f;
        transitionScreen.gameObject.SetActive(true);
        while (transitionScreen.color.a < 1)
        {
            transitionScreen.color =
                new Color(
                    transitionScreen.color.r,
                    transitionScreen.color.g,
                    transitionScreen.color.b,
                    transitionScreen.color.a + 0.02f);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        switch (SceneManager.GetActiveScene().name)
        {
            case "Title Screen": 
                SceneManager.LoadScene("Level 01"); 
                break;
            case "Level 01":
                levelComplete.Play();
                SceneManager.LoadScene("Level 02");
                break;
            case "Level 02":
                levelComplete.Play();
                SceneManager.LoadScene("Level 03");
                music1.enabled = false;
                music2.enabled = true;
                break;
            case "Level 03":
                levelComplete.Play();
                SceneManager.LoadScene("Credits");
                break;
        }

        // Fade from black
        while (transitionScreen.color.a > 0)
        {
            transitionScreen.color =
                new Color(
                    transitionScreen.color.r,
                    transitionScreen.color.g,
                    transitionScreen.color.b,
                    transitionScreen.color.a - 0.02f);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        transitionScreen.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
