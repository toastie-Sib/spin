using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSpin : MonoBehaviour
{
    [Header("Spin Settings")]
    public float maxSpinSpeed = 250; // degrees per second at top speed
    public float accelerationTime = 5f; // seconds to reach full speed
    public float waitTime = 1.5f;
    private float currentSpeed = 0f;
    private float elapsed = 0f;
    private bool spin = false;

    void Start()
    {
        StartCoroutine(StartSpin(waitTime));
    }

    public IEnumerator StartSpin(float amount)
    {
        yield return new WaitForSeconds(amount);
        spin = true;
    }

    void Update()
    {
        if (spin == false) return;
        // Increase elapsed time until we hit the target acceleration time
        if (elapsed < accelerationTime)
        {
            elapsed += Time.deltaTime;
        }

        // t goes from 0 → 1 as we accelerate
        float t = Mathf.Clamp01(elapsed / accelerationTime);

        // Use a smooth ease-in curve (slow → fast)
        // You can replace SmoothStep with other easing functions for different feels
        float easedT = t * t * (3f - 2f * t);

        // Interpolate current speed from 0 to maxSpinSpeed
        currentSpeed = Mathf.Lerp(0f, maxSpinSpeed, easedT);

        // Apply rotation on the Z-axis
        transform.Rotate(0f, 0f, currentSpeed * Time.deltaTime);
    }
}
