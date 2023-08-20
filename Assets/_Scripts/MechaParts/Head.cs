using UnityEngine;

namespace _Scripts.MechaParts
{
    public enum HeadType {DEFAULT}
    
    [CreateAssetMenu(menuName = "Mech Parts/Head", order = 3)]
    public class Head : MechPart
    {
        public HeadType type;
    }
}