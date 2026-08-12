using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject mainMenuPanel;
    public GameObject chapterSelectPanel;

    [Header("Scene References")]
    public int defaultPlayBuildIndex = 1;

    public int chapter1BuildIndex = 1;
    public int chapter2BuildIndex = 1;
    public int chapter3BuildIndex = 1;
    public int chapter4BuildIndex = 1;

    void Start()
    {
        ShowMainMenu();
    }

    // Main Menu 
    public void PlayGame()
    {
        // LoadSceneByIndex(defaultPlayBuildIndex, "Play");
        LoadSceneWithTransition(defaultPlayBuildIndex, "CHAPTER 1");
    }

    public void OpenChapterSelect()
    {
        mainMenuPanel.SetActive(false);
        chapterSelectPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        chapterSelectPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Application");
        Application.Quit();
    }


    // Chapter Select Panel
    public void LoadChapter1()
    {
        // LoadSceneByIndex(chapter1BuildIndex, "Chapter 1");
        LoadSceneWithTransition(chapter1BuildIndex, "CHAPTER 1");
    }

    public void LoadChapter2()
    {
        // LoadSceneByIndex(chapter2BuildIndex, "Chapter 2");
        LoadSceneWithTransition(chapter2BuildIndex, "CHAPTER 2");
    }

    public void LoadChapter3()
    {
        // LoadSceneByIndex(chapter3BuildIndex, "Chapter 3");
        LoadSceneWithTransition(chapter3BuildIndex, "CHAPTER 3");
    }

    public void LoadChapter4()
    {
        // LoadSceneByIndex(chapter4BuildIndex, "Chapter 4");
        LoadSceneWithTransition(chapter4BuildIndex, "CHAPTER 4");
    }

    // Load Scene
    // private void LoadSceneByIndex(int index, string sceneLabel)
    // {
    //     if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
    //     {
    //         SceneManager.LoadScene(index);
    //     }
    //     else
    //     {
    //         Debug.LogError($"Couldn't load {sceneLabel}! Check Build Settings.");
    //     }
    // }

    private void LoadSceneWithTransition(int index, string chapterName)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
        {
            if (ScreenFader.Instance != null)
            {
                ScreenFader.Instance.LoadChapterWithFade(index, chapterName);
            }
            else
            {
                SceneManager.LoadScene(index); 
            }
        }
        else
        {
            Debug.LogError($"Couldn't load {chapterName}! Check Build Settings.");
        }
    }
}