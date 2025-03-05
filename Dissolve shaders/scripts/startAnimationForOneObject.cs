using System.Collections;
using UnityEngine;

namespace GDNoob.MyPackage
{
    public class StartAnimationForOneObject : MonoBehaviour
    {
        // SkinnedMeshRenderer instead of regular Renderer
        private SkinnedMeshRenderer targetRenderer;
        private Material[] targetMaterials;

        // Speed settings
        public float dissolveRate = 0.0125f;
        public float refreshRate = 0.025f;

        // Time delay before automatic dissolve
        public float automaticDissolveDelay = 10f;

        void Awake()
        {
            // Assigning the SkinnedMeshRenderer component
            targetRenderer = GetComponent<SkinnedMeshRenderer>();
            if (targetRenderer != null)
            {
                targetMaterials = targetRenderer.materials;
            }
            else
            {
                Debug.LogError("SkinnedMeshRenderer not found on object: " + gameObject.name);
            }

            // Start the automatic dissolve sequence after setting it fully visible
            StartCoroutine(AutomaticAppearAndDissolve());
        }

        // Coroutine to handle automatic dissolve after the object is fully visible
        IEnumerator AutomaticAppearAndDissolve()
        {
            // Directly set the object to be fully visible (without animation)
            SetVisibility(0); // Set to fully visible instantly

            // Wait for 10 seconds before starting the dissolve effect
            yield return new WaitForSeconds(automaticDissolveDelay);

            // Then, start dissolving the object
            yield return StartCoroutine(DissolveEffect(1)); // Fade out after the delay
        }

        // Set the visibility to the target value (0: fully visible, 1: fully dissolved)
        private void SetVisibility(float targetValue)
        {
            if (targetMaterials.Length == 0) return;

            foreach (Material mat in targetMaterials)
            {
                mat.SetFloat("_visble_amount", targetValue); // Set the visibility immediately
            }
        }

        // Coroutine to handle the dissolve effect to fade out material visibility
        IEnumerator DissolveEffect(float targetValue)
        {
            if (targetMaterials.Length == 0) yield break;

            float currentValue = targetMaterials[0].GetFloat("_visble_amount");
            float direction = (targetValue > currentValue) ? 1 : -1;

            while ((direction > 0 && currentValue < targetValue) || (direction < 0 && currentValue > targetValue))
            {
                currentValue += direction * dissolveRate;
                currentValue = Mathf.Clamp(currentValue, 0, 1);

                foreach (Material mat in targetMaterials)
                {
                    mat.SetFloat("_visble_amount", currentValue);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
