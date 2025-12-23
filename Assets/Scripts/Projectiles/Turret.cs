using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Bow
{
    public GameObject nose;
    [HideInInspector] public Fighter owner;
    private Color color;
    [HideInInspector] public bool side;


    public GameObject explosionEffectSDB;

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
        if (owner.isActive == false) return;
        if (hp <= 0)
        {
            owner.GetComponentInChildren<Wrench>().turrets -= 1;
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

        TurretProjectile arrow = projectile.GetComponent<TurretProjectile>();
        arrow.GetComponentInChildren<Renderer>().material.color = color;
        if (arrow != null && owner != null)
        {
            arrow.shooter = owner;
            arrow.cannon = this;
            arrow.side = side;
            arrow.damage += bonusDamage;

            //item Blood of the Knight
            if (owner.GetComponentInChildren<BloodoftheKnight>() != null)
            {
                BloodoftheKnight BotK = owner.GetComponentInChildren<BloodoftheKnight>();
                arrow.damage += (0.2f * BotK.stacks);
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
                    TurretProjectile bloodArrow = projectileGO.GetComponent<TurretProjectile>();
                    bloodArrow.shooter = owner;
                    bloodArrow.side = side;
                    bloodArrow.GetComponentInChildren<Renderer>().material.color = color;

                    //Item
                    if (GetComponentInChildren<BloodoftheKnight>() != null)
                    {
                        BloodoftheKnight BotK = GetComponentInChildren<BloodoftheKnight>();
                        bloodArrow.damage += (0.2f * BotK.stacks);
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

    public void SelfDestructButton()
    {
        if (explosionEffectSDB != null) //Visual Effect
        {
            GameObject explosion = Instantiate(explosionEffectSDB, transform.position, Quaternion.identity);
            float scaleFactor = 1.25f;
            Vector3 Scale = explosion.transform.localScale;
            Scale = Scale * scaleFactor;
            explosion.transform.localScale = Scale;
        }

        Color finalColor = Color.red;
        finalColor.a = 0.0f;
        SpriteRenderer childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        childSpriteRenderer.color = finalColor;
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = 2f * 2f;
        sphereCollider.isTrigger = true;

        StartCoroutine(ActuallyDestroy());
    }

    private IEnumerator ActuallyDestroy()
    {
        yield return new WaitForSeconds(0.5f); //MAKE SURE THIS IS THE SAME AS THE EXPLOSION VALUE
        Destroy(gameObject);
    }
    //mesh with turret and make sure on trigger doesn't happen if self destruct not active (is this optimal for resources?)
    public virtual void OnTriggerEnter(Collider other)
    {
        Fighter otherFighter = other.GetComponent<Fighter>();
        
        
         
        if (other.gameObject.CompareTag("Fighter"))
        {

            if (side == otherFighter.isPlayer) return;
            if (otherFighter.isInvincible) return;

            owner.AttackAnimation(otherFighter);

            float damage = bonusDamage + 1;

            //item Blood of the Knight
            if (owner.GetComponentInChildren<BloodoftheKnight>() != null)
            {
                BloodoftheKnight BotK = owner.GetComponentInChildren<BloodoftheKnight>();
                damage += (0.2f * BotK.stacks);
            }


            owner.DealingDamage(damage * 5, otherFighter);
            otherFighter.HurtAnimation();
        }

        

    }
}
