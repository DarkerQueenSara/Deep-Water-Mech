using System.Collections.Generic;
using _Scripts.MechaParts.SO;
using UnityEngine;

namespace _Scripts.Managers
{
    public class PartsManager : MonoBehaviour
    {
        public static PartsManager Instance { get; private set; }

        [SerializeField] private List<MechPart> parts;
        [SerializeField] private Inventory inventory;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one Parts! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public MechPart GetRandomPart()
        {
            int c = 0;

            while (c < 1000)
            {
                MechPart randomPart = parts[Random.Range(0, parts.Count)];
                if (!inventory.Contains(randomPart)) return randomPart;
                c++;
            }

            return null;
        }
        
    }
}