using System;
using _Scripts.Controller;
using _Scripts.MechaParts.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class MechaHUD : MonoBehaviour
    {
        public Image healthBar;
        public Image boostBar;

        public TextMeshProUGUI hpText;
        public TextMeshProUGUI boostText;

        private void Update()
        {
            healthBar.fillAmount = 1.0f * MechaController.Instance.currentHp / MechaController.Instance.maxHp;
            hpText.text = MechaController.Instance.currentHp + "/" + MechaController.Instance.maxHp;

            BonusPart part = MechaController.Instance.BonusPart;
            boostBar.gameObject.SetActive(part != null && part is BoostPart);
            boostText.gameObject.SetActive(part != null && part is BoostPart);

            if (boostBar.IsActive() && boostText.IsActive())
            {
                boostBar.fillAmount = MechaController.Instance.currentBoost / MechaController.Instance.maxBoost;
                boostText.text = Mathf.Round(boostBar.fillAmount * 100 * 10.0f) * 0.1f + "%";
            }

        }
    }
}
