using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBall : Familiar
{

    public override void Start()
    {
        minDistanceFromFighters = 0.8f;
        dummyRadius = 0.45f;
        spawner = Resources.Load<GameObject>("Spawns/BouncyBall");

        base.Start();

    }

}

