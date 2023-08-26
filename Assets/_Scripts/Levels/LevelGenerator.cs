using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Levels
{
    public class LevelGenerator : MonoBehaviour
    {
        public static LevelGenerator Instance { get; private set; }

        [SerializeField] private int timeLimit = 30 * 60;
        [HideInInspector] public float timeLeft;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one LevelGenerator! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        [SerializeField] private int maxLevel;
        [SerializeField] private int heightInterval;

        public GameObject platformPrefab;
        
        [HideInInspector] public int currentLevel = 1;

        [HideInInspector] public int currentHeight;

        private void Start()
        {
            timeLeft = timeLimit;
        }

        private void Update()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
               //TODO end game
            }
        }
        
        [ContextMenu("Generate New Level")]
        public void GenerateNewLevel()
        {
            currentLevel++;
            if (currentLevel > maxLevel)
            {
                EndGame();
                return;
            }
            currentHeight -= heightInterval;
            Instantiate(platformPrefab, new Vector3(0, currentHeight, 0), Quaternion.identity);
        }

        private static void EndGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
