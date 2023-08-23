using UnityEngine;

namespace _Scripts.MechaParts.SO
{
    [CreateAssetMenu(menuName = "Mech Parts/Bonus Parts/Boost Part", order = 1)]
    public class BoostPart : BonusPart
    {
        public float boostForce;
        public float boostJumpForce;
        [Range(0,1)] public float boostControlLoss;
        public float boostConsumption;
        public float boostRecovery;
    }
}