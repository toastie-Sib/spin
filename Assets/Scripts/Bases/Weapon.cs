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
    private bool firstHitDone = false;
    [HideInInspector] public bool side;
    public Transform firePoint;
    [HideInInspector] public Fighter myFighter;
    [HideInInspector] public bool gatitoBlade = false;
    private int tracker = 0;

    public virtual void Start() {
        myFighter = GetComponentInParent<Fighter>();
        side = myFighter.isPlayer;

        
        if (myFighter.isPlayer == true && doNotHurt == false)
        {
            damage += myFighter.bonusDamage;
        }

    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Fighter myFighter = GetComponentInParent<Fighter>();
        Fighter otherFighter = other.GetComponent<Fighter>();

        if (other.gameObject.CompareTag("Weapon")) //Parry
        {
            Fighter otherParentighter = other.GetComponentInParent<Fighter>();
            Weapon otherWeapon = other.GetComponent<Weapon>();
            
            if (otherWeapon == null || side == otherWeapon.side) return; //preventing hitting same team
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
                        myFighter.AttackOnParryAnimation();
                        otherParentighter.ParryAnimation();
                    }
                } else
                if (myFighter.GetComponent<Bow>() != null || GetComponent<Shield>() != null)
                {
                    myFighter.ParryAnimation();
                    otherParentighter.AttackOnParryAnimation();
                } else {
                    if (otherParentighter.direction == 1) { myFighter.AttackOnParryAnimation(); } else { myFighter.ParryAnimation(); }
                    if (myFighter.direction == 1) { otherParentighter.ParryAnimation(); } else { otherParentighter.AttackOnParryAnimation(); }
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

            if (scythe == true) { // Scythe stuff
                if (GetComponent<Training>() != null && firstHitDone == false)
                {
                    Training training = GetComponent<Training>();
                    for (int i = 0; i < training.stacks; i++)
                    {
                        ScytheApply(otherFighter);
                    }
                    firstHitDone = true;
                }
                if (GetComponent<GlassBall>() != null)
                {
                    GlassBall glassBall = GetComponent<GlassBall>();
                    for (int i = -1; i < glassBall.stacks; i++)
                    {
                        ScytheApply(otherFighter);
                    }
                }
                else
                {
                    ScytheApply(otherFighter);
                }
            } // End Scythe BullShit that I should really get rid of and move
            if (otherFighter.isInvincible == false && doNotHurt == false) {
                myFighter.AttackAnimation(otherFighter);

                otherFighter.HurtAnimation();

                myFighter.DealingDamage(damage, otherFighter);


                if (other.GetComponent<Turret>() == null && GetComponentInParent<Anchor>() == null)
                {
                    TriggerScaling();
                }

                if (GetComponent<ShatteredStopwatch>() != null)
                {
                    ShatteredStopwatch ss = GetComponent<ShatteredStopwatch>();
                    ss.StartTimer();
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

    public virtual void IncreaseScaling(){ // do not call this
        
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

    public void ScytheApply(Fighter otherFighter)
    {
        otherFighter.ApplyPoison();
        otherFighter.poisonStacks += 1;
        tracker += 1;
        if (gatitoBlade == false)
        {
            myFighter.UpdateDynamicUI("Poison: ", tracker, 1);
            myFighter.UpdateDynamicUI("Damage: ", damage, 2);
        }
    }

    
    
}
