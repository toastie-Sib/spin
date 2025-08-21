using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBall : ItemBase
{
    public override void Start() //Make sure to update with Fighter
    {
        base.Start();

        Fighter myFighter = GetComponentInParent<Fighter>();

        myFighter.maxHp *= 0.5f; //BOWER NOTE MAKE SURE THAT THIS ALSO LOWERS CURRENT HP, prolly update if higher than max then go to max
        
        GameObject extraWeapon = Instantiate(myFighter.weapon, -(myFighter.weapon.transform.position), Quaternion.Euler(0, 0, 180));
        extraWeapon.transform.localScale *= 0.5f;
    }
    // if bow script exists then assign new spawn point and fire
}
