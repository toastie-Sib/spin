using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Arrow
{
    public override void DestroySelf()
    {
        Destroy(gameObject);
    }
}
