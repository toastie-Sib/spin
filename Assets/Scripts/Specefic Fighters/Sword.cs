using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    [Header("Sword Weapon Type")]
    public float damageIncrease = 1;
    

    public override void IncreaseScaling()
    {
        base.IncreaseScaling();
        damage += damageIncrease;

        if(GetComponentInParent<Axe>() != null)
        {
            if (GetComponent<BloodoftheBandit>() != null)
            {
                for (int i = 0; i < GetComponent<BloodoftheBandit>().stacks; i++)
                {
                    if (GetComponentInParent<Axe>().refreshInterval > 0.1f) { GetComponentInParent<Axe>().refreshInterval -= 0.1f; }
                }
                
            }
        }
    }

    public override void TriggerParryImpactFrames()
    {
        if (axe == true)
        {
            GameSpeedManager.Instance.PauseForImpact(0.4f);
        } else
        {
            GameSpeedManager.Instance.PauseForImpact(0.2f);
        }
        
    }
}
