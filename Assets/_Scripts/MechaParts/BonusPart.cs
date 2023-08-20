using UnityEngine;

namespace _Scripts.MechaParts
{
    
    public enum BonusType {JETPACK}
    
    [CreateAssetMenu(fileName = "Mech Parts/Bonus Part")]
    public class BonusPart : MechPart
    {
        public BonusType type;
    }
}