using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Weapon
{
    public override void IncreaseScaling()
    {
        base.IncreaseScaling();
        Fighter myFighter = GetComponentInParent<Fighter>();
        myFighter.IncreaseSpeed();
    }
}
