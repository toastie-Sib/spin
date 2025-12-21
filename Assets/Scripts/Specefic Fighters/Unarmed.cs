using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unarmed : Fighter
{
    private int glassBallStacks = 1;
    private float bounceBonus = 0;
    public override void Start() //Make sure to update with Fighter
    {
        base.Start();
        isUnarmed = true;
        if (isPlayer == true)

        UpdateDynamicUI("Speed: ", 0, 1);
        UpdateDynamicUI("Damage: ", 0, 2);

        if (GetComponent<GlassBall>() != null)
        {
            GlassBall glassBall = GetComponent<GlassBall>();
            for (int i = 1; i < glassBall.stacks; i++)
            {
                glassBallStacks += 1;
            }
        }
    }

    public override void Update()
    {
        base.Update();
        UpdateDynamicUI("Speed: ", Mathf.Abs((rb.velocity.magnitude)), 1);
        UpdateDynamicUI("Damage: ", glassBallStacks*(bonusDamage + (Mathf.RoundToInt(Mathf.Abs((rb.velocity.magnitude / 5))))), 2); // Doesn't account Items (GB BotK)
    }

    public override void OnCollisionEnter(Collision collision)
    {
        
        
        //Keep bounce going 
        // LEFT WALL
        if (collision.gameObject.CompareTag("LeftWall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity += wallBoost * (1+ bounceBonus + (SceneSwitcher.Instance.playerBonusAtkSpd * 0.25f));

            if (bleedStacks > 0)
            {
                BleedDamage(bleedStacks);
            }
        }
        // RIGHT WALL
        if (collision.gameObject.CompareTag("RightWall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity -= wallBoost * (1+ bounceBonus + (SceneSwitcher.Instance.playerBonusAtkSpd * 0.25f));

            if (bleedStacks > 0)
            {
                BleedDamage(bleedStacks);
            }
        }
        // Bottom WALL
        if (collision.gameObject.CompareTag("BottomWall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity += wallBoost * (1+ bounceBonus + (SceneSwitcher.Instance.playerBonusAtkSpd * 0.25f));

            if (bleedStacks > 0)
            {
                BleedDamage(bleedStacks);
            }
        }
        // Top WALL
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (GetComponent<RaiseTheRoof>() != null)
            {
                RaiseTheRoof raiseTheRoof = GetComponent<RaiseTheRoof>();
                for (int i = 0; i < raiseTheRoof.stacks; i++)
                {
                    hp += 2;
                    UpdateUI();
                }
            }
            Vector3 wallBoost = new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity -= wallBoost * (1+ bounceBonus + (SceneSwitcher.Instance.playerBonusAtkSpd * 0.25f));

            if (bleedStacks > 0)
            {
                BleedDamage(bleedStacks);
            }
        }
        
        //attack
        if (collision.gameObject.CompareTag("Fighter"))
        {
            
            Fighter otherFighter = collision.gameObject.GetComponentInParent<Fighter>();
            if (otherFighter.isInvincible == false) {
                if (GetComponent<GlassBall>() != null)
                {
                    GlassBall glassBall = GetComponent<GlassBall>();
                    for (int i = -1; i < glassBall.stacks; i++)
                    {
                        Damage(otherFighter);
                    }
                }
                else
                {
                    // If there's no GlassBall, perhaps just apply the single stack effect once?
                    Damage(otherFighter);
                }
            }
            Vector3 wallBoost = new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity -= wallBoost * (1+ bounceBonus + (SceneSwitcher.Instance.playerBonusAtkSpd * 0.25f));
        }

        
    }

    public void Damage(Fighter otherFighter)
    {
        otherFighter.HitDetect(bonusDamage + (Mathf.RoundToInt(Mathf.Abs((rb.velocity.magnitude / 5)))));
        AttackAnimation(otherFighter);
        otherFighter.HurtAnimation();

        // Item Effects on Hit
        if (GetComponent<BloodoftheBandit>() != null)
        {
            BloodoftheBandit botB = GetComponent<BloodoftheBandit>();
            for (int i = 0; i < botB.stacks; i++)
            {
                if (botB.applied < 25* botB.stacks) { bounceBonus += 0.15f; }
            }
        }
        if (GetComponent<TriTippedDagger>() != null)
        {
            TriTippedDagger ttD = GetComponent<TriTippedDagger>();
            ttD.Effect(otherFighter);
        }
        if (GetComponent<BloodoftheMage>() != null)
        {
            BloodoftheMage botM = GetComponent<BloodoftheMage>();
            botM.hitsDone += 1;
            if (botM.hitsDone >= 6 - botM.stacks)
            {
                botM.hitsDone = 0;
                otherFighter.HitDetect(bonusDamage + (Mathf.RoundToInt(Mathf.Abs((rb.velocity.magnitude / 5)))));
                botM.ExplosionEffect(otherFighter);
                // Direction AWAY from explosion
                Vector3 center = (transform.position + otherFighter.transform.position) * 0.5f;
                Vector3 otherDir = (otherFighter.transform.position - center).normalized;
                Vector3 myDir = (transform.position - center).normalized;
                Rigidbody myRb = GetComponent<Rigidbody>();
                Rigidbody otherRb = otherFighter.GetComponent<Rigidbody>();
                otherRb.AddForce(otherDir * botM.stacks * 20, ForceMode.Impulse);
                myRb.AddForce(myDir * botM.stacks * 20 * 0.75f, ForceMode.Impulse);
            }
        }

    }

    public void BloodSacrifice()
    {
        if (rb == null) return;

        // Sacrifice some HP first
        hp -= 5;
        UpdateUI();

        // Use current velocity direction
        Vector3 direction = rb.velocity.normalized;
        if (direction == Vector3.zero)
        {
            // if not moving, just dash upward
            direction = Vector3.up;
        }

        // Apply velocity boost
        rb.velocity += direction * 6;

       
    }


}