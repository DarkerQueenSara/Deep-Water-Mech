using UnityEngine;

namespace _Scripts.MechaParts
{
    
    public enum LegType {NORMAL, REVERSE, TANK}
    
    [CreateAssetMenu(menuName = "Mech Parts/Legs")]
    public class Legs : MechPart
    {
        public float speed;
        public LegType type;
    }
}