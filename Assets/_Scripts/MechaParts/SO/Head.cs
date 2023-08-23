using UnityEngine;

namespace _Scripts.MechaParts.SO
{
    
    public enum HeadType { DEFAULT, LANTERN, CROSSHAIR, LOCKON}
    
    [CreateAssetMenu(menuName = "Mech Parts/Legs", order = 2)]
    public class Head : MechPart
    {
        public HeadType type;
    }
}