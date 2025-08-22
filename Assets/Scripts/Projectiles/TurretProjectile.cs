using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : Projectile
{
    public override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Turret>() != null) return;

        base.OnTriggerEnter(other);
    }

    public override void ScalingIncrease()
    {
        base.ScalingIncrease();
    }
}
