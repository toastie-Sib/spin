using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private HashSet<Collider> currentContacts = new HashSet<Collider>();
    private bool active = false;
    public Bow shooter;
    public AudioClip parry;
    public float damage = 1.0f;
    public float speed = 10.0f;

    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
     
        
        if (other.gameObject.CompareTag("Weapon"))
        {
            AudioSource.PlayClipAtPoint(parry, transform.position);
            Destroy(gameObject);
        
        }
        
        if (other.gameObject.CompareTag("Fighter"))
        {
            Fighter otherFighter = other.GetComponent<Fighter>();
        
            if (otherFighter.isInvincible) return;
            otherFighter.HitDetect(damage);

            shooter.IncreaseFireRate();

            Destroy(gameObject);
        }

        // LEFT WALL
        if (other.gameObject.CompareTag("LeftWall"))
        {
            Destroy(gameObject);
        }

        // RIGHT WALL
        if (other.gameObject.CompareTag("RightWall"))
        {
            Destroy(gameObject);
        }

        // Bottom WALL
        if (other.gameObject.CompareTag("BottomWall"))
        {
            Destroy(gameObject);
        }

        // Top WALL
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (active == false)
        {
            active = true;
        }

        if (other.gameObject.CompareTag("Weapon"))
        {
            currentContacts.Remove(other);
        }
    }
}
