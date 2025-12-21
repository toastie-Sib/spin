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
        {
            ItemCheck();
        }

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

        if (GetComponent<BloodoftheBandit>() != null)
        {
            BloodoftheBandit botB = GetComponent<BloodoftheBandit>();
            for (int i = 0; i < botB.stacks; i++)
            {
                if (botB.applied < 25* botB.stacks) { bounceBonus += 0.15f; }
            }
        }
        if (GetComponent<BloodoftheMage>() != null)
        {
            BloodoftheMage botM = GetComponent<BloodoftheMage>();
            botM.hitsDone += 1; 
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


    public void ItemCheck() // Update on Unarmed too since no weapon
    {
        if (SceneSwitcher.Instance.HasItem("BloodoftheArcher"))
        {
            var botA = gameObject.AddComponent<BloodoftheArcher>();
            botA.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheArcher");
        }

        if (SceneSwitcher.Instance.HasItem("BloodoftheBandit"))
        {
            var botB = gameObject.AddComponent<BloodoftheBandit>();
            botB.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheBandit");
        }

        if (SceneSwitcher.Instance.HasItem("BloodoftheKnight"))
        {
            var botK = gameObject.AddComponent<BloodoftheKnight>();
            botK.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheKnight");
        }

        if (SceneSwitcher.Instance.HasItem("BloodoftheSoldier"))
        {
            var botS = gameObject.AddComponent<BloodoftheSoldier>();
            botS.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheSoldier");
        }

        if (SceneSwitcher.Instance.HasItem("Food"))
        {
            var food = gameObject.AddComponent<Food>();
            food.stacks = SceneSwitcher.Instance.GetItemCount("Food");
        }

        if (SceneSwitcher.Instance.HasItem("GlassBall"))
        {
            var glassBall = gameObject.AddComponent<GlassBall>();
            glassBall.stacks = SceneSwitcher.Instance.GetItemCount("GlassBall");
        }

        if (SceneSwitcher.Instance.HasItem("RaiseTheRoof"))
        {
            var raisetheRoof = gameObject.AddComponent<RaiseTheRoof>();
            raisetheRoof.stacks = SceneSwitcher.Instance.GetItemCount("RaiseTheRoof");
        }

        if (SceneSwitcher.Instance.HasItem("Training"))
        {
            var training = gameObject.AddComponent<Training>();
            training.stacks = SceneSwitcher.Instance.GetItemCount("Training");
        }

        if (SceneSwitcher.Instance.HasItem("TriTippedDagger"))
        {
            var ttD = gameObject.AddComponent<TriTippedDagger>();
            ttD.stacks = SceneSwitcher.Instance.GetItemCount("TriTippedDagger");
        }
        if (SceneSwitcher.Instance.HasItem("BloodoftheMage"))
        {
            var bloodoftheMage = gameObject.AddComponent<BloodoftheMage>();
            bloodoftheMage.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheMage");
        }

        if (SceneSwitcher.Instance.HasItem("GatitoBlade"))
        {
            var gatitoBlade = gameObject.AddComponent<GatitoBlade>();
            gatitoBlade.stacks = SceneSwitcher.Instance.GetItemCount("GatitoBlade");
        }
    }
}