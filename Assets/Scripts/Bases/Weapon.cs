using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private HashSet<GameObject> alreadyTriggered = new HashSet<GameObject>();
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
            Fighter otherParentighter = other.GetComponentInParent<Fighter>();
            Weapon otherWeapon = other.GetComponent<Weapon>();
            if (side == otherWeapon.side) return; //preventing hitting same team
            // Prevent double trigger by comparing instance IDs
            if (GetInstanceID() > otherWeapon.GetInstanceID()) return;

            if (myFighter != null)
            {
                if (axe == false) { myFighter.ReverseDirection(); }
                myFighter.isInvincible = true;

                //Animation Logic
                if (otherParentighter.GetComponent<Bow>() != null || otherWeapon.GetComponent<Shield>() != null)
                {
                    if (myFighter.GetComponent<Bow>() != null || GetComponent<Shield>() != null)
                    {
                        myFighter.ParryAnimation();
                        otherParentighter.ParryAnimation();
                    } else
                    {
                        myFighter.AttackAnimation();
                        otherParentighter.ParryAnimation();
                    }
                } else
                if (myFighter.GetComponent<Bow>() != null || GetComponent<Shield>() != null)
                {
                    myFighter.ParryAnimation();
                    otherParentighter.AttackAnimation();
                } else {
                    if (otherParentighter.direction == 1) { myFighter.AttackAnimation(); } else { myFighter.ParryAnimation(); }
                    if (myFighter.direction == 1) { otherParentighter.ParryAnimation(); } else { otherParentighter.AttackAnimation(); }
                }

            }

            if (otherParentighter != null)
            {
                otherParentighter.ReverseDirection();
                otherParentighter.isInvincible = true;

            }
            TriggerParryImpactFrames();
        }


        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            if (side == otherFighter.isPlayer) return;

            if (scythe == true) {
                if (GetComponent<GlassBall>() != null)
                {
                    GlassBall glassBall = GetComponent<GlassBall>();
                    for (int i = -1; i < glassBall.stacks; i++)
                    {
                        otherFighter.ApplyPoison(); // Call the actual scaling logic 'stacks' times
                    }
                }
                else
                {
                    // If there's no GlassBall, perhaps just apply the single stack effect once?
                    otherFighter.ApplyPoison();
                }
            }
            if (otherFighter.isInvincible == false && doNotHurt == false) {
                myFighter.AttackAnimation();

                Weapon otherWeapon = other.GetComponentInChildren<Weapon>();
                otherFighter.HurtAnimation();

                otherFighter.HitDetect(damage);

                if (other.GetComponent<Turret>() == null)
                {
                    TriggerScaling();
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

    public void TriggerScaling() // call this
    {
        if (GetComponent<GlassBall>() != null)
        {
            GlassBall glassBall = GetComponent<GlassBall>();
            for (int i = -1; i < glassBall.stacks; i++)
            {
                IncreaseScaling(); // Call the actual scaling logic 'stacks' times
            }
        }
        else
        {
            // If there's no GlassBall, perhaps just apply the single stack effect once?
            IncreaseScaling();
        }
    }

    public virtual void IncreaseScaling(){ // do not call this one
        
    }

    public void ShieldGrow(float damage)
    {
        if (GetComponent<GlassBall>() != null)
        {
            GlassBall glassBall = GetComponent<GlassBall>();
            damage *= glassBall.stacks;
        }
        Vector3 scale = transform.localScale;
        scale += new Vector3(0f, ((0.01f) * damage), 0f);

        transform.localScale = scale;
    }
}
