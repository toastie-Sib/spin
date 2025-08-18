using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private HashSet<GameObject> alreadyTriggered = new HashSet<GameObject>();
    public Animator animator;
    public float damage = 1.0f;
    [Header("Type")]
    public bool doNotHurt = false;
    public bool shield = false;


    public virtual void OnTriggerEnter(Collider other)
    {
        Fighter myFighter = GetComponentInParent<Fighter>();
        Fighter otherFighter = other.GetComponent<Fighter>();

        if (other.gameObject.CompareTag("Weapon")) //Parry
        {
            if (myFighter != null)
            {
                myFighter.ReverseDirection();
                myFighter.isInvincible = true;
            }

            if (otherFighter != null)
            {
                otherFighter.ReverseDirection();
                otherFighter.isInvincible = true;
            }
        }

        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (otherFighter.isInvincible == false && doNotHurt == false) {
                otherFighter.HitDetect(damage);
                IncreaseScaling();
                animator.SetTrigger("attack");
            }
            
        }

        if (alreadyTriggered.Contains(other.gameObject)) return;

        alreadyTriggered.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        alreadyTriggered.Remove(other.gameObject);
    }

    public void ShieldGrow(float damage)
    {
        Vector3 scale = transform.localScale;
        scale += new Vector3(0f, ((0.01f) * damage), 0f);

        transform.localScale = scale;
    }

    public virtual void IncreaseScaling(){}
}
