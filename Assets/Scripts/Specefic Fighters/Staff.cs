using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : Bow
{
    [Header("Staff")]
    public float fireBoostStrength = 5f;
    public float explosionRadius = 1f;
    public float damageIncrease = 0f;

    public override void FireProjectile(Transform firePoint)
    {
        base.FireProjectile(firePoint);
        
        //Apply Force away when shot
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
    
    public override void IncreaseProjectileScale()
    {
        explosionRadius += 0.25f;
        damageIncrease += 1f;
    }
}
