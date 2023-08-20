using UnityEngine;

namespace _Scripts.MechaParts
{
    public enum HeadType {DEFAULT}
    
    [CreateAssetMenu(fileName = "Mech Parts/Head")]
    public class Head : MechPart
    {
        public HeadType type;
    }
}