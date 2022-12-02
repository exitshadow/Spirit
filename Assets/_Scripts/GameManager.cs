using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SceneListManagementSO _scenes;
    private string currentScene;
    private string nextScene;
    public bool IsPoemRunning;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;

        for (int i = 0; i < _scenes.ScenesList.Count; i++)
        {
            if (currentScene == _scenes.ScenesList[i])
            {
                try
                {
                    nextScene = _scenes.ScenesList[i + 1];
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.Log($"{e.Message} - All these words means you have reached the end. Program will load the title screen.");
                    nextScene = "MENU";
                }

                break;
            }
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
