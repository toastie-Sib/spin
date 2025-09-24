using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    [Header("Sword Weapon Type")]
    public float damageIncrease = 1;

    private int stackIncreases = 0;

    public GameObject bladeBeam;

    public override void Start()
    {
        base.Start();

        if (gatitoBlade == false)
        {
            myFighter.UpdateDynamicUI("Dmg Increase: ", stackIncreases, 1);
            myFighter.UpdateDynamicUI("Damage: ", damage, 2);
        }

    }
    

    public override void IncreaseScaling()
    {
        base.IncreaseScaling();
        damage += damageIncrease;
        stackIncreases += 1;

        if (GetComponentInParent<Axe>() != null)
        {
            if (GetComponent<BloodoftheBandit>() != null)
            {
                for (int i = 0; i < GetComponent<BloodoftheBandit>().stacks; i++)
                {
                    if (GetComponentInParent<Axe>().weapon.GetComponent<BloodoftheBandit>().applied < 25* GetComponent<BloodoftheBandit>().stacks) { myFighter.GetComponent<Axe>().refreshInterval *= 0.99f; }
                    myFighter.UpdateDynamicUI("Fire Rate: ", myFighter.GetComponent<Axe>().refreshInterval, 3);
                }
                
            }
        }

        if(gatitoBlade == false)
        {
            myFighter.UpdateDynamicUI("Dmg Increase: ", stackIncreases, 1);
            myFighter.UpdateDynamicUI("Damage: ", damage, 2);
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

    public void BladeBeam()
    {
        GameObject projectile = Instantiate(bladeBeam, firePoint.position, firePoint.rotation);

        Projectile arrow = projectile.GetComponent<Projectile>();
        if (arrow != null)
        {
            arrow.shooter = myFighter;
            arrow.side = side;
            arrow.damage = stackIncreases;
        }

        
    }
}
