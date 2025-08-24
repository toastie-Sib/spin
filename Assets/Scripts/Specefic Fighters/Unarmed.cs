using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unarmed : Fighter
{

    public override void Start() //Make sure to update with Fighter
    {
        base.Start();
        isUnarmed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        
        //Keep bounce going 
        // LEFT WALL
        if (collision.gameObject.CompareTag("LeftWall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity += wallBoost;
        }
        // RIGHT WALL
        if (collision.gameObject.CompareTag("RightWall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity -= wallBoost;
        }
        // Bottom WALL
        if (collision.gameObject.CompareTag("BottomWall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity += wallBoost;
        }
        // Top WALL
        if (collision.gameObject.CompareTag("Wall"))
        {

            Vector3 wallBoost = new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(0.7f, 0.7f), 0f);
            rb.velocity -= wallBoost;
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
            rb.velocity -= wallBoost;
            //rb.velocity -= rb.velocity * 2;
        }

        
    }

    public void Damage(Fighter otherFighter)
    {
        otherFighter.HitDetect(Mathf.RoundToInt(Mathf.Abs((rb.velocity.magnitude / 5))));
        AttackAnimation();
        otherFighter.HurtAnimation();
    }

}