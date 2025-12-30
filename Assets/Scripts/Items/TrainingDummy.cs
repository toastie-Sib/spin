using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : ItemBase
{
    [HideInInspector] public GameObject dummy;

    public override void Start()
    {
        base.Start();
        dummy = Resources.Load<GameObject>("Spawns/TrainingDummy");

    }
    //stacks and assign player
}
