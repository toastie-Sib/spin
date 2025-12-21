using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriTippedDagger : ItemBase
{
    private HashSet<GameObject> alreadyTriggered = new HashSet<GameObject>();
    public void OnTriggerEnter(Collider other)
    {
        Fighter myFighter = GetComponentInParent<Fighter>();
        Fighter otherFighter = other.GetComponent<Fighter>();


        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (otherFighter.isInvincible == false && myFighter.GetComponent<Bow>() == null && myFighter.GetComponentInChildren<Shield>() == null)
            {
                Effect(otherFighter);

            }

        }


        if (alreadyTriggered.Contains(other.gameObject)) return;

        alreadyTriggered.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        alreadyTriggered.Remove(other.gameObject);
    }

    public void Effect(Fighter otherFighter)
    {
        SeedManager.Instance.UseSubSeed("TriTippedDagger"); //generate random 

        int randomInt = Random.Range(0, 100);
        if (randomInt <= 20 * stacks)
        {
            otherFighter.bleedStacks += 1;
        }

        SeedManager.Instance.RestoreMasterSeed();
    }
}
