using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    [Header("Sword Weapon Type")]
    public float damageIncrease = 1;

    public override void Start()
    {
        base.Start();

        myFighter.UpdateDynamicUI("Dmg Increase: ", damageIncrease, 1);
        myFighter.UpdateDynamicUI("Damage: ", damage, 2);

        if (GetComponentInParent<Axe>() != null)
        {
            
            if (GetComponentInParent<Axe>().refreshInterval > 0.1f) { 
                GetComponentInParent<Axe>().refreshInterval -= 0.1f; 
            }
            
        }
    }
    

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
                    if (myFighter.GetComponent<Bow>().refreshInterval > 0.5f) { myFighter.GetComponent<Bow>().refreshInterval *= 0.99f; }
                    myFighter.UpdateDynamicUI("Fire Rate: ", myFighter.GetComponent<Axe>().refreshInterval, 3);
                }
                
            }
        }

        myFighter.UpdateDynamicUI("Dmg Increase: ", damageIncrease, 1);
        myFighter.UpdateDynamicUI("Damage: ", damage, 2);
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
