using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : Bow
{
    [Header("Staff")]
    public float fireBoostStrength = 5f;

    public override void FireProjectile()
    {
        base.FireProjectile();

        float zRot = transform.eulerAngles.z;

        // Convert to radians for Mathf trig functions
        float radians = zRot * Mathf.Deg2Rad;

        // Calculate direction based on rotation
        // This makes 0° = down, 180° = up
        Vector3 fireBoost = new Vector3(Mathf.Sin(radians), -Mathf.Cos(radians), 0f);

        // Apply the force
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(fireBoost * fireBoostStrength, ForceMode.Impulse);
    }
}
