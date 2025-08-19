using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Fighter
{
    [Header("Axe")]
    public float refreshInterval = 5f;         // Fire every second
    private float nextRefreshTime = 0.5f;

    public float returnDuration = 1.0f;
    public AnimationCurve returnSpeedCurve;
    private Quaternion targetRot;

    public override void Update() //Make sure to update with Fighter
    {
        base.Update();

        //Timers for Refresh
        if (Time.time >= nextRefreshTime && rb.useGravity == true) //Arrow Refresh
        {
            if (direction == 0) return; // paused
            StartCoroutine(Spin());
            nextRefreshTime = Time.time + refreshInterval;
        }
        
    }

    public override void Start() //Make sure to update with Fighter
    {
        base.Start();

        targetRot = Quaternion.identity; ;
    }

    private IEnumerator Spin()
    {
        spinMult = 1200;
        yield return new WaitForSeconds(0.30f);
        spinMult = 0;
        float timer = 0f;

        Quaternion startRot = transform.rotation;

        while (timer < returnDuration)
        {
            float t = timer / returnDuration;

            // Apply easing using an AnimationCurve if provided, otherwise use a default ease
            //if (returnSpeedCurve != null)
            //{
            //    t = returnSpeedCurve.Evaluate(t);
            //}
            //else
            //{
                // You can use Mathf.SmoothStep or another easing function here
                t = Mathf.SmoothStep(0.5f, 1.0f, t);
            //}

            // Interpolate from the rotation at the end of the spin to the target rotation (e.g., identity)
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            timer += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        transform.rotation = targetRot;
    }
}
