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
    public bool axe = false;
    public bool scythe = false;
    [HideInInspector] public bool side;
    [HideInInspector] public Transform firePoint;

    public virtual void Start() {
        Fighter myFighter = GetComponentInParent<Fighter>();
        side = myFighter.isPlayer;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Fighter myFighter = GetComponentInParent<Fighter>();
        Fighter otherFighter = other.GetComponent<Fighter>();

        if (other.gameObject.CompareTag("Weapon")) //Parry
        {
            Weapon otherWeapon = other.GetComponent<Weapon>();
            if (side == otherWeapon.side) return; //preventing hitting same team

            if (myFighter != null)
            {
                if (axe == false) { myFighter.ReverseDirection(); }
                myFighter.isInvincible = true;
                
                
            }

            if (otherFighter != null)
            {
                otherFighter.ReverseDirection();
                otherFighter.isInvincible = true;
                
                
            }
            TriggerParryImpactFrames();
        }


        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (side == otherFighter.isPlayer) return;

            if (scythe == true) { otherFighter.ApplyPoison(); }
            if (otherFighter.isInvincible == false && doNotHurt == false) {
                otherFighter.HitDetect(damage);
                //animator.SetTrigger("attack");

                if (other.GetComponent<Turret>() == null)
                {
                    IncreaseScaling();
                }
            }
            
        }

        if (alreadyTriggered.Contains(other.gameObject)) return;

        alreadyTriggered.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        alreadyTriggered.Remove(other.gameObject);
    }

    public virtual void TriggerParryImpactFrames() 
    {
        GameSpeedManager.Instance.PauseForImpact(0.2f);
    }

    public void ShieldGrow(float damage)
    {
        Vector3 scale = transform.localScale;
        scale += new Vector3(0f, ((0.01f) * damage), 0f);

        transform.localScale = scale;
    }

    public virtual void IncreaseScaling(){}
}
