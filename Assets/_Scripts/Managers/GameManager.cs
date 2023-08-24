using System;
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
        [SerializeField] private GameObject playerMesh;
        [SerializeField] private Transform exitMechaPos;
        [SerializeField] private MechaController mecha;
        [SerializeField] private GameObject mechaCam;
        [SerializeField] public Inventory inventory;

        private CharacterController _playerController;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one GameManager! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            IsInsideMecha = true;
            inventory.InitiateInventory();
        }

        private void Start()
        {
            _playerController = player.GetComponent<CharacterController>();
        }

        public void EnterMecha()
        {
            IsInsideMecha = true;
            mechaCam.SetActive(true);
            player.gameObject.SetActive(false);
            playerMesh.SetActive(false);
            playerCam.SetActive(false);
        }

        public void ExitMecha()
        {
            IsInsideMecha = false;
            player.gameObject.SetActive(true);
            _playerController.enabled = false;
            player.transform.position = exitMechaPos.position;
            _playerController.enabled = true;
            playerMesh.SetActive(true);
            playerCam.SetActive(true);
            mechaCam.SetActive(false);
        }
    }
}