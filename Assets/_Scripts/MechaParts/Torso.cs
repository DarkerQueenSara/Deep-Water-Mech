using UnityEngine;

namespace _Scripts.MechaParts
{
    public enum TorsoType {DEFAULT}
    
    [CreateAssetMenu(fileName = "Mech Parts/Torso")]
    public class Torso : MechPart
    {
        public TorsoType type;
    }
}