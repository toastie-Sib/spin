using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodofthePhalanx : ItemBase
{

    private float timer = 0f;
    private bool running = false;
    private float damageTotal = 0f;
    private int ticks = 0;

    void Update()
    {
        if (!running) return;

        timer += Time.deltaTime;

        if (timer >= 0.5f && ticks >= 0)
        {
            DamageTick();
            ticks--;
            timer = 0f;
        }
    }

    public void StartDamage(float damage)
    {
        timer = 0f;
        running = true;
        damageTotal += damage;
        ticks = (stacks * 2) + 4;
    }

    public void StopTimer()
    {
        running = false;
    }

    void DamageTick()
    {
        float damage = damageTotal / (4 + (stacks * 2));
        damageTotal -= damage;
        Fighter myFighter = GetComponentInParent<Fighter>();
        //Take Damage to HP
        myFighter.hp -= damage;
        myFighter.hp = Mathf.Round(myFighter.hp * 10.0f) * 0.1f;
        myFighter.hp = Mathf.Max(myFighter.hp, 0);
        myFighter.UpdateUI();
    }
}
