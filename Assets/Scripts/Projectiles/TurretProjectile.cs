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

        //if (other.gameObject.CompareTag("Weapon"))
        //{
        //
        //    shooter.animationRef.GetComponent<AnimationMovement>().SpareProjAttackAnimation();
        //}

        if (other.gameObject.CompareTag("Fighter"))
        {

            shooter.animationRef.GetComponent<AnimationMovement>().SpareProjAttackAnimation();


            Fighter otherFighter = other.GetComponent<Fighter>();
            if (side != otherFighter.isPlayer)
                otherFighter.DelayedHurtAnimation(0.45f);
        }
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
