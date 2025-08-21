using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Bow
{
    public GameObject nose;
    [HideInInspector]
    public Fighter owner;
    private Color color;
    // Start is called before the first frame update
    public override void Start()
    {
        rb = GetComponent<Rigidbody>();
        direction = owner.direction;
        if (owner != null)
        {
            color = owner.originalColor;
        }
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
        arrow.GetComponentInChildren<Renderer>().material.color = color;
        if (arrow != null && owner != null)
        {
            arrow.shooter = owner;
            

            //item
            if (owner.GetComponentInChildren<BloodoftheKnight>() != null)
            {
                BloodoftheKnight BotK = owner.GetComponentInChildren<BloodoftheKnight>();
                arrow.damage += BotK.damage;
            }
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
