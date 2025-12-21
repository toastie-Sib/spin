using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbine : ItemBase
{
    private float previousAngle;

    private bool passedUp = false;
    private bool passedDown = false;

    public override void Start()
    {
        base.Start();
        if (transform.parent == null)
        {
            Debug.LogError("RotationPassDetector requires a parent object.");
            enabled = false;
            return;
        }

        previousAngle = GetParentAngle();
    }

    void Update()
    {
        float currentAngle = GetParentAngle();

        // Check UP (90°)
        if (!passedUp && PassedAngle(previousAngle, currentAngle, 90f))
        {
            passedUp = true;
            Debug.Log("Passed facing UP");
        }

        // Check DOWN (270°)
        if (!passedDown && PassedAngle(previousAngle, currentAngle, 270f))
        {
            passedDown = true;
            Debug.Log("Passed facing DOWN");
        }

        // When both have been passed
        if (passedUp && passedDown)
        {
            Debug.Log("Passed BOTH up and down!");
            passedUp = false;
            passedDown = false;
        }

        previousAngle = currentAngle;
    }

    float GetParentAngle()
    {
        return NormalizeAngle(transform.parent.eulerAngles.z);
    }

    float NormalizeAngle(float angle)
    {
        return (angle + 360f) % 360f;
    }

    bool PassedAngle(float from, float to, float target)
    {
        // Normal case
        if (from < to)
            return from < target && target <= to;

        // Wrapped case (e.g. 350 → 10)
        return from < target || target <= to;
    }
}
