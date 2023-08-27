using System.Linq;
using _Scripts.Controller;
using _Scripts.MechaParts.SO;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class MechSwapHUD : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;

        [Header("Dropdowns")] 
        [SerializeField] private TMP_Dropdown headsDropdown;
        [SerializeField] private TMP_Dropdown torsosDropdown;
        [SerializeField] private TMP_Dropdown leftArmsDropdown;
        [SerializeField] private TMP_Dropdown rightArmsDropdown;
        [SerializeField] private TMP_Dropdown legsDropdown;
        [SerializeField] private TMP_Dropdown bonusPartsDropdown;

        [Header("Current Stats Texts")] 
        [SerializeField] private TextMeshProUGUI currentHpText;
        [SerializeField] private TextMeshProUGUI currentWeightText;
        [SerializeField] private TextMeshProUGUI currentHeadEffectText;
        [SerializeField] private TextMeshProUGUI currentLeftArmTypeText;
        [SerializeField] private TextMeshProUGUI currentLeftArmDamageText;
        [SerializeField] private TextMeshProUGUI currentLeftArmCooldownText;
        [SerializeField] private TextMeshProUGUI currentRightArmTypeText;
        [SerializeField] private TextMeshProUGUI currentRightArmDamageText;
        [SerializeField] private TextMeshProUGUI currentRightArmCooldownText;
        [SerializeField] private TextMeshProUGUI currentSpeedText;
        [SerializeField] private TextMeshProUGUI currentJumpText;
        [SerializeField] private TextMeshProUGUI currentBonusEffectText;
        [SerializeField] private TextMeshProUGUI currentBonusExplanationText;

        [Header("New Stats Texts")]
        [SerializeField] private TextMeshProUGUI newHpText;
        [SerializeField] private TextMeshProUGUI newWeightText;
        [SerializeField] private TextMeshProUGUI newHeadEffectText;
        [SerializeField] private TextMeshProUGUI newLeftArmTypeText;
        [SerializeField] private TextMeshProUGUI newLeftArmDamageText;
        [SerializeField] private TextMeshProUGUI newLeftArmCooldownText;
        [SerializeField] private TextMeshProUGUI newRightArmTypeText;
        [SerializeField] private TextMeshProUGUI newRightArmDamageText;
        [SerializeField] private TextMeshProUGUI newRightArmCooldownText;
        [SerializeField] private TextMeshProUGUI newSpeedText;
        [SerializeField] private TextMeshProUGUI newJumpText;
        [SerializeField] private TextMeshProUGUI newBonusEffectText;
        [SerializeField] private TextMeshProUGUI newBonusExplanationText;
        
        [Header("Button")] 
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button closeMenuButton;


        private Head _selectedHead;
        private Torso _selectedTorso;
        private Arm _selectedLeftArm, _selectedRightArm;
        private Legs _selectedLegs;
        private BonusPart _selectedBonusPart;

        private AudioManager _audioManager;

        private void Start()
        {
            confirmButton.onClick.AddListener(Confirm);
            closeMenuButton.onClick.AddListener(Close);
            _audioManager = GetComponent<AudioManager>();
        }
        
        private void OnEnable()
        {
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            //Clear all dropdowns
            headsDropdown.ClearOptions();
            torsosDropdown.ClearOptions();
            leftArmsDropdown.ClearOptions();
            rightArmsDropdown.ClearOptions();
            legsDropdown.ClearOptions();
            bonusPartsDropdown.ClearOptions();
            
            //Gets the list - first option is equipped
            headsDropdown.AddOptions(inventory.GetListHeads());
            torsosDropdown.AddOptions(inventory.GetListTorsos());
            leftArmsDropdown.AddOptions(inventory.GetListLeftArms());
            rightArmsDropdown.AddOptions(inventory.GetListRightArms());
            legsDropdown.AddOptions(inventory.GetListLegs());
            bonusPartsDropdown.AddOptions(inventory.GetListBonusParts());

            _selectedHead = inventory.equippedHead;
            _selectedTorso = inventory.equippedTorso;
            _selectedLeftArm = inventory.equippedLeftArm;
            _selectedRightArm = inventory.equippedRightArm;
            _selectedLegs = inventory.equippedLegs;
            _selectedBonusPart = inventory.equippedBonusPart;
            
            //set the HP text
            currentHpText.text = MechaController.Instance.currentHp + "/" + MechaController.Instance.maxHp + " HP";
            
            //Set the weight text
            int currentWeight = MechaController.Instance.currentWeight;
            int medianWeight = MechaController.Instance.GetMedianWeight();
            currentWeightText.text = currentWeight + "/" + medianWeight + " KG";
            currentWeightText.color = currentWeight <= medianWeight ? Color.white : Color.red;
            
            //Set the head text
            HeadType headType = inventory.equippedHead.type;
            string headTypeText = headType == HeadType.DEFAULT ? "" : headType.ToString();
            currentHeadEffectText.text = "HEAD EFFECT: " + headTypeText;
            
            //Set the left arm text
            currentLeftArmTypeText.text = inventory.equippedLeftArm.type.ToString();
            currentLeftArmDamageText.text = inventory.equippedLeftArm.damage + " DAMAGE";
            currentLeftArmCooldownText.text = inventory.equippedLeftArm.cooldown.ToString("F1") + " SECONDS COOLDOWN";
            
            //Set the right arm text
            currentRightArmTypeText.text = inventory.equippedRightArm.type.ToString();
            currentRightArmDamageText.text = inventory.equippedRightArm.damage + " DAMAGE";
            currentRightArmCooldownText.text = inventory.equippedRightArm.cooldown.ToString("F1") + " SECONDS COOLDOWN";

            //Set the speed text
            currentSpeedText.text = inventory.equippedLegs.speed + " KPH";
            //Set the jump force text
            currentJumpText.text = inventory.equippedLegs.jumpPower + " JUMP POWER";

            if (inventory.equippedBonusPart != null)
            {
                BonusPart part = inventory.equippedBonusPart;
                switch (part)
                {
                    case BoostPart boostPart:
                        currentBonusEffectText.text = "BONUS EFFECT: BOOST";
                        currentBonusExplanationText.text = "ALLOWS THE MECH TO USE A  BOOST\n"
                                                           + "BOOST FORCE: " + boostPart.boostForce + "\n"
                                                           + "JUMP FORCE: " + boostPart.boostJumpForce + "\n"
                                                           + "BOOST CONSUMPTION: " + boostPart.boostConsumption + " U/S\n"
                                                           + "BOOST RECOVERY: " + boostPart.boostRecovery + " U/S";
                        break;
                    case LighteningPart lighteningPart:
                        currentBonusEffectText.text = "BONUS EFFECT: LIGHTENING";
                        currentBonusExplanationText.text =
                            "INCREASES WEIGHT LIMIT BY: " + lighteningPart.weightLimitReduction;
                        break;
                }
            }
            else
            {
                currentBonusEffectText.text = "";
                currentBonusExplanationText.text = "";
            }

            RefreshNewStats();

        }

        private void RefreshNewStats()
        {
            //set the HP text
            float hpLoss = 1.0f * MechaController.Instance.currentHp / MechaController.Instance.maxHp;
            int maxHp = _selectedHead.hp + _selectedTorso.hp + _selectedLeftArm.hp +
                                 _selectedRightArm.hp +
                                  _selectedLeftArm.hp;
            maxHp = _selectedBonusPart != null ? maxHp + _selectedBonusPart.hp : maxHp;

            newHpText.text =  Mathf.RoundToInt(maxHp * hpLoss) + "/" + maxHp + " HP";
            
            //Set the weight text
            int currentWeight = _selectedHead.weight + _selectedTorso.weight + _selectedLeftArm.weight +
                                _selectedRightArm.weight + _selectedLegs.weight;
            currentWeight = _selectedBonusPart != null ? currentWeight + _selectedBonusPart.weight : currentWeight;
            
            int medianWeight = MechaController.Instance.medianWeight;
            if (_selectedBonusPart != null && _selectedBonusPart is LighteningPart lPart)
            {
                medianWeight -= lPart.weightLimitReduction;
            }
            newWeightText.text = currentWeight + "/" + medianWeight + " KG";
            newWeightText.color = currentWeight <= medianWeight ? Color.white : Color.red;
            
            //Set the head text
            HeadType headType = _selectedHead.type;
            string headTypeText = headType == HeadType.DEFAULT ? "" : headType.ToString();
            newHeadEffectText.text = "HEAD EFFECT: " + headTypeText;
            
            //Set the left arm text
            newLeftArmTypeText.text = _selectedLeftArm.type.ToString();
            newLeftArmDamageText.text = _selectedLeftArm.damage + " DAMAGE";;
            newLeftArmCooldownText.text = _selectedLeftArm.cooldown.ToString("F1") + " SECONDS COOLDOWN";
            
            //Set the right arm text
            newRightArmTypeText.text = _selectedRightArm.type.ToString();
            newRightArmDamageText.text = _selectedRightArm.damage + " DAMAGE";;
            newRightArmCooldownText.text = _selectedRightArm.cooldown.ToString("F1") + " SECONDS COOLDOWN";

            //Set the speed text
            newSpeedText.text = _selectedLegs.speed + " KPH";
            //Set the jump force text
            newJumpText.text = _selectedLegs.jumpPower + " JUMP POWER"; 
            
            if (_selectedBonusPart != null)
            {
                BonusPart part = _selectedBonusPart;
                switch (part)
                {
                    case BoostPart boostPart:
                        newBonusEffectText.text = "BONUS EFFECT: BOOST";
                        newBonusExplanationText.text = "ALLOWS THE MECH TO USE A  BOOST\n"
                                                           + "BOOST FORCE: " + boostPart.boostForce + "\n"
                                                           + "JUMP FORCE: " + boostPart.boostJumpForce + "\n"
                                                           + "BOOST CONSUMPTION: " + boostPart.boostConsumption + " U/S\n"
                                                           + "BOOST RECOVERY: " + boostPart.boostRecovery + " U/S";
                        break;
                    case LighteningPart lighteningPart:
                        newBonusEffectText.text = "BONUS EFFECT: LIGHTENING";
                        newBonusExplanationText.text =
                            "INCREASES WEIGHT LIMIT BY: " + lighteningPart.weightLimitReduction;
                        break;
                }
            }
            else
            {
                newBonusEffectText.text = "";
                newBonusExplanationText.text = "";
            }
        }

        public void DropdownSelectHead(int index)
        {
            foreach (Head head in inventory.heads.Where(head => headsDropdown.options[index].text == head.name 
                                                                || headsDropdown.options[index].text == head.name.ToUpper() 
                                                                || headsDropdown.options[index].text == head.name.ToUpper() + " (EQUIPPED)" ))
            {
                _selectedHead = head;
                break;
            }
            RefreshNewStats();
        }
        
        public void DropdownSelectTorso(int index)
        {
            foreach (Torso torso in inventory.torsos.Where(torso => torsosDropdown.options[index].text == torso.name 
                                                                    || torsosDropdown.options[index].text == torso.name.ToUpper() 
                                                                    || torsosDropdown.options[index].text == torso.name.ToUpper() + " (EQUIPPED)" )){
                _selectedTorso = torso;
                break;
            }
            RefreshNewStats();
        }
        
        public void DropdownSelectLeftArm(int index)
        {
            foreach (Arm arm in inventory.arms.Where(arm => leftArmsDropdown.options[index].text == arm.name 
                                                            || leftArmsDropdown.options[index].text == arm.name.ToUpper() 
                                                            || leftArmsDropdown.options[index].text == arm.name.ToUpper() + " (EQUIPPED)" )){
                _selectedLeftArm = arm;
                break;
            }
            RefreshNewStats();
        }
        
        public void DropdownSelectRightArm(int index)
        {
            foreach (Arm arm in inventory.arms.Where(arm => rightArmsDropdown.options[index].text == arm.name 
                                                            || rightArmsDropdown.options[index].text == arm.name.ToUpper() 
                                                            || rightArmsDropdown.options[index].text == arm.name.ToUpper() + " (EQUIPPED)" )){
                _selectedRightArm = arm;
                break;
            }
            RefreshNewStats();
        }
        
        public void DropdownSelectLegs(int index)
        {
            foreach (Legs legs in inventory.legs.Where(legs => legsDropdown.options[index].text == legs.name 
                                                               || legsDropdown.options[index].text == legs.name.ToUpper() 
                                                               || legsDropdown.options[index].text == legs.name.ToUpper() + " (EQUIPPED)" )){
                _selectedLegs = legs;
                break;
            }
            RefreshNewStats();
        }
        
        public void DropdownSelectBonus(int index)
        {
            if (bonusPartsDropdown.options[index].text == "NO EQUIPMENT")
            {
                _selectedBonusPart = null;
                return;
            }
            
            foreach (BonusPart bonus in inventory.bonusParts.Where(bonus => bonusPartsDropdown.options[index].text == bonus.name 
                                                                            || bonusPartsDropdown.options[index].text == bonus.name.ToUpper() 
                                                                            || bonusPartsDropdown.options[index].text == bonus.name.ToUpper() + " (EQUIPPED)" ))
            {
                _selectedBonusPart = bonus;
                break;
            }
            RefreshNewStats();
        }
        

        private void Confirm()
        {
            _audioManager.Play("ConfirmButton");
            inventory.equippedHead = _selectedHead;
            inventory.equippedTorso = _selectedTorso;
            inventory.equippedLeftArm = _selectedLeftArm;
            inventory.equippedRightArm = _selectedRightArm;
            inventory.equippedLegs = _selectedLegs;
            inventory.equippedBonusPart = _selectedBonusPart;
            RefreshInfo();
        }

        private void Close()
        {
            _audioManager.Play("CloseButton");
            MechaController.Instance.UpdateMech();
            PlayerController.Instance.HUDClosed();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("HUD closed");
            gameObject.SetActive(false);
        }
    }
}
