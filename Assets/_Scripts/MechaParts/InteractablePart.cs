using System;
using _Scripts.Counters;
using _Scripts.MechaParts.SO;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.MechaParts
{
    public class InteractablePart : MonoBehaviour, IHasProgress
    {
        public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
        public MechPart mechPart;
        public bool isDrop;
        public int currentHp;
        [SerializeField] private GameObject selectedGO;
        [SerializeField] private Inventory inventory;
        [SerializeField] private float repairRate;

        private void Awake()
        {
            currentHp = mechPart.hp;
        }

        public void SetSelected(bool active)
        {
            if (selectedGO != null) selectedGO.SetActive(active);
        }

        public void DamagePart(int damage)
        {
            currentHp -= damage;
        }

        public void RepairPart()
        {
            if (currentHp >= mechPart.hp)
            {
                currentHp = mechPart.hp;
                return;
            }
            currentHp += (int) (repairRate * Time.deltaTime);
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                ProgressNormalized = (float) currentHp / mechPart.hp,
                partName = mechPart.partName
            });
        }
    }
}