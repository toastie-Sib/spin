using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignAnimation : Assign
{
    public Animator stashedAnimation;
    public bool isPlayer = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        StartCoroutine(CheckIfPlayer());
    }

    public IEnumerator CheckIfPlayer()
    {
        yield return new WaitForSeconds(0.000001f);
        if (isPlayer == true)
        {
            SpawnAnimation(es.animatorPrefab);

        }
        else
        {
            SpawnAnimation(es.otherAnimPrefab);

        }
    }

    void SpawnAnimation(GameObject prefab)
    {
        GameObject animation = Instantiate(prefab, transform.position, Quaternion.identity);

        stashedAnimation = animation.GetComponent<Animator>();

        Vector3 scale = transform.localScale;
        animation.transform.localScale = scale;
        Quaternion rotation = transform.localRotation;
        animation.transform.localRotation = rotation;
    }
}
