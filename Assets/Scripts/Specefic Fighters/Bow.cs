using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Fighter //Inherit Fighter
{
    [Header("Projectile User")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    //Shot Frequency
    public float refreshInterval = 1f;         // Fire every second
    
    public float fireInterval = 0.1f;          // How fast Fire
    
    //Shot Count
    public float arrowCount = 1f;
    public float maxArrowCount = 1f;
    [HideInInspector]
    public float nextFireTime = 0.0f;
    public float nextRefreshTime = 0.5f;

    // Update is called once per frame
    public override void Update() //Make sure to update with Fighter
    {
        base.Update();

        //Timers for Refresh
        if (Time.time >= nextRefreshTime && rb.useGravity == true) //Arrow Refresh
        {
            ArrowRefresh();
            nextRefreshTime = Time.time + refreshInterval;
        }
        if (arrowCount > 0 && Time.time >= nextFireTime && rb.useGravity == true) //Try to fire
        {
            FireProjectile();
            arrowCount -= 1;
            nextFireTime = Time.time + fireInterval;
        }
    }

    public virtual void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile arrow = projectile.GetComponent<Projectile>();
        if (arrow != null)
        {
            arrow.shooter = this;
        }

        //Item Blood of the Knight
        if (GetComponentInChildren<BloodoftheKnight>() != null)
        { 
            BloodoftheKnight BotK = GetComponentInChildren<BloodoftheKnight>();
            arrow.damage += BotK.damage;
        }
        //Blood of the Archer
        if (GetComponentInChildren<BloodoftheArcher>() != null)
        {
            float angleSpread = 30;
            BloodoftheArcher BotA = GetComponentInChildren<BloodoftheArcher>();
            float currentAngleSpread = (BotA != null) ? angleSpread : 0f;
            float startAngle = -((BotA.stacks - 1) * currentAngleSpread) / 2f;
            for (int i = 0; i < BotA.stacks; i++)
            {
                // Calculate the current projectile's angle offset
                float currentAngleOffset = startAngle + (i * currentAngleSpread);

                // Create a rotation for this specific projectile
                // We apply the offset to the firePoint's original rotation
                Quaternion projectileRotation = firePoint.rotation * Quaternion.Euler(0, 0, currentAngleOffset);

                // Instantiate a new projectile
                GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, projectileRotation);
                Projectile bloodArrow = projectileGO.GetComponent<Projectile>();
                bloodArrow.shooter = this;

                //Item
                if (GetComponentInChildren<BloodoftheKnight>() != null)
                {
                    BloodoftheKnight BotK = GetComponentInChildren<BloodoftheKnight>();
                    bloodArrow.damage += BotK.damage;
                }
            }

            arrow.transform.Rotate(0,0,angleSpread);
        }

        // Ignore collision between bow and projectile
        Collider projectileCollider = projectile.GetComponent<Collider>();
        Collider[] weaponColliders = GetComponentsInChildren<Collider>();
        foreach (Collider weaponCol in weaponColliders)
        {
            if (weaponCol != GetComponent<Collider>()) // skip parent's collider
            {
                Physics.IgnoreCollision(weaponCol, projectileCollider);
            }
        }

    }

    public void ArrowRefresh()
    {
        if (arrowCount > 0)
        {
            fireInterval -= 0.01f;
        }
        arrowCount = maxArrowCount;
        
    }


    // Bow Scale
    public void IncreaseFireRate()
    {
        maxArrowCount += 1;
    }

}