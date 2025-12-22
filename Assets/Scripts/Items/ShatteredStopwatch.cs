using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatteredStopwatch : ItemBase
{
    public float duration = 5f;

    private float timer = 0f;
    private bool running = false;

    void Update()
    {
        if (!running) return;

        timer += Time.deltaTime;

        if (timer >= duration)
        {
            running = false;
            OnTimerFinished();
        }
    }

    public void StartTimer()
    {
        timer = 0f;
        running = true;
    }

    public void StopTimer()
    {
        running = false;
    }

    void OnTimerFinished()
    {
        for (int i = 0; i < stacks; i++)
        {
            GetComponent<Weapon>().TriggerScaling();
        }
        StartTimer();
    }
}
