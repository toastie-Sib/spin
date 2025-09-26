using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Fighter
{
    [Header("Axe")]
    public float refreshInterval = 5f;         // Fire every second
    private float nextRefreshTime = 0.5f;

    public float returnDuration = 1.0f;
    public AnimationCurve returnSpeedCurve;
    private CapsuleCollider capscollider;
    private bool axeSpin = true;

    public override void Update() //Make sure to update with Fighter
    {
        base.Update();

        //Timers for Refresh
        if (Time.time >= nextRefreshTime && rb.useGravity == true && axeSpin == true) //Arrow Refresh
        {
            if (direction == 0) return; // paused
            StartCoroutine(Spin());
            nextRefreshTime = Time.time + refreshInterval;
        }
        
    }

    public override void Start() //Make sure to update with Fighter
    {
        base.Start();
        Weapon myWeapon = GetComponentInChildren<Weapon>();
        capscollider = myWeapon.GetComponent<CapsuleCollider>();
        capscollider.enabled = false;
    }

    public override void IncreaseBaseAtkSpeed()
    {

        for (int i = 0; i < SceneSwitcher.Instance.playerBonusAtkSpd; i++)
        {
            refreshInterval *= 0.90f;
        }
        UpdateDynamicUI("Attack Rate: ", refreshInterval, 3);
    }

    private IEnumerator Spin()
    {
        capscollider.enabled = true;
        float duration = 0.3f; // How long the spin takes
        float timer = 0f;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.identity; // Always finish at Z = 0

        while (timer < duration)
        {
            if (axeSpin == false) break;

            timer += Time.deltaTime;
            float t = timer / duration;

            // Smooth ramp up and ramp down (ease-in-out)
            float curve = Mathf.Sin(t * Mathf.PI); // 0→1→0

            // Apply direction and scale to spin speed
            spinMult = 1200;



            yield return null;
        }

        // Snap rotation exactly to 0 and stop spinning
        transform.rotation = endRot;
        spinMult = 0;
        capscollider.enabled = false;
    }

    public void Berserker()
    {
        axeSpin = false;
        spinMult = 1200;
        StartCoroutine(BerserkerTimer());
    }

    private IEnumerator BerserkerTimer()
    {
        yield return new WaitForSeconds(5f);
        spinMult = 0;
        axeSpin = true;
    }
}