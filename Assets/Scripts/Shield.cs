using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon
{
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon")) //Parry
        {
            base.OnTriggerEnter(other);
            Weapon otherWeapon = other.gameObject.GetComponentInParent<Weapon>();
            Fighter otherFighter = other.gameObject.GetComponentInParent<Fighter>();
            otherFighter.isInvincible = false;
            if (otherWeapon.doNotHurt == true) return;
            otherFighter.HitDetect(otherWeapon.damage);
            ShieldGrow(otherWeapon.damage);
        }
    }
}
