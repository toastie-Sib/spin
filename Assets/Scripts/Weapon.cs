using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private HashSet<Collider> currentContacts = new HashSet<Collider>();

    public float damage = 1.0f;
    [Header("Type")]
    public bool doNotHurt = false;
    public bool shield = false;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon")) //Parry
        {
            Fighter myFighter = GetComponentInParent<Fighter>();
            if (myFighter != null)
                myFighter.ReverseDirection();
            myFighter.isInvincible = true;

            Fighter otherFighter = other.gameObject.GetComponentInParent<Fighter>();
            if (otherFighter != null)
                otherFighter.ReverseDirection();
            otherFighter.isInvincible = true;
        }

        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            Fighter otherFighter = other.GetComponent<Fighter>();
            Fighter myFighter = GetComponentInParent<Fighter>();

            if (otherFighter.isInvincible || doNotHurt == true) return;
            otherFighter.HitDetect(damage);
            myFighter.ReverseDirection();
            IncreaseScaling();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            currentContacts.Remove(other);
        }
    }

    public void ShieldGrow(float damage)
    {
        Vector3 scale = transform.localScale;
        scale += new Vector3(0f, ((0.01f) * damage), 0f);

        transform.localScale = scale;
    }

    public virtual void IncreaseScaling(){}
}
