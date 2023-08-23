using UnityEngine;

namespace _Scripts.MechaParts.SO
{
    
    public enum LegType {NORMAL, REVERSE, SPIDER, TANK}
    
    [CreateAssetMenu(menuName = "Mech Parts/Legs", order = 4)]
    public class Legs : MechPart
    {
        public float jumpPower;
        public float speed;
        public LegType type;
    }
}