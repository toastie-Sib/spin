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
