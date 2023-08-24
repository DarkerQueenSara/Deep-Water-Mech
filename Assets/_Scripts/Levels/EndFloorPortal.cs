using Extensions;
using UnityEngine;

namespace _Scripts.Levels
{
    public class EndFloorPortal : MonoBehaviour
    {

        [SerializeField] private LayerMask playerMask;
        [SerializeField] private float timeToTeleport;

        private float _timeElapsed;
        private bool _inPortal;

        private void Update()
        {
            if (_inPortal) _timeElapsed += Time.deltaTime;
            else _timeElapsed = 0.0f;

            if (_timeElapsed >= timeToTeleport) ChangeLevel();
        }

        private void ChangeLevel()
        {
            LevelGenerator.Instance.GenerateNewLevel();
            Destroy(transform.parent.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (playerMask.HasLayer(other.gameObject.layer)) _inPortal = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (playerMask.HasLayer(other.gameObject.layer)) _inPortal = false;
        }
    }
}
