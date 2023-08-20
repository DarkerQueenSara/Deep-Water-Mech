using UnityEngine;

namespace _Scripts.MechaParts
{
    public enum ArmType
    {
        PROJECTILE,
        HITSCAN,
        MELEE
    }
    
    [CreateAssetMenu(menuName = "Mech Parts/Arm")]
    public class Arm : MechPart
    {
        public int damage;
        public ArmType type;
    }
}