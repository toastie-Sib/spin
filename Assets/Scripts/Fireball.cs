using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Projectile
{
    [Header("Fireball")]
    public float explosionRadius = 5f;
    public float explosionDamage = 2f;
    public GameObject explosionEffect; // Optional VFX prefab

    public override void DestroySelf()
    {
        if (explosionEffect != null) //Visual Effect
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius); // Detect all in range
        foreach (Collider hit in hitColliders)
        {
            Fighter fighter = hit.GetComponent<Fighter>();
            if (fighter != null)
            {
                fighter.HitDetect(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    public override void ScalingIncrease()
    {
        //shooter.IncreaseFireRate();
    }
}
