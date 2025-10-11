using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeRamFighter : Fighter
{
    [Header("Movement Settings")]
    public float acceleration = 10f;   // How quickly to speed up
    private float maxSpeed = 50f;       // Top speed
    public float refreshInterval = 4f;

    private float currentSpeed = 0f;
    private Vector3 moveDirection;
    private float storedSpin;
    private bool isCharging = false;

    private float nextRefreshTime;

    public bool canCharge = false;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        moveDirection = transform.up;

        if (isCharging)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);

            if (spinMult != 0)
            {
                storedSpin = spinMult;
                spinMult = 0;
            }

            rb.useGravity = false;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(0,0,0);

            if (spinMult == 0)
            {
                spinMult = storedSpin;
                rb.useGravity = true;
            }
        }

        // Auto-trigger charge every few seconds
        if (Time.time >= nextRefreshTime && rb.useGravity == true && canCharge == true)
        {
            isCharging = true;
            nextRefreshTime = Time.time + refreshInterval;
        }
    }

    void FixedUpdate()
    {
        if (isCharging)
            rb.velocity = moveDirection * currentSpeed;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (isCharging)
        {

            // Stop charging if you want (optional)
            isCharging = false;
        }
    }

    public void ShieldCharge()
    {
        isCharging = true;
    }

}
