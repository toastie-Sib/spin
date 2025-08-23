using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private HashSet<GameObject> alreadyTriggered = new HashSet<GameObject>();
    public Animator animationRef;
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

        StartCoroutine(AssignAnim());
    }

    public IEnumerator AssignAnim()
    {
        yield return new WaitForSeconds(0.01f);
        if (side == true)
        {
            GameObject pA = GameObject.Find("PlayerAnim");
            AssignAnimation aA = pA.GetComponent<AssignAnimation>();
            animationRef = aA.stashedAnimation;
        }
        else
        {
            GameObject pA = GameObject.Find("EnemyAnim");
            AssignAnimation aA = pA.GetComponent<AssignAnimation>();
            animationRef = aA.stashedAnimation;
        }
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

                animationRef.SetTrigger("Parry");
            }

            if (otherFighter != null)
            {
                otherFighter.ReverseDirection();
                otherFighter.isInvincible = true;

                otherWeapon.animationRef.SetTrigger("Parry");
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
                animationRef.SetTrigger("Attack");

                Weapon otherWeapon = other.GetComponentInChildren<Weapon>();
                otherWeapon.animationRef.SetTrigger("Pain");

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

    public virtual void IncreaseScaling(){
        
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
