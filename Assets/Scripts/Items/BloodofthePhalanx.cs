using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodofthePhalanx : ItemBase
{

    private float timer = 0f;
    private bool running = false;
    private float damageTotal = 0f;
    private float tickDamage = 0f;
    private int ticks = 0;


    public void StartDamage(float damage)
    {
        running = true;
        damageTotal += damage;
        ticks = (stacks * 2) + 4;

        tickDamage = damageTotal / (4 + (stacks * 2));
    }

    void Update()
    {
        if (!running) return;

        timer += Time.deltaTime;

        if (timer >= 0.5f && ticks >= 0)
        {
            ticks--;
            timer = 0f;

            DamageTick();
        }
    }


    void DamageTick()
    {
        damageTotal -= tickDamage;
        Fighter myFighter = GetComponentInParent<Fighter>();
        //Take Damage to HP
        myFighter.hp -= tickDamage;
        myFighter.hp = Mathf.Round(myFighter.hp * 10.0f) * 0.1f;
        myFighter.hp = Mathf.Max(myFighter.hp, 0);
        myFighter.UpdateUI();

        if (damageTotal <= 0 || ticks <= 0)
        {
            running = false;
        }
    }
}
