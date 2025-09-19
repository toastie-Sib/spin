using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Fighter //Inherit Fighter
{
    [Header("Projectile User")]
    public GameObject projectilePrefab;
    public GameObject animatedProjectilePrefab;
    public Transform firePoint;
    //Shot Frequency
    public float refreshInterval = 1f;         // Fire every second
    
    public float fireInterval = 0.1f;          // How fast Fire
    
    //Shot Count
    public float arrowCount = 1f;
    public float maxArrowCount = 1f;
    [HideInInspector] public float nextFireTime = 0.0f;
    public float nextRefreshTime = 0.5f;
    [HideInInspector] public List<Transform> extraFirepoints = new List<Transform>();
    [HideInInspector] public Weapon myWeapon;

    public override void Start() //Make sure to update with Fighter
    {
        base.Start();

        myWeapon = GetComponentInChildren<Weapon>();
        myWeapon.firePoint = firePoint;

        //Training Item
        if (GetComponentInChildren<Training>() != null)
        {
            Training glassBall = GetComponentInChildren<Training>();
            for (int i = 0; i < glassBall.stacks; i++)
            {
                IncreaseProjectileScale(); // Call the actual scaling logic 'stacks' times
                IncreaseProjectileScale();
            }
        }

        UpdateDynamicUI("Arrows: ", maxArrowCount, 1);
        if (weapon.GetComponent<Weapon>().gatitoBlade == false)
            UpdateDynamicUI("Damage: ", 1 + bonusDamage + myWeapon.damage, 2);
        UpdateDynamicUI("Fire Rate: ", refreshInterval, 3);
    }

    public override void IncreaseBaseAtkSpeed()
    {

        for (int i = 0; i < SceneSwitcher.Instance.playerBonusAtkSpd; i++)
        {
            spinMult += (spinMult * 0.25f);
            refreshInterval *= 0.90f;
        }

    }

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
            // Fire from main firepoint
            FireProjectile(firePoint);

            // Fire from extra firepoints
            foreach (Transform fp in extraFirepoints)
                FireProjectile(fp);

            arrowCount -= 1;
            nextFireTime = Time.time + fireInterval;
        }
    }

    public virtual void FireProjectile(Transform firePoint)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile arrow = projectile.GetComponent<Projectile>();
        if (arrow != null)
        {
            arrow.shooter = this;
            arrow.side = isPlayer;
            arrow.damage += bonusDamage;
        }

        //Item Blood of the Knight
        if (GetComponentInChildren<BloodoftheKnight>() != null)
        { 
            arrow.damage += myWeapon.damage;
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
                bloodArrow.side = isPlayer;

                //Item BOTH HERE AGAIN COULD CONDENCE??
                if (GetComponentInChildren<BloodoftheKnight>() != null)
                {
                    bloodArrow.damage += myWeapon.damage;
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
    public virtual void IncreaseProjectileScale()
    {
        maxArrowCount += 1;
        UpdateDynamicUI("Arrows: ", maxArrowCount, 1);
        if (weapon.GetComponent<Weapon>().gatitoBlade == false)
            UpdateDynamicUI("Damage: ", 1 + bonusDamage + myWeapon.damage, 2);
        UpdateDynamicUI("Fire Rate: ", refreshInterval, 3);
    }

    public void RegisterExtraFirepoint(Transform fp)
    {
        if (!extraFirepoints.Contains(fp))
            extraFirepoints.Add(fp);
    }

    public override void AttackAnimation()
    {
        base.AttackAnimation();
        GameObject projectile = Instantiate(animatedProjectilePrefab, animationRef.transform.position, animationRef.transform.rotation);
        projectile.transform.Rotate(0, 0, -90);
        StartCoroutine(AnimationProjectile(projectile));
    }

    public IEnumerator AnimationProjectile(GameObject projectile)
    {
        yield return new WaitForSeconds(0.75f);
        Destroy(projectile);
    }
}