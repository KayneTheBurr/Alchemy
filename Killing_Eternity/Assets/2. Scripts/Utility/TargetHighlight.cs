using System.Collections.Generic;
using UnityEngine;
namespace KE
{
    public class TargetHighlight : MonoBehaviour
    {
        [Header("Renderers")]
        [Tooltip("Assign the SkinnedMeshRenderers for this enemy")]
        public Renderer[] enemyRenderers;

        [Header("Highlight Material")]
        public Material highlightMaterial;

        private bool isHighlighted = false;

        public void SetHighlight(bool active)
        {
            if (isHighlighted == active) return;
            isHighlighted = active;

            foreach (Renderer rend in enemyRenderers)
            {
                if (rend == null) continue;

                List<Material> mats = new List<Material>(rend.sharedMaterials);

                if (active)
                {
                    // Add the overlay material to the end of the array
                    mats.Add(highlightMaterial);
                }
                else
                {
                    // Remove the overlay material
                    mats.Remove(highlightMaterial);
                }

                rend.sharedMaterials = mats.ToArray();
            }
        }
    }
}