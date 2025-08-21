using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : Weapon
{
    private int startingDirection;
    private Fighter myFighter;
    private Coroutine parryRoutine;
    public override void Start()
    {
        myFighter = GetComponentInParent<Fighter>();
        startingDirection = myFighter.direction;
        if (startingDirection == 1)
        {
            transform.Rotate(0, 180f, 0);
        }
    }

    public override void TriggerParryImpactFrames()
    {
        base.TriggerParryImpactFrames();
        // Cancel old routine but *don’t* reset transform
        if (parryRoutine != null)
            StopCoroutine(parryRoutine);

        // Start a new one from the current local state
        parryRoutine = StartCoroutine(ParryImpactMotion());
    }

    private IEnumerator ParryImpactMotion()
    {
        for (int i = 0; i <= 12; i++)
        {
            transform.localRotation *= Quaternion.Euler(0, 15f, 0);


            yield return new WaitForSeconds(0.02f); // controls speed (0.02s = 50 FPS)
        }
        if (myFighter.direction == -1) 
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        
        if (myFighter.direction == 1)
        {
            transform.localRotation = Quaternion.Euler(0, 180f, 0);
        }
        parryRoutine = null;
    }
}