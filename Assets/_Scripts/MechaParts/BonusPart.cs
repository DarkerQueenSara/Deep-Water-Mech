using UnityEngine;

namespace _Scripts.MechaParts
{
    
    public enum BonusType {JETPACK}
    
    [CreateAssetMenu(menuName = "Mech Parts/Bonus Part")]
    public class BonusPart : MechPart
    {
        public BonusType type;
    }
}