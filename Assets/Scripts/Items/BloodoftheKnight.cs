using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodoftheKnight : ItemBase
{
    private HashSet<GameObject> alreadyTriggered = new HashSet<GameObject>();
    public float damage = 0.0f;
    public void OnTriggerEnter(Collider other)
    {
        Fighter myFighter = GetComponentInParent<Fighter>();
        Fighter otherFighter = other.GetComponent<Fighter>();


        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (otherFighter.isInvincible == false && myFighter.GetComponent<Bow>() == null )
            {
                otherFighter.HitDetect(damage);
                IncreaseScaling();

            }

        }

        if (alreadyTriggered.Contains(other.gameObject)) return;

        alreadyTriggered.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        alreadyTriggered.Remove(other.gameObject);
    }

    public void IncreaseScaling() { if(damage < (3f * stacks)) { damage += (0.2f * stacks); } }
} //Also Changed Projectile, Turret, and Shield
