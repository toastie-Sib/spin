using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbine : ItemBase
{
    private int startSegment = -1;
    private int currentSegment = -1;
    private int segmentsPassed = 0;

    private float lastAngle;
    private int rotationDir = 0; // 1 = CW, -1 = CCW

    public override void Start()
    {
        base.Start();
        lastAngle = GetParentAngle();
    }

    void Update()
    {
        float angle = GetParentAngle();
        float delta = Mathf.DeltaAngle(lastAngle, angle);

        // No movement
        if (Mathf.Abs(delta) < 0.01f)
            return;

        int newDir = delta > 0 ? 1 : -1;

        // Direction changed → RESET and start fresh
        if (rotationDir != 0 && newDir != rotationDir)
        {
            ResetTracking();
        }

        rotationDir = newDir;

        int seg = GetSegment(angle);

        // First segment
        if (startSegment == -1)
        {
            startSegment = seg;
            currentSegment = seg;
            segmentsPassed = 0;
        }
        else if (seg != currentSegment)
        {
            int expected = (currentSegment + rotationDir + 4) % 4;

            // Skipped or reversed → reset
            if (seg != expected)
            {
                ResetTracking();
                startSegment = seg;
                currentSegment = seg;
                segmentsPassed = 0;
            }
            else
            {
                currentSegment = seg;
                segmentsPassed++;

                // Completed full loop
                if (currentSegment == startSegment && segmentsPassed >= 4)
                {
                    OnRotationComplete();
                    ResetTracking();
                }
            }
        }

        lastAngle = angle;
    }

    void ResetTracking()
    {
        startSegment = -1;
        currentSegment = -1;
        segmentsPassed = 0;
        rotationDir = 0;
    }


    int GetSegment(float angle)
    {
        if (angle >= 315f || angle < 45f) return 0;     // Up
        if (angle >= 45f && angle < 135f) return 1;     // Right
        if (angle >= 135f && angle < 225f) return 2;    // Down
        return 3;                                       // Left
    }

    float GetParentAngle()
    {
        return NormalizeAngle(transform.parent.eulerAngles.z);
    }

    float NormalizeAngle(float angle)
    {
        return (angle + 360f) % 360f;
    }


    //final effect when rotation is complete
    void OnRotationComplete()
    {
        for (int i = 0; i < stacks; i++)
        {
            GetComponent<Weapon>().TriggerScaling();
        }
    }
}
