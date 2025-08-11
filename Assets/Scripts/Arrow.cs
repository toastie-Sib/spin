using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    public override void ScalingIncrease()
    {
        shooter.IncreaseFireRate();
    }
}
