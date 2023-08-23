using _Scripts.Controller;
using UnityEngine;

namespace _Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public bool IsInsideMecha { get; set; }

        [SerializeField] private PlayerController player;
        [SerializeField] private GameObject playerCam;
        [SerializeField] private MechaController mecha;
        [SerializeField] private GameObject mechaCam;
        [SerializeField] public Inventory inventory;

        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one GameManager! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            inventory.InitiateInventory();
        }
        

        public void EnterMecha()
        {
            IsInsideMecha = true;
            mechaCam.SetActive(true);
            player.gameObject.SetActive(false);
            playerCam.SetActive(false);
        }

        public void ExitMecha()
        {
            IsInsideMecha = false;
            player.gameObject.SetActive(true);
            playerCam.SetActive(true);
            mechaCam.SetActive(false);
        }
    }
}