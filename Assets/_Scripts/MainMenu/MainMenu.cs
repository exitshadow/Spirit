using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneListManagementSO _scenesList;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _moveSound;
    [SerializeField] private AudioClip _launchSound;

    public void LoadEntryScene()
    {
        _audioSource.clip = _launchSound;
        _audioSource.Play();
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
