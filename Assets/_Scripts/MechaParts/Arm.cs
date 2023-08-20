using UnityEngine;

namespace _Scripts.MechaParts
{
    public enum ArmType
    {
        PROJECTILE,
        HITSCAN,
        MELEE
    }
    
    [CreateAssetMenu(fileName = "Mech Parts/Arm")]
    public class Arm : MechPart
    {
        public ArmType type;
    }
}