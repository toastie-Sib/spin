using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintedParry : ItemBase
{
    public override void Start()
    {
        base.Start();
        Fighter myFighter = GetComponentInParent<Fighter>();
        for (int i = -1; i < stacks; i++)
        {
            myFighter.invincibilityDuration += 0.1f;
        }
    }
}
