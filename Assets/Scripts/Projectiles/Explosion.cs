using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float scaleDuration = 5.0f; // Total time for scaling from 1 to 100
    public float fadeStartPercentage = 0.75f; // When the fade starts (75% of scale duration)

    private Renderer objectRenderer;
    private Color originalColor;
    private Vector3 originalScale;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        // Store the original color and scale
        originalColor = objectRenderer.material.color;
        originalScale = transform.localScale; // This assumes initial scale is 1,1,1

        StartCoroutine(ScaleAndFadeOutCoroutine());
    }

    private IEnumerator ScaleAndFadeOutCoroutine()
    {
        float timer = 0.0f;

        while (timer < scaleDuration)
        {
            // Calculate progress for scaling (0 to 1)
            float scaleProgress = timer / scaleDuration;

            // Scale the object from original scale to 100 times original scale
            // You can also use Mathf.Lerp(1, 100, scaleProgress) for the multiplier
            transform.localScale = originalScale * Mathf.Lerp(1.0f, 100.0f, scaleProgress);

            // Check if it's time to start fading
            if (scaleProgress >= fadeStartPercentage)
            {
                // Calculate fade progress (0 to 1, where 0 is opaque, 1 is transparent)
                // This remaps the scaleProgress from [fadeStartPercentage, 1] to [0, 1] for fading
                float fadeProgress = Mathf.InverseLerp(fadeStartPercentage, 1.0f, scaleProgress);

                // Set the material's color with the adjusted alpha
                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(1.0f, 0.0f, fadeProgress); // Fade alpha from 1 to 0
                objectRenderer.material.color = newColor;
            }

            timer += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure final state is correct (fully scaled, fully transparent)
        transform.localScale = originalScale * 100.0f;
        Color finalColor = originalColor;
        finalColor.a = 0.0f;
        objectRenderer.material.color = finalColor;

        Destroy(gameObject);
    }
}
