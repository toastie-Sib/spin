using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignAnimation : Assign
{
    public GameObject stashedAnimation;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        GameObject animation = Instantiate(es.animatorPrefab, transform.position, Quaternion.identity);

        stashedAnimation = animation;

        animation.transform.localScale *= 1.5f;
    }
}
