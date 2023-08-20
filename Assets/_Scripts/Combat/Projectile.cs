using System;
using Extensions;
using UnityEngine;

namespace _Scripts.Combat
{
    public class Projectile : MonoBehaviour
    {
        /// <summary>
        /// The player layer
        /// </summary>
        public LayerMask hittables;
        /// <summary>
        /// The walls layer
        /// </summary>
        public LayerMask wallsLayer;

        public float bulletLifetime = 5f;
        private float _timePassed;
        
        /// <summary>
        /// The bullet damage
        /// </summary>
        public int projectileDamage = 1;
        /// <summary>
        /// The bullet speed
        /// </summary>
        public float projectileSpeed = 20.0f;

        /// <summary>
        /// The rigidbody
        /// </summary>
        public Rigidbody body;

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            //body.AddForce(transform.forward * -1 * projectileSpeed, ForceMode.Impulse);
        }

        private void Update()
        {
            _timePassed += Time.deltaTime;
            if (_timePassed >= bulletLifetime)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Called when [trigger enter].
        /// </summary>
        /// <param name="col">The col.</param>
        private void OnTriggerEnter(Collider col)
        {
            if (hittables.HasLayer(col.gameObject.layer))
            {
                
            }
            if (wallsLayer.HasLayer(col.gameObject.layer))
            {
                Destroy(gameObject);
            }
        }
    }
}