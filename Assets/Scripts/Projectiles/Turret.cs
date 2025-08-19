using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Bow
{
    private float nextFireTime = 0.5f;
    private float nextRefreshTime = 1f;
    public Fighter owner;
    // Start is called before the first frame update
    public override void Start()
    {
        direction = owner.direction;
    }

    // Update is called once per frame
    public override void Update()
    {
        transform.Rotate(0f, 0f, spinMult * direction * Time.deltaTime); //Spin

        //Timers for Refresh
        if (Time.time >= nextRefreshTime && rb.useGravity == true) //Arrow Refresh
        {
            ArrowRefresh();
            nextRefreshTime = Time.time + refreshInterval;
        }
        if (arrowCount > 0 && Time.time >= nextFireTime && rb.useGravity == true) //Try to fire
        {
            FireTurretProjectile();
            arrowCount -= 1;
            nextFireTime = Time.time + fireInterval;
        }
    }

    public virtual void FireTurretProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile arrow = projectile.GetComponent<Projectile>();
        if (arrow != null)
        {
            arrow.shooter = (Bow)owner;
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
}
