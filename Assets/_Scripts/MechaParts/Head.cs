using UnityEngine;

namespace _Scripts.MechaParts
{
    public enum HeadType {DEFAULT}
    
    [CreateAssetMenu(menuName = "Mech Parts/Head")]
    public class Head : MechPart
    {
        public HeadType type;
    }
}