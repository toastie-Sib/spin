using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBall : ItemBase
{
    // Start is called before the first frame update
    public override void Start()
    {
        Fighter myFighter = GetComponentInParent<Fighter>();
        for (int i = 0; i < stacks; i++)
        {
            myFighter.maxHp *= 0.5f;
        }
    }

}
