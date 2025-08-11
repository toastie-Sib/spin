using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Projectile
{
    public override void DestroySelf()
    {
        Destroy(gameObject);
    }

    public override void ScalingIncrease()
    {
        shooter.IncreaseFireRate();
    }
}
