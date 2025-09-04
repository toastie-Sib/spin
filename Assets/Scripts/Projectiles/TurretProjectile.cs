using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : Projectile
{
    [HideInInspector] public Turret cannon;
    public override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Turret>() != null) return;

        base.OnTriggerEnter(other);
    }

    public override void ScalingIncrease()
    {
        base.ScalingIncrease();
    }

    public override void BotbScale()
    {
        cannon.refreshInterval *= 0.99f;
    }
}
