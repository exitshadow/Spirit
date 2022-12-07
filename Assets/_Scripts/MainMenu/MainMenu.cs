using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneListManagementSO _scenesList;

    public void LoadEntryScene()
    {
        SceneManager.LoadScene(_scenesList.ScenesList[0]);
    }

    public void QuitGame()
    {
        #if UNITY_STANDALONE
        Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
