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

        //Item Check
        if (myFighter.isPlayer == true && gatitoBlade == false)
        {
            ItemCheck();

            if (doNotHurt == false)
            {
                damage += myFighter.bonusDamage;

            }
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
                        otherFighter.ApplyPoison();
                        tracker += 1;
                        if (gatitoBlade == false)
                        {
                            myFighter.UpdateDynamicUI("Poison: ", tracker, 1);
                            myFighter.UpdateDynamicUI("Damage: ", damage, 2);
                        }
                        
                    }
                    firstHitDone = true;
                }
                if (GetComponent<GlassBall>() != null)
                {
                    GlassBall glassBall = GetComponent<GlassBall>();
                    for (int i = -1; i < glassBall.stacks; i++)
                    {
                        otherFighter.ApplyPoison(); // Call the actual scaling logic 'stacks' times
                        tracker += 1;
                        if (gatitoBlade == false)
                        {
                            myFighter.UpdateDynamicUI("Poison: ", tracker, 1);
                            myFighter.UpdateDynamicUI("Damage: ", damage, 2);
                        }
                    }
                }
                else
                {
                    otherFighter.ApplyPoison(); // Regular Sytche apply Poison Scaling Alternative
                    tracker += 1;
                    if (gatitoBlade == false)
                    {
                        myFighter.UpdateDynamicUI("Poison: ", tracker, 1);
                        myFighter.UpdateDynamicUI("Damage: ", damage, 2);
                    }
                }
            }
            if (otherFighter.isInvincible == false && doNotHurt == false) {
                myFighter.AttackAnimation();

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

    public void ItemCheck() // Update on Unarmed too since no weapon
    {
        if (SceneSwitcher.Instance.HasItem("BloodoftheArcher"))
        {
            var botA = gameObject.AddComponent<BloodoftheArcher>();
            botA.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheArcher");
        }

        if (SceneSwitcher.Instance.HasItem("BloodoftheBandit"))
        {
            var botB = gameObject.AddComponent<BloodoftheBandit>();
            botB.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheBandit");
        }

        if (SceneSwitcher.Instance.HasItem("BloodoftheKnight"))
        {
            var botK = gameObject.AddComponent<BloodoftheKnight>();
            botK.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheKnight");
        }

        if (SceneSwitcher.Instance.HasItem("BloodoftheSoldier"))
        {
            var botS = gameObject.AddComponent<BloodoftheSoldier>();
            botS.stacks = SceneSwitcher.Instance.GetItemCount("BloodoftheSoldier");
        }

        if (SceneSwitcher.Instance.HasItem("Food"))
        {
            var food = gameObject.AddComponent<Food>();
            food.stacks = SceneSwitcher.Instance.GetItemCount("Food");
        }

        if (SceneSwitcher.Instance.HasItem("GlassBall"))
        {
            var glassBall = gameObject.AddComponent<GlassBall>();
            glassBall.stacks = SceneSwitcher.Instance.GetItemCount("GlassBall");
        }

        if (SceneSwitcher.Instance.HasItem("RaiseTheRoof"))
        {
            var raisetheRoof = gameObject.AddComponent<RaiseTheRoof>();
            raisetheRoof.stacks = SceneSwitcher.Instance.GetItemCount("RaiseTheRoof");
        }

        if (SceneSwitcher.Instance.HasItem("Training"))
        {
            var training = gameObject.AddComponent<Training>();
            training.stacks = SceneSwitcher.Instance.GetItemCount("Training");
        }

        if (SceneSwitcher.Instance.HasItem("TriTippedDagger"))
        {
            var ttD = gameObject.AddComponent<TriTippedDagger>();
            ttD.stacks = SceneSwitcher.Instance.GetItemCount("TriTippedDagger");
        }

        if (SceneSwitcher.Instance.HasItem("GatitoBlade"))
        {
            var gatitoBlade = gameObject.AddComponent<GatitoBlade>();
            gatitoBlade.stacks = SceneSwitcher.Instance.GetItemCount("GatitoBlade");
        }
    }
}
