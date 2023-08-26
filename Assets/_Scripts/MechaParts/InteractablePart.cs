using System;
using System.Collections;
using _Scripts.Controller;
using _Scripts.Counters;
using _Scripts.Managers;
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
        [SerializeField] private float repairWait = 1;

        private Coroutine _repairCoroutine;
        private bool _flag = true;

        private void Awake()
        {
            currentHp = mechPart.hp;
        }

        private void Start()
        {
            GameManager.Instance.ExitedMecha += OnExitedMecha;
        }

        private void OnExitedMecha(object sender, EventArgs e)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                ProgressNormalized = (float)currentHp / mechPart.hp,
            });
        }

        public void SetSelected(bool active)
        {
            if (selectedGO != null) selectedGO.SetActive(active);
        }

        public void StartRepair()
        {
            _repairCoroutine ??= StartCoroutine(RepairOverTime());
        }

        public void StopRepair()
        {
            if (_repairCoroutine == null) return;
            StopCoroutine(_repairCoroutine);
            _repairCoroutine = null;
        }

        private IEnumerator RepairOverTime()
        {
            while (true)
            {
                RepairPart();
                yield return new WaitForSeconds(repairWait);
            }
        }

        public void RepairPart()
        {
            if (currentHp >= mechPart.hp)
            {
                currentHp = mechPart.hp;
                return;
            }

            currentHp += Mathf.CeilToInt(repairRate);
            currentHp = Mathf.Min(currentHp, mechPart.hp);
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                ProgressNormalized = (float)currentHp / mechPart.hp,
            });
            MechaController.Instance.CalculateCurrentHealth();
        }
    }
}