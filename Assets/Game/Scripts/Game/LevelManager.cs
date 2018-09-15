using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//场景管理
public class LevelManager : MonoBehaviour
{
    public LevelPreset[] LevelPresets;
    public GameObject Preloader;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (Preloader != null)
            Preloader.SetActive(false);
    }

    public void OnGameIsBegin()
    {
        if (Preloader != null)
            Preloader.SetActive(true);
    }
    public void OnGameplay()
    {
        if (Preloader != null)
            Preloader.SetActive(false);
    }

    bool isSceneCurrentlyLoaded(string sceneName_no_extention)
    {
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName_no_extention)
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (UnitZ.gameManager.IsPlaying)
        {
            if (Preloader != null)
                Preloader.SetActive(false);
        }
    }
}