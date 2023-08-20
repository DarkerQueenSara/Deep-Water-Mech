using UnityEngine;

namespace _Scripts.MechaParts
{
    public enum TorsoType {DEFAULT}
    
    [CreateAssetMenu(menuName = "Mech Parts/Torso", order = 5)]
    public class Torso : MechPart
    {
        public TorsoType type;
    }
}