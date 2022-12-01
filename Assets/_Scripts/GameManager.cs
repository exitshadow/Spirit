using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool IsPoemRunning;
    [SerializeField] private SceneListManagementSO _scenes;
    [SerializeField] private string currentScene;
    private string nextScene;

    private void Start()
    {
        for (int i = 0; i < _scenes.ScenesList.Count; i++)
        {
            if (currentScene == _scenes.ScenesList[i])
            {
                try
                {
                    nextScene = _scenes.ScenesList[i + 1];
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log($"{e.Message}: You have reached the end. Program will now load the title screen.");
                    nextScene = "MENU";
                }

                break;
            }
        }
    }

    public void Next()
    {
        SceneManager.LoadScene(nextScene);
    }
}
