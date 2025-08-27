using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Training : ItemBase
{


    // Start is called before the first frame update
    public override void Start()
    {
        Weapon myWeapon = GetComponent<Weapon>();
        for (int i = 0; i < stacks; i++)
        {
            myWeapon.TriggerScaling();
            myWeapon.TriggerScaling();
        }
    }

}
