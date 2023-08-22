using _Scripts.Combat;
using UnityEngine;

namespace _Scripts.MechaParts
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
        public bool isLeftArm;
        public int damage;
        public ArmType type;
    }
}