using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{

    public override void Start()
    {
        base.Start();
        Collider collider = GetComponent<Collider>();
    }

    public override void IncreaseScaling()
    {
        base.IncreaseScaling();
        Vector3 scale = transform.localScale;
        scale += new Vector3(0f, (0.1f), 0f);

        transform.localScale = scale;

        Vector3 position = transform.localPosition;
        position -= new Vector3(0f, (0.05f), 0f);

        transform.localPosition = position;

        damage += 0.5f;
    }
}
