using UnityEngine;

namespace _Scripts.MechaParts
{
    
    public enum BonusType {JETPACK}
    
    [CreateAssetMenu(menuName = "Mech Parts/Bonus Part", order = 1)]
    public class BonusPart : MechPart
    {
        public BonusType type;
    }
}