using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMovement : MonoBehaviour
{
    public Transform startingPoint;
    public Transform attackPoint;
    public Transform parryPoint;
    [HideInInspector] public float slideDuration = 0.01f;
    public bool autoMove = true;

    void Start()
    {
        startingPoint.transform.parent = null;
        attackPoint.transform.parent = null;
        parryPoint.transform.parent = null;
    }

    public void StartingPoint()
    {
        if (autoMove == false) return;
        StartCoroutine(SlideToPoint(startingPoint.position, slideDuration));
    }

    IEnumerator SlideToPoint(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float time = 0f;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // snap to final position
    }

    public void AttackPoint()
    {
        StartCoroutine(SlideToPoint(attackPoint.position, slideDuration));

    }

    public void ParryPoint()
    {
        StartCoroutine(SlideToPoint(parryPoint.position, slideDuration));
    }

    public void StartingPointMap()
    {
        StartCoroutine(SlideToPoint(startingPoint.position, slideDuration));
    }
}
