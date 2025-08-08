using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Fighter
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float refreshInterval = 1f;         // Fire every second
    private float nextRefreshTime = 0.5f;
    public float fireInterval = 0.1f;
    private float nextFireTime = 0.0f;
    public float arrowCount = 1f;
    public float maxArrowCount = 1f;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {

        if (hp <= 0)
        {
            Destroy(gameObject);
        }

        if (Time.time >= nextRefreshTime)
        {
            ArrowRefresh();
            nextRefreshTime = Time.time + refreshInterval;
        }
        if (arrowCount > 0 && Time.time >= nextFireTime)
        {
            FireProjectile();
            arrowCount -= 1;
            nextFireTime = Time.time + fireInterval;
        }

        float speed = rb.velocity.magnitude;

        transform.Rotate(0f, 0f, spinMult * direction * Time.deltaTime);

        Vector3 velocity = rb.velocity;

        if (isInvincible && Time.time >= invincibleUntil)
        {
            isInvincible = false;
        }

        //Nudge
        bool isMovingSlow = Mathf.Abs(velocity.magnitude) < velocityThreshold;
        bool cooldownPassed = Time.time - lastNudgeTime >= nudgeCooldown;
        if (isMovingSlow && cooldownPassed && (transform.position.x < -5.1f || transform.position.x > 5.1f))
        {
            float xNudge = 0f;

            if (velocity.x > 0)
                xNudge = -nudgeForce;
            else if (velocity.x < 0)
                xNudge = nudgeForce;
            else
                xNudge = (Random.value > 0.5f) ? nudgeForce : -nudgeForce; // Random left/right if stuck

            Vector3 nudge = new Vector3(xNudge, 0f, 0f);

            rb.AddForce(nudge, ForceMode.Impulse);
            lastNudgeTime = Time.time; // Important: Reset cooldown
            Debug.Log("Poke Poke :)");
        }
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Arrow arrow = projectile.GetComponent<Arrow>();
        if (arrow != null)
        {
            arrow.shooter = this; // Or whatever object has the script that controls rate of fire
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // Ignore collision between shooter and projectile
        Collider shooterCollider = GetComponent<Collider>();
        Collider projectileCollider = projectile.GetComponent<Collider>();
        if (shooterCollider != null && projectileCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, shooterCollider);
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

    public void IncreaseFireRate()
    {
        maxArrowCount += 1;
    }

}