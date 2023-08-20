using UnityEngine;

namespace _Scripts.MechaParts
{
    
    public enum LegType {NORMAL, REVERSE, TANK}
    
    [CreateAssetMenu(menuName = "Mech Parts/Legs", order = 4)]
    public class Legs : MechPart
    {
        public float jumpPower;
        public float speed;
        public LegType type;
    }
}