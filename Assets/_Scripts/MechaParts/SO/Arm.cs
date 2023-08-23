using UnityEngine;

namespace _Scripts.MechaParts.SO
{
    public enum ArmType
    {
        PROJECTILE,
        HITSCAN,
        MELEE
    }
    
    [CreateAssetMenu(menuName = "Mech Parts/Arm", order = 0)]
    public class Arm : MechPart
    {
        public int damage;
        public float cooldown;
        public ArmType type;
    }
}