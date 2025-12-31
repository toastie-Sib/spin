using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodoftheKnight : ItemBase
{
    private HashSet<GameObject> alreadyTriggered = new HashSet<GameObject>();
    public float increase = 0.0f;
    public void OnTriggerEnter(Collider other)
    {
        Fighter myFighter = GetComponentInParent<Fighter>();
        Fighter otherFighter = other.GetComponent<Fighter>();


        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (otherFighter.isInvincible == false && (myFighter.isPlayer != otherFighter.isPlayer || otherFighter.neutralParty == true) 
                && other.GetComponent<Turret>() == null && myFighter.GetComponentInChildren<Weapon>().doNotHurt == false)
            {
                IncreaseScaling();
                increase += 1;
            }

        }

        if (alreadyTriggered.Contains(other.gameObject)) return;

        alreadyTriggered.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        alreadyTriggered.Remove(other.gameObject);
    }

    public void IncreaseScaling() { if(increase < (15 * stacks)) { GetComponent<Weapon>().damage += (0.2f * stacks); } }
} //Also Changed Projectile, Turret, and Shield
