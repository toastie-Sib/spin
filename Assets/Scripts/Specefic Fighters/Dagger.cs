using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Weapon
{
    private int stacks = 0;
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
}
