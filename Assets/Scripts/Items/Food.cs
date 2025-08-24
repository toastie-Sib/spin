using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : ItemBase
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        Fighter myFighter = GetComponentInParent<Fighter>();
        
        myFighter.maxHp += (myFighter.maxHp * (0.3f * stacks)); //This might not fully work with starting new fights
        myFighter.hp = myFighter.maxHp;
    }
}
