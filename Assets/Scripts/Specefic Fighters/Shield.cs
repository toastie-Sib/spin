using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon
{
    public override void OnTriggerEnter(Collider other)
    {
        Fighter otherFighter = other.gameObject.GetComponentInParent<Fighter>();
        Fighter myFighter = GetComponentInParent<Fighter>();
        if (other.gameObject.CompareTag("Weapon")) //Parry
        {
            base.OnTriggerEnter(other);
            Weapon otherWeapon = other.gameObject.GetComponentInParent<Weapon>();

            otherFighter.isInvincible = false;
            if (otherWeapon.scythe == true) { otherFighter.ApplyPoison(); } //Scythe
            if (otherWeapon.doNotHurt == false)
            {
                float totalDamage = otherWeapon.damage;
                //item
                if (GetComponent<BloodoftheKnight>() != null)
                {
                    BloodoftheKnight BotK = GetComponent<BloodoftheKnight>();
                    totalDamage += BotK.damage;
                    BotK.IncreaseScaling();
                }

                otherFighter.HitDetect(totalDamage); //Damage Fighter and Grow Shield
                ShieldGrow(otherWeapon.damage);
                if (otherWeapon is Wrench) { Wrench wrench = other.gameObject.GetComponentInParent<Wrench>();    wrench.ShieldTurret(myFighter); } // Wrench

                
            }

        }

        // shield v unarmed
        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (otherFighter.isUnarmed == true)
            {
                
                float damage = Mathf.RoundToInt(Mathf.Abs((otherFighter.rb.velocity.magnitude / 5))); // Same formula as Unarmed

                otherFighter.HitDetect(damage);

                ShieldGrow(damage);

                myFighter.ReverseDirection();
                myFighter.isInvincible = true;
            }
        }
    }
}
