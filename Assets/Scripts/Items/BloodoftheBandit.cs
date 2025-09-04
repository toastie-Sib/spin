using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodoftheBandit : ItemBase
{
    private HashSet<GameObject> alreadyTriggered = new HashSet<GameObject>();

    public int applied = 0;

    public void OnTriggerEnter(Collider other)
    {
        Fighter myFighter = GetComponentInParent<Fighter>();
        Fighter otherFighter = other.GetComponent<Fighter>();


        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (otherFighter.isInvincible == false && myFighter.GetComponent<Bow>() == null && myFighter.GetComponentInChildren<Unarmed>() == null && myFighter.GetComponent<Axe>() == null)
            {
                for (int i = 0; i < stacks; i++)
                {
                    if (applied < 25*stacks) { myFighter.spinMult += 20; }
                }
                
                

            }

        }


        if (alreadyTriggered.Contains(other.gameObject)) return;

        alreadyTriggered.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        alreadyTriggered.Remove(other.gameObject);
    }
}
