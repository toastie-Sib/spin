using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : Fighter
{


    public override void Update()
    {
        base.Update();
        Vector3 velocity = rb.velocity;

        if (velocity.sqrMagnitude > 0.01f) // make sure it's actually moving
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("BottomWall"))
        {
            Sword weapon = GetComponentInChildren<Sword>();
            weapon.IncreaseScaling();
            weapon.damage = Mathf.Round(weapon.damage * 10.0f) * 0.1f;
        }
    }
}