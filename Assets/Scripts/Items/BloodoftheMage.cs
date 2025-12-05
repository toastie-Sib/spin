using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodoftheMage : ItemBase
{
    private int hitsDone = 0;

    public GameObject explosionEffect;

    public override void Start()
    {
        base.Start();
        explosionEffect = Resources.Load<GameObject>("Effects/Explosion");
    }

    public void OnTriggerEnter(Collider other)
    {

        Fighter myFighter = GetComponentInParent<Fighter>();
        Fighter otherFighter = other.GetComponent<Fighter>();
        Weapon myWeapon = myFighter.weapon.GetComponent<Weapon>();

        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (otherFighter.isInvincible == false && myWeapon.doNotHurt == false)
            {
                hitsDone += 1;
                if (hitsDone == 6 - stacks)
                {
                    hitsDone = 0;
                    GameObject explosion = Instantiate(explosionEffect, otherFighter.transform.position, Quaternion.identity);


                    float damage = myWeapon.damage + myFighter.bonusDamage;
                    otherFighter.HitDetect(damage);

                    //Knockback
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
            }
        }
    }
}
