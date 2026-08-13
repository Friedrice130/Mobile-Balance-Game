using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("Audio Settings")]
    public SoundData mainMenuBGM;
    public AudioMixer mainMixer;

    void Start()
    {
        ShowMainMenu();

        StartCoroutine(InitializeAudioAndPlayMusic());
    }

    private IEnumerator InitializeAudioAndPlayMusic()
    {
        yield return new WaitForSecondsRealtime(0.1f); 

        if (mainMixer != null)
        {
            float master = PlayerPrefs.GetFloat("SavedMasterVol", 1f);
            float music = PlayerPrefs.GetFloat("SavedMusicVol", 1f);
            float sfx = PlayerPrefs.GetFloat("SavedSFXVol", 1f);

            SetMixerVolume("MasterVol", master);
            SetMixerVolume("MusicVol", music);
            SetMixerVolume("SFXVol", sfx);
            
            Debug.Log("Audio Mixer Volumes Loaded Successfully.");
        }
        else
        {
            Debug.LogWarning("Main Mixer is NOT assigned in the MainMenuManager!");
        }
        
        if (AudioManager.Instance != null && mainMenuBGM != null)
        {
            AudioManager.Instance.PlayMusic(mainMenuBGM);
            Debug.Log("Main Menu BGM Started.");
        }
    }

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        if (mainMixer == null) return;

        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float decibelValue = Mathf.Log10(clampedValue) * 20f;
        mainMixer.SetFloat(parameterName, decibelValue);
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