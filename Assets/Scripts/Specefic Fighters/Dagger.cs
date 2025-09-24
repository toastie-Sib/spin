using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Weapon
{
    private int stacks = 0;

    private int parries = 0;
    private bool hmiyc = false;

    public override void Start() //Make sure to update with Fighter
    {
        base.Start();

        if (gatitoBlade == false)
        {
            myFighter.UpdateDynamicUI("Extra Spd: ", stacks, 1);
            myFighter.UpdateDynamicUI("Damage: ", damage, 2);
        }
    }

    public override void IncreaseScaling()
    {
        base.IncreaseScaling();
        Fighter myFighter = GetComponentInParent<Fighter>();
        myFighter.IncreaseSpeed();

        stacks += 1;
        if (gatitoBlade == false)
        {
            myFighter.UpdateDynamicUI("Extra Spd: ", stacks, 1);
            myFighter.UpdateDynamicUI("Damage: ", damage, 2);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.CompareTag("Weapon") && hmiyc == true) //Parry
        {
            parries += 1;
        }

        if (other.gameObject.CompareTag("Fighter") && parries != 0) //Damage
        {
            Fighter otherFighter = other.GetComponent<Fighter>();
            if (side == otherFighter.isPlayer || otherFighter.isInvincible == true) return;
                otherFighter.HitDetect(parries);
                parries = 0;
            
        }
    }

    public void HMIYC()
    {
        hmiyc = true;
        StartCoroutine(HMIYCWait());
    }

    public IEnumerator HMIYCWait()
    {
        yield return new WaitForSeconds(5f);
        hmiyc = false;
        myFighter.UpdateDynamicUI("Damage: ", damage + parries, 2);
    }
}
