using UnityEngine;

namespace _Scripts.Combat
{
    public class Hittable : MonoBehaviour
    {
        
        /// <summary>
        /// How many hits the object can take before dying
        /// </summary>
        public int maxHits = 5;

        public bool canBecomeInvincible;
        /// <summary>
        /// The hits left
        /// </summary>
        public int hitsLeft = 5;
        /// <summary>
        /// The number of invincibility frames
        /// </summary>
        public int invincibilityFrames;
        /// <summary>
        /// If the object is currently invincible
        /// </summary>
        [HideInInspector] public bool invincible;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public virtual void Start()
        {
            hitsLeft = maxHits;
        }
        
        /// <summary>
        /// Deals the damage.
        /// </summary>
        public virtual void DoDamage(int damage)
        {
            if (invincible) return;
            //Else deal damage
            hitsLeft = Mathf.Clamp(hitsLeft - damage, 0, maxHits);
            if (hitsLeft > 0)
            {
                if (canBecomeInvincible) invincible = true;
                Invoke(nameof(RestoreVulnerability), invincibilityFrames / 60.0f);
                //Debug.Log(transform.name + " got hit");
            }
            else
            {
                //Debug.Log(transform.name + " died");
                Die();
            }
        }

        /// <summary>
        /// Restores the vulnerability.
        /// </summary>
        protected virtual void RestoreVulnerability()
        {
            if (canBecomeInvincible) invincible = false;
        }

        /// <summary>
        /// Kill the player
        /// </summary>
        protected virtual void Die()
        {
            Destroy(gameObject);
        }
    }
}