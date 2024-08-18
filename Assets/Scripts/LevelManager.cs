using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadLevel(int levelNo)
    {
        SceneManager.LoadScene(levelNo);
    }

    public void FirstLevel()
    {
        int levelNo = 0;
        LoadLevel(levelNo);
    }

    public void LastLevel()
    {
        int levelNo = SceneManager.sceneCountInBuildSettings - 1;
        LoadLevel(levelNo);
    }

    public void NextLevel()
    {
        int levelNo = SceneManager.GetActiveScene().buildIndex + 1;
        if (levelNo > SceneManager.sceneCountInBuildSettings - 1)
        {
            levelNo = SceneManager.sceneCountInBuildSettings - 1;
        }
        LoadLevel(levelNo);
    }

    public void PreviousLevel()
    {
        int levelNo = SceneManager.GetActiveScene().buildIndex - 1;
        if (levelNo < 0)
        {
            levelNo = 0;
        }
        LoadLevel(levelNo);
    }

}
