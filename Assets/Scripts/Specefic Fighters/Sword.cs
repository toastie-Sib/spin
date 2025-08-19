using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    [Header("Sword Weapon Type")]
    public float damageIncrease = 1;
    

    public override void IncreaseScaling()
    {
        damage += damageIncrease;
    }

    public override void TriggerParryImpactFrames()
    {
        GameSpeedManager.Instance.PauseForImpact(0.4f);
    }
}
