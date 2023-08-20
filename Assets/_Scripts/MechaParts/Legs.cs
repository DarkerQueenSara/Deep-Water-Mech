using UnityEngine;

namespace _Scripts.MechaParts
{
    
    public enum LegType {NORMAL, REVERSE, TANK}
    
    [CreateAssetMenu(fileName = "Mech Parts/Legs")]
    public class Legs : MechPart
    {
        public LegType type;
    }
}