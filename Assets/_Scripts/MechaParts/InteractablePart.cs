using UnityEngine;

namespace _Scripts.MechaParts
{
    public class InteractablePart : MonoBehaviour
    {
        [SerializeField] private GameObject selectedGO;

        public void SetSelected(bool active)
        {
            selectedGO.SetActive(active);
        }
    }
}