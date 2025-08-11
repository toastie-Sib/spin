using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unarmed : Fighter
{
    private void OnCollisionEnter(Collision collision)
    {
        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        float verticalSpeed = Mathf.Abs(rb.velocity.y);

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
            if (otherFighter.isInvincible) return;
            otherFighter.HitDetect(Mathf.RoundToInt(Mathf.Abs((rb.velocity.magnitude / 5))));
        }

        // shield v unarmed
        if (collision.gameObject.CompareTag("Weapon"))
        {
            Weapon other = collision.gameObject.GetComponentInParent<Weapon>();
            if (other.shield == false) return;
            float damage = Mathf.RoundToInt(Mathf.Abs((rb.velocity.magnitude / 5))); // Same formula as above
            HitDetect(damage); 

            other.ShieldGrow(damage);
        }
    }

}