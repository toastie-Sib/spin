using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainingDummy : Familiar
{

    public override void Start()
    {
        minDistanceFromFighters = 1.2f;
        dummyRadius = 0.65f;
        spawner = Resources.Load<GameObject>("Spawns/TrainingDummy");

        base.Start();
        
    }

}
