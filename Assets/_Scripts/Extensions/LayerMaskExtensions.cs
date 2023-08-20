using UnityEngine;

namespace Extensions
{
    /// <summary>
    /// An extension to easily check for collisions in specific layers
    /// </summary>
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Determines whether the specified mask has a layer.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <param name="layer">The layer.</param>
        /// <returns>
        ///   <c>true</c> if the specified mask has layer; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasLayer(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
    }
}