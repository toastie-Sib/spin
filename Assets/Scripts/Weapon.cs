using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private HashSet<Collider> currentContacts = new HashSet<Collider>();

    public float damage = 1.0f;

    public bool sword = false;
    public bool dagger = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            // Prevent both sides from triggering — only do it on the one with lower ID
            if (gameObject.GetInstanceID() < other.gameObject.GetInstanceID())
            {

                Fighter myFighter = GetComponentInParent<Fighter>();
                if (myFighter != null)
                    myFighter.ReverseDirection();

                Fighter otherFighter = other.gameObject.GetComponentInParent<Fighter>();
                if (otherFighter != null)
                    otherFighter.ReverseDirection();
            }
        }

        if (other.gameObject.CompareTag("Fighter"))
        {
            Fighter otherFighter = other.gameObject.GetComponentInParent<Fighter>();
            if (otherFighter.isInvincible) return;
                otherFighter.HitDetect(damage);
                if (sword == true)
                {
                    damage += 1;
                }
                if (dagger == true)
                {
                    Fighter myFighter = GetComponentInParent<Fighter>();
                    myFighter.IncreaseSpeed();
                }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            currentContacts.Remove(other);
        }
    }
}
