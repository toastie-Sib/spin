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
    private float nextRefreshTime = 0.5f;
    public float fireInterval = 0.1f;          // How fast Fire
    private float nextFireTime = 0.0f;
    //Shot Count
    public float arrowCount = 1f;
    public float maxArrowCount = 1f;

    // Update is called once per frame
    public override void Update() //Make sure to update with Fighter
    {
        base.Update();

        //Timers for Refresh
        if (Time.time >= nextRefreshTime) //Arrow Refresh
        {
            ArrowRefresh();
            nextRefreshTime = Time.time + refreshInterval;
        }
        if (arrowCount > 0 && Time.time >= nextFireTime) //Try to fire
        {
            FireProjectile();
            arrowCount -= 1;
            nextFireTime = Time.time + fireInterval;
        }
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile arrow = projectile.GetComponent<Projectile>();
        if (arrow != null)
        {
            arrow.shooter = this;
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

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

    void ArrowRefresh()
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