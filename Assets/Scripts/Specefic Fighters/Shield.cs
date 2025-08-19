using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon
{
    public override void OnTriggerEnter(Collider other)
    {
        Fighter otherFighter = other.gameObject.GetComponentInParent<Fighter>();
        if (other.gameObject.CompareTag("Weapon")) //Parry
        {
            base.OnTriggerEnter(other);
            Weapon otherWeapon = other.gameObject.GetComponentInParent<Weapon>();

            otherFighter.isInvincible = false;
            if (otherWeapon.scythe == true) { otherFighter.ApplyPoison(); }
            if (otherWeapon.doNotHurt == false)
            {
                otherFighter.HitDetect(otherWeapon.damage);
                ShieldGrow(otherWeapon.damage);
            }

        }

        // shield v unarmed
        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (otherFighter.isUnarmed == true)
            {
                Fighter myFighter = GetComponentInParent<Fighter>();
                float damage = Mathf.RoundToInt(Mathf.Abs((otherFighter.rb.velocity.magnitude / 5))); // Same formula as Unarmed

                otherFighter.HitDetect(damage);

                ShieldGrow(damage);

                myFighter.ReverseDirection();
                myFighter.isInvincible = true;
            }
        }
    }
}
