using _Scripts.Managers.SO;
using Audio;
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

        private AudioManager _audioManager;

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
            
            _audioManager = GetComponent<AudioManager>();
            _audioManager.Play("MenuMusic");
        }

        private void RetryGame()
        {
            _audioManager.Play("ButtonPress");
            SceneManager.LoadScene(1);
        }

        private void Credits()
        {
            _audioManager.Play("ButtonPress");
            SceneManager.LoadScene(3);
        }
    }
}