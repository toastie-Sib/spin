using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBallFighter : Fighter
{
    private float bounceBonus = 1.0f;

    public void Awake()
    {
        isInvincible = true;

        rb = GetComponentInParent<Rigidbody>();
    }

    public override void Start()
    {

    }

    public override void HitDetect(float amount)
    {
    }

    public override void HurtAnimation()
    {
    }
    public override void DelayedHurtAnimation(float amount)
    {
    }
    public override void ApplyPoison()
    {
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        //Keep bounce going 
        // LEFT WALL
        if (collision.gameObject.CompareTag("LeftWall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity += wallBoost * (1 + bounceBonus + (SceneSwitcher.Instance.playerBonusAtkSpd * 0.25f));

        }
        // RIGHT WALL
        if (collision.gameObject.CompareTag("RightWall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity -= wallBoost * (1 + bounceBonus + (SceneSwitcher.Instance.playerBonusAtkSpd * 0.25f));

        }
        // Bottom WALL
        if (collision.gameObject.CompareTag("BottomWall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity += wallBoost * (1 + bounceBonus + (SceneSwitcher.Instance.playerBonusAtkSpd * 0.25f));

        }
        // Top WALL
        if (collision.gameObject.CompareTag("Wall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity -= wallBoost * (1 + bounceBonus + (SceneSwitcher.Instance.playerBonusAtkSpd * 0.25f));

        }
    }
}
