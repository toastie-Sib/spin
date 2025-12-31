using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodoftheMage : ItemBase
{
    [HideInInspector] public int hitsDone = 0;
    [HideInInspector] public bool readyToExplode = false;
    [HideInInspector] public GameObject explosionEffect;

    public override void Start()
    {
        base.Start();
        explosionEffect = Resources.Load<GameObject>("Spawns/Explosion");
    }

    public void OnTriggerEnter(Collider other)
    {

        Fighter myFighter = GetComponentInParent<Fighter>();
        Fighter otherFighter = other.GetComponent<Fighter>();
        Weapon myWeapon = myFighter.weapon.GetComponent<Weapon>();

        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (otherFighter.isInvincible == false && myWeapon.doNotHurt == false && myFighter.isPlayer != otherFighter.isPlayer && other.GetComponent<Turret>() == null)
            {
                hitsDone += 1;
                
            }

            if (hitsDone >= 6 - stacks && myWeapon.scythe == true && myFighter.isPlayer != otherFighter.isPlayer)
            {
                hitsDone = 0;
                
                myWeapon.ScytheApply(otherFighter);

                ExplosionKnockback(otherFighter, myFighter);
            }
            else if (hitsDone >= 6 - stacks && myFighter.isPlayer != otherFighter.isPlayer)
            {
                hitsDone = 0;

                float damage = myWeapon.damage + myFighter.bonusDamage;
                myFighter.DealingDamage(damage, otherFighter);

                ExplosionKnockback(otherFighter, myFighter);
            }
        }
    }

    public void ExplosionKnockback(Fighter otherFighter, Fighter myFighter)
    {
        ExplosionEffect(otherFighter);

        //Apply Force away when shot
        float zRot = transform.eulerAngles.z;

        // Convert to radians for Mathf trig functions
        float radians = zRot * Mathf.Deg2Rad;

        // Calculate direction based on rotation
        // This makes 0° = down, 180° = up
        Vector3 fireBoost = new Vector3(Mathf.Sin(radians), -Mathf.Cos(radians), 0f);

        // Apply the force
        Rigidbody rb = myFighter.GetComponent<Rigidbody>();
        rb.AddForce(fireBoost * stacks * 20, ForceMode.Impulse);

        Rigidbody otherRb = otherFighter.GetComponent<Rigidbody>();
        otherRb.AddForce(-fireBoost * stacks * 20, ForceMode.Impulse);
    }

    public void ExplosionEffect(Fighter otherFighter)
    {
        GameObject explosion = Instantiate(explosionEffect, otherFighter.transform.position, Quaternion.identity);
    }

    public void RemoteTrigger()
    {
        hitsDone += 1;
        if (hitsDone >= 6 - stacks)
        {
            hitsDone = 0;
            
            readyToExplode = true;
        }
    }

    public void RemoteExplode(Fighter otherFighter)
    {
        GameObject explosion = Instantiate(explosionEffect, otherFighter.transform.position, Quaternion.identity);

        //Knockback
        //Apply Force away when shot
        float zRot = transform.eulerAngles.z;

        // Convert to radians for Mathf trig functions
        float radians = zRot * Mathf.Deg2Rad;

        // Calculate direction based on rotation
        // This makes 0° = down, 180° = up
        Vector3 fireBoost = new Vector3(Mathf.Sin(radians), -Mathf.Cos(radians), 0f);

        Rigidbody otherRb = otherFighter.GetComponent<Rigidbody>();
        otherRb.AddForce(-fireBoost * stacks * 20, ForceMode.Impulse);
    }
}
