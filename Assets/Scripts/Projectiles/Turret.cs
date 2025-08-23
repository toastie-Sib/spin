using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Bow
{
    public GameObject nose;
    [HideInInspector] public Fighter owner;
    private Color color;
    [HideInInspector] public bool side;

    // Start is called before the first frame update
    public override void Start()
    {
        rb = GetComponent<Rigidbody>();
        direction = owner.direction;
        if (owner != null)
        {
            color = owner.originalColor;
            hp = owner.maxHp/5f;
        }

        isPlayer = side;

        objectRenderer = GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;

        Weapon myWeapon = GetComponentInChildren<Weapon>();
        myWeapon.firePoint = firePoint;
    }

    // Update is called once per frame
    public override void Update()
    {
        if (hp <= 0)
        {

            Destroy(gameObject);
        }

        transform.Rotate(0f, 0f, spinMult * direction * Time.deltaTime); //Spin

        //Timers for Refresh
        if (Time.time >= nextRefreshTime && rb.useGravity == true) //Arrow Refresh
        {
            ArrowRefresh();
            nextRefreshTime = Time.time + refreshInterval;
        }
        if (arrowCount > 0 && Time.time >= nextFireTime && rb.useGravity == true) //Try to fire
        {
            // Fire from main firepoint
            FireProjectile(firePoint);

            // Fire from extra firepoints
            foreach (Transform fp in extraFirepoints)
                FireProjectile(fp);

            arrowCount -= 1;
            nextFireTime = Time.time + fireInterval;
        }
    }

    public override void FireProjectile(Transform firePoint)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile arrow = projectile.GetComponent<Projectile>();
        arrow.GetComponentInChildren<Renderer>().material.color = color;
        if (arrow != null && owner != null)
        {
            arrow.shooter = owner;
            arrow.side = side;


            //item Blood of the Knight
            if (owner.GetComponentInChildren<BloodoftheKnight>() != null)
            {
                BloodoftheKnight BotK = owner.GetComponentInChildren<BloodoftheKnight>();
                arrow.damage += BotK.damage;
            }
            //Blood of the Archer
            if (owner.GetComponentInChildren<BloodoftheArcher>() != null)
            {
                float angleSpread = 30;
                BloodoftheArcher BotA = owner.GetComponentInChildren<BloodoftheArcher>();
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
                    bloodArrow.shooter = owner;
                    bloodArrow.side = side;
                    bloodArrow.GetComponentInChildren<Renderer>().material.color = color;

                    //Item
                    if (GetComponentInChildren<BloodoftheKnight>() != null)
                    {
                        BloodoftheKnight BotK = GetComponentInChildren<BloodoftheKnight>();
                        bloodArrow.damage += BotK.damage;
                    }
                }

                arrow.transform.Rotate(0, 0, angleSpread);
            }
            // Glass ball
            if (owner.GetComponentInChildren<GlassBall>() != null)
            {
                GlassBall glassBall = GetComponent<GlassBall>();
                for (int i = -1; i < glassBall.stacks; i++) { arrow.damage *= 2; }
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
