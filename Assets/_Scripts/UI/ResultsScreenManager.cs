using System;
using _Scripts.Managers.SO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class ResultsScreenManager : MonoBehaviour
    {
        [SerializeField] private ResultsHolder resultsHolder;
        
        public Button retryButton;
        public Button creditsButton;

        public TextMeshProUGUI resultsText;

        private void Start()
        {
            retryButton.onClick.AddListener(RetryGame);
            creditsButton.onClick.AddListener(Credits);
            
            float timer =resultsHolder.timeLeft;
            int minutes = Mathf.FloorToInt(timer / 60F);
            int seconds = Mathf.FloorToInt(timer - minutes * 60);
            string success = resultsHolder.won ? "YOU WON!" : "YOU LOST...";
            resultsText.text = success + "\n"
                + "YOU REACHED LEVEL " + resultsHolder.levelReached + "\n"
                + "YOU HAD " + $"{minutes:0}:{seconds:00}" + " LEFT";
        }

        private static void RetryGame()
        {
            SceneManager.LoadScene(1);
        }

        private static void Credits()
        {
            SceneManager.LoadScene(3);
        }
    }
}