using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatitoBlade : ItemBase
{
    private bool hasSpawned = false;

    [HideInInspector] public float nextFireTime = 0.0f;
    [HideInInspector] public Transform firepoint;
    private GatitoBlade gb;

    public override void Start() //Make sure to update with Fighter
    {
        stacks = 5;
        base.Start();
        if (hasSpawned == false)
        {
            Fighter myFighter = GetComponentInParent<Fighter>();
            Weapon myWeapon = GetComponentInParent<Weapon>();


            // Spawn the blade (only one extra)
            GameObject blade = Instantiate(myFighter.weapon.gameObject, myFighter.transform);

            // Put it just below fighter
            blade.transform.localPosition = new Vector3(0, -.4f+(0.1f * stacks), 0);

            // Scale the blade based on stacks
            float sizeMultiplier = 0.2f + (0.2f * stacks); // each stack makes it 20% bigger
            blade.transform.localScale *= sizeMultiplier;

            // Flip it upside down
            blade.transform.localRotation *= Quaternion.Euler(0, 0, 180);

            // Mark it as a spawned blade
            gb = blade.GetComponent<GatitoBlade>();
            if (gb == null) //Case for Turret
            {
                GatitoBlade newGatitoBlade = blade.AddComponent<GatitoBlade>();
                newGatitoBlade.stacks = stacks;
                gb = blade.GetComponent<GatitoBlade>();
                blade.transform.localScale = new Vector3 (0.3f,0.3f, 0.01f);
                blade.transform.localPosition = new Vector3(0, -0.6f, 0);
            }
            if (gb != null) gb.hasSpawned = true;

            // Reduce damage (since it’s an extra)
            Weapon weapon = blade.GetComponent<Weapon>();
            if (weapon != null) weapon.damage *= 0.5f;

            // Register firepoint if it’s a Bow
            Bow hasBow = gb.GetComponentInParent<Bow>();
            if (hasBow != null)
            {
                gb.firepoint = weapon.firePoint;
                hasBow.RegisterExtraFirepoint(gb.firepoint);
            }
            
            if (myWeapon.GetComponent<Scythe>() != null) // switched logic since it is cleaner, could do for above too
            {
                Scythe hasScythe = myWeapon.GetComponent<Scythe>();
                Scythe gbScythe = gb.GetComponent<Scythe>();
                hasScythe.GBScythe = gbScythe;
                gbScythe.GBScythe = hasScythe;

            }


            hasSpawned = true;
        }
    }


}