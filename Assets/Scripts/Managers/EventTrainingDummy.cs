using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTrainingDummy : TrainingDummyFighter
{
    private SceneSwitcher es;
    float damageAmount = 0.0f;
    public Text timerUIText;
    public Text damageUIText;
    float timer = 60.0f;

    public override void Awake()
    {
        base.Awake();

        GameObject esObject = GameObject.Find("EventSystem");
        es = esObject.GetComponent<SceneSwitcher>();
    }

    public override void HitDetect(float amount)
    {
        base.HitDetect(amount);

        damageAmount += amount;

        // Update damage UI: show cumulative damage rounded to nearest tenth with label
        float display = Mathf.Round(damageAmount * 10f) * 0.1f;
        if (damageUIText != null)
        {
            damageUIText.text = string.Format("Damage {0:F1}", display);
        }
    }

    public override void Update()
    {
        base.Update();

        // Countdown timer (start at 60s) with two decimal places
        if (timer > 0f && rb.useGravity == true)
        {
            timer -= Time.deltaTime;
            if (timer < 0f) timer = 0f;
        }

        if (timerUIText != null)
        {
            timerUIText.text = timer.ToString("F2");
        }

        if (timer <= 0f)
        {
            float threshold = es.enemyNodesCompleted * es.chapter * 100;

            if (damageAmount >= (threshold + (threshold / 3)))
            {
                es.SlowLoadSpecificSceneDelay("EventItemRare");
            }
            else if (damageAmount >= (threshold))
            {
                es.SlowLoadSpecificSceneDelay("EventItemUncommon");
            }
            else if (damageAmount >= (threshold - (threshold / 3)))
            {
                es.SlowLoadSpecificSceneDelay("EventItemCommon");
            }
            else { es.SlowLoadSpecificSceneDelay("EventItemNone"); }
            Destroy(gameObject);
        }
    }
}

