using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    public float damageIncrease = 1;
    public override void IncreaseScaling()
    {
        damage += damageIncrease;
    }
}
