using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandStrong : ItemBase
{
    Fighter myFighter;
    private float lastCheckedChunk;
    private float HPtillActivate = 10f;

    public override void Start()
    {
        base.Start();
        myFighter = GetComponentInParent<Fighter>();

        // Initialize to current HP chunk
        lastCheckedChunk = myFighter.hp;
    }

    public void Update()
    {
        float currentChunk = myFighter.hp;

        if (currentChunk < lastCheckedChunk)
        {
            float change = lastCheckedChunk - currentChunk;
            HPtillActivate -= change;
        }

        lastCheckedChunk = currentChunk;

        if (HPtillActivate <= 0f)
        {
            HPtillActivate = HPtillActivate - 10f;
            int triggers = Mathf.RoundToInt(Mathf.Abs(HPtillActivate) / 10f);

            for (int i = 0; i < triggers; i++)
            {
                OnLostTenHP();
                HPtillActivate += 10f;
            }

            HPtillActivate = 10f + HPtillActivate;
        }
    }

    void OnLostTenHP()
    {
        for (int i = 0; i < stacks; i++)
        {
            GetComponentInParent<Fighter>().IncreaseAtkSpd();
        }
    }
}
