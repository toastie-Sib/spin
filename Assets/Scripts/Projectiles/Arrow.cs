using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    public override void ScalingIncrease()
    {
        base.ScalingIncrease();
        Bow bow = shooter.GetComponentInParent<Bow>();
        bow.IncreaseFireRate();
    }
}
