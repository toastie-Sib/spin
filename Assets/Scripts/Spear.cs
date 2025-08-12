using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{
    public float originalHeight;
    public float scaleAmount = 0.05f;

    void Start()
    {
        Collider collider = GetComponent<Collider>();
        originalHeight = collider.bounds.size.y;
    }

    public override void IncreaseScaling()
    {
        Vector3 scale = transform.localScale;
        scale += new Vector3(0f, (0.1f), 0f);

        transform.localScale = scale;

        Vector3 position = transform.localPosition;
        position += new Vector3(0f, (0.1f), 0f);

        transform.localPosition = position;

        damage += 0.5f;
    }
}
