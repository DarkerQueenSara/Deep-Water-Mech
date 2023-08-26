using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsScreenManager : MonoBehaviour
{
    // Start is called before the first frame update
    /// <summary>
    /// The start game button
    /// </summary>
    [Header("Buttons")] public Button backButton;

    /// <summary>
    /// The tutorial button
    /// </summary>
    public Button exitButton;

    private AudioManager _audioManager;

    private void Start()
    {
        backButton.onClick.AddListener(BackToTitle);
        exitButton.onClick.AddListener(ExitGame);
        _audioManager = GetComponent<AudioManager>();
        _audioManager.Play("MenuMusic");
    }

    private void BackToTitle()
    {
        _audioManager.Play("ButtonPress");
        SceneManager.LoadScene(0);
    }

    private void ExitGame()
    {
        _audioManager.Play("ButtonPress");
        Application.Quit();
    }
}
