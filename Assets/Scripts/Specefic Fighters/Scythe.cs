using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : Weapon
{
    private int startingDirection;
    private Coroutine parryRoutine;
    [HideInInspector] public Scythe GBScythe;

    public override void Start()
    {
        base.Start();
        startingDirection = myFighter.direction;
        if (startingDirection == 1)
        {
            transform.Rotate(0, 180f, 0);
        }

        myFighter.UpdateDynamicUI("Poison: ", 0, 1);
        myFighter.UpdateDynamicUI("Damage: ", damage, 2);
    }

    public override void TriggerParryImpactFrames()
    {
        base.TriggerParryImpactFrames();
        // Cancel old routine but *don’t* reset transform
        //if (parryRoutine != null)
            //StopCoroutine(parryRoutine);

        // Start a new one from the current local state
        GatitoBlade gB = GetComponent<GatitoBlade>();
        if (gB == null) { parryRoutine = StartCoroutine(ParryImpactMotion()); } else
        {
            StartCoroutine(ParryImpactMotion());
            StartCoroutine(GBScythe.ParryImpactMotion());
        }
        
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
            // Set Y to 0, preserve Z
            Quaternion initialZRotation = Quaternion.Euler(0, 0, transform.localEulerAngles.z);
            Vector3 finalEuler = initialZRotation.eulerAngles; // Start with the preserved Z
            finalEuler.y = 0; // Set Y to 0
            transform.localRotation = Quaternion.Euler(finalEuler);
        }
        
        if (myFighter.direction == 1)
        {
            // Set Y to 180, preserve Z
            Quaternion initialZRotation = Quaternion.Euler(0, 0, transform.localEulerAngles.z);
            Vector3 finalEuler = initialZRotation.eulerAngles; // Start with the preserved Z
            finalEuler.y = 180f; // Set Y to 180
            transform.localRotation = Quaternion.Euler(finalEuler);
        }
        parryRoutine = null;
    }

    
}