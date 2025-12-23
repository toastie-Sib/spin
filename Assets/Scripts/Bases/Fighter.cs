using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Fighter : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Color originalColor;
    [HideInInspector] public Renderer objectRenderer;
    [HideInInspector] public float lastNudgeTime = -Mathf.Infinity;
    [HideInInspector] public bool isInvincible = false;
    [HideInInspector] public float invincibleUntil = 0f;
    [HideInInspector] public int direction = 1;
    [HideInInspector] public bool freezeFrame = false;
    [HideInInspector] public Launcher UI;
    [HideInInspector] private Vector3 storedVelocity;
    [HideInInspector] private Vector3 storedAngularVelocity;
    [HideInInspector] private int storedDirection;
    [HideInInspector] public bool isActive = false;
    [HideInInspector] public bool isUnarmed = false;
    [HideInInspector] public bool isPlayer = false;
    [HideInInspector] public static event Action<Fighter> OnFighterDied; // global event
    [HideInInspector] public Animator animationRef;
    [HideInInspector] public bool canPlayAnimation = true;

    [HideInInspector] public int bleedStacks = 0;
    [HideInInspector] public int poisonStacks = 0;
    [HideInInspector] public bool poisonLeech = false;

    [Header("Set then Static Shouldnt need to touch")]
    public AudioClip parry;
    public AudioClip hit;
    public AudioClip click;
    public GameObject weapon;
    public GameObject Animation;
     public float invincibilityDuration = 0.2f;

    [Header("Modifyable")]
    public float hp = 100;
    [HideInInspector] public float maxHp = 100;
    public float spinMult = 100f;
    public float bonusDamage = 0.0f;
    //Nudge shit
    [HideInInspector] public float velocityThreshold = 1f;
    [HideInInspector] public float nudgeForce = 3f;
    [HideInInspector] public float nudgeCooldown = 3f;

    // Start is called before the first frame update
    public virtual void Start()
    {
        if (isPlayer == true)
        {
            maxHp = SceneSwitcher.Instance.playerMaxHP;
            hp = SceneSwitcher.Instance.playerCurrentHP;
            bonusDamage = SceneSwitcher.Instance.playerBonusDamage;
            IncreaseBaseAtkSpeed();
        }

        rb = GetComponent<Rigidbody>();
        objectRenderer = GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;

        UpdateUI();

        float myXValue = transform.position.x;
        if (myXValue < 0)
        {
            direction = -1;
        }
        if (myXValue > 0)
        {
            direction = 1;
        }

        if (isPlayer == false) // Assign non player's animation to ES
        {
            GameObject esObject = GameObject.Find("EventSystem");
            SceneSwitcher es = esObject.GetComponent<SceneSwitcher>();
            es.otherAnimPrefab = Animation; 
        }
        StartCoroutine(AssignAnim());
    }

    public IEnumerator AssignAnim()
    {
        yield return new WaitForSeconds(0.01f);
        if (isPlayer == true)
        {
            GameObject pA = GameObject.Find("PlayerAnim");
            AssignAnimation aA = pA.GetComponent<AssignAnimation>();
            animationRef = aA.stashedAnimation;

            AnimationMovement anim = animationRef.GetComponent<AnimationMovement>();
            Vector3 pos = anim.parryPoint.transform.position;
            pos.x = 0;
            anim.parryPoint.transform.position = pos;
            anim.parryPoint.transform.position -= new Vector3(1f, 0, 0);
        }
        else
        {
            if (UI.name == "Other Spawn") {
                GameObject pA = GameObject.Find("EnemyAnim");
                AssignAnimation aA = pA.GetComponent<AssignAnimation>();
                animationRef = aA.stashedAnimation;
            } else if (UI.name == "Other Spawn (1)") {
                GameObject pA = GameObject.Find("EnemyAnimSpare");
                AssignAnimation aA = pA.GetComponent<AssignAnimation>();
                animationRef = aA.stashedAnimation;
            } else {
                GameObject pA = GameObject.Find("EnemyAnimSpare (1)");
                AssignAnimation aA = pA.GetComponent<AssignAnimation>();
                animationRef = aA.stashedAnimation;
            }
            AnimationMovement anim = animationRef.GetComponent<AnimationMovement>();
            Vector3 pos = anim.parryPoint.transform.position;
            pos.x = 0;
            anim.parryPoint.transform.position = pos;
            anim.parryPoint.transform.position += new Vector3(1f, 0, 0);
        }
    }

    public virtual void IncreaseBaseAtkSpeed()
    {
        
        for (int i = 0; i < SceneSwitcher.Instance.playerBonusAtkSpd; i++)
        {
            spinMult += (spinMult * 0.25f);
        }
        UpdateUI();
    }

    public virtual void IncreaseAtkSpd()
    {
        spinMult += (spinMult * 0.1f);
        UpdateUI();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (hp > maxHp)
        {
            hp = maxHp;
            UpdateUI();
        }

        if (isActive == false) return;

        if (hp <= 0)
        {
            if (UI != null) { UI.hpText.text = (" ").ToString(); }
            OnFighterDied?.Invoke(this);
            
            Destroy(gameObject);
        }
        

        float speed = rb.velocity.magnitude;
        Vector3 velocity = rb.velocity;

        transform.Rotate(0f, 0f, spinMult * direction * Time.deltaTime); //Spin

        if (isInvincible && Time.time >= invincibleUntil) //i-frames
        {
            isInvincible = false;
        }

        //Nudge
        bool isMovingSlow = Mathf.Abs(velocity.magnitude) < velocityThreshold;
        bool cooldownPassed = Time.time - lastNudgeTime >= nudgeCooldown;
        if (isMovingSlow && cooldownPassed && (transform.position.x < -5.1f || transform.position.x > 5.1f))
        {
            float xNudge = 0f;

            if (velocity.x > 0)
                xNudge = -nudgeForce;
            else if (velocity.x < 0)
                xNudge = nudgeForce;
            else
                xNudge = (Random.value > 0.5f) ? nudgeForce : -nudgeForce; // Random left/right if stuck

            Vector3 nudge = new Vector3(xNudge, 0f, 0f);

            rb.AddForce(nudge, ForceMode.Impulse);
            lastNudgeTime = Time.time; // Important: Reset cooldown
            Debug.Log("Poke Poke :)");
        }
    }

    //Parry
    public void ReverseDirection()
    {
        if (freezeFrame == false)
        {
            Vector3 velocityBoost = new Vector3(Random.Range(0.5f, 1.5f), Random.Range(1f, 2f), 0f);
            rb.velocity += velocityBoost;
        }

        direction *= -1;

        invincibleUntil = Time.time + invincibilityDuration;

        AudioSource.PlayClipAtPoint(parry, transform.position, 0.5f);

        
    }

    //Ouchy
    public void HitDetect(float amount)
    {
        if (isInvincible) return; // Don't get hurt

        if(GetComponentInChildren<TungstonSphere>() != null && amount > 1)
        {
            TungstonSphere ts = GetComponentInChildren<TungstonSphere>();
            amount -= ts.stacks;
            if (amount < 1) { amount = 1; }
        }
        if (GetComponentInChildren<ShatteredStopwatch>() != null)
        {
            ShatteredStopwatch ss = GetComponentInChildren<ShatteredStopwatch>();
            ss.StartTimer();
        }



        if (GetComponentInChildren<BloodofthePhalanx>() != null)
        {
            BloodofthePhalanx bp = GetComponentInChildren<BloodofthePhalanx>();
            bp.StartDamage(amount);
            StartCoroutine(GetHit(0));
        }
        else
        {
            StartCoroutine(GetHit(amount));
        }
    }
    private IEnumerator GetHit(float amount)
    {
        //Take Damage to HP
        hp -= amount;
        hp = Mathf.Round(hp * 10.0f) * 0.1f;
        hp = Mathf.Max(hp, 0);
        UpdateUI();

        AudioSource.PlayClipAtPoint(hit, transform.position, 0.8f);
        // Trigger impact frames
        GetComponentInChildren<Renderer>().material.color = Color.white;
        //StartCoroutine(ImpactFrames(0.2f));

        if (UI != null) { GameSpeedManager.Instance.PauseForImpact(0.2f); }
        
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<Renderer>().material.color = originalColor;
    }

    public void DealingDamage(float damage, Fighter otherFighter)
    {
        if (GetComponentInChildren<BloodoftheReaper>() != null)
        {
            BloodoftheReaper br = GetComponentInChildren<BloodoftheReaper>();
            br.StartAttack(damage, otherFighter);
            otherFighter.HitDetect(0);
        }
        else
        {

            otherFighter.HitDetect(damage);
        }
    }

    //Keep bounce going
    public virtual void OnCollisionEnter(Collision collision)
    {
        if (click != null) { AudioSource.PlayClipAtPoint(click, transform.position); } //bounce sound :D
        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        //float verticalSpeed = Mathf.Abs(rb.velocity.y);

        // LEFT WALL
        if (collision.gameObject.CompareTag("LeftWall"))
        {

            // If moving toward the wall (x is negative) and slow
            if (horizontalSpeed < 1f)
            {
                Vector3 wallBoost = new Vector3(Random.Range(4f, 7f), Random.Range(-2f, 2f), 0f);
                rb.velocity += wallBoost;
            }

            if (bleedStacks > 0)
            {
                BleedDamage(bleedStacks);
            }
        }

        // RIGHT WALL
        if (collision.gameObject.CompareTag("RightWall"))
        {

            // If moving toward the wall (x is positive) and slow
            if (horizontalSpeed < 1f)
            {
                Vector3 wallBoost = new Vector3(Random.Range(4f, 7f), Random.Range(-2f, 2f), 0f);
                rb.velocity -= wallBoost;
            }

            if (bleedStacks > 0)
            {
                BleedDamage(bleedStacks);
            }
        }

         //Bottom WALL
        if (collision.gameObject.CompareTag("BottomWall"))
        {

            if (bleedStacks > 0)
            {
                BleedDamage(bleedStacks);
            }
        }

        // Top WALL
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (GetComponentInChildren<RaiseTheRoof>() != null)
            {
                RaiseTheRoof raiseTheRoof = GetComponentInChildren<RaiseTheRoof>();
                for (int i = 0; i < raiseTheRoof.stacks; i++)
                {
                    hp += 2;
                    UpdateUI();
                }
            }

            if (bleedStacks > 0)
            {
                BleedDamage(bleedStacks);
            }
        }
    }


    //private IEnumerator ImpactFrames(float freezeDuration)
    //{
    //    if (freezeFrame == false)
    //    {
    //        storedVelocity = rb.velocity;
    //        storedAngularVelocity = rb.angularVelocity;
    //        storedDirection = direction;
    //    }
    //
    //    freezeFrame = true;
    //
    //    //Set to 0
    //    rb.velocity = Vector3.zero;
    //    rb.angularVelocity = Vector3.zero;
    //    rb.useGravity = false;
    //    direction = 0;
    //
    //    yield return new WaitForSecondsRealtime(freezeDuration);
    //
    //    rb.velocity = storedVelocity;
    //    rb.angularVelocity = storedAngularVelocity;
    //    rb.useGravity = true;
    //    direction = storedDirection;
    //    freezeFrame = false;
    //}

    public void UpdateUI()
    {
        if (UI != null) { 
            if(hp <= 0.5){ UI.hpText.text = "1"; } else { UI.hpText.text = Mathf.Round(hp).ToString(); }
            UI.hpUIText.text = ("HP: " + hp).ToString();
            UI.spinText.text = ("Spin: " + (Mathf.Round(spinMult))).ToString();
        }
    }

    public void UpdateDynamicUI(string str, float num, int text)
    {
        if (UI != null)
        {
            if (text == 1) {
                UI.stacksText.text = (str + (Mathf.Round(num))).ToString();
            } else if (text == 2) {
                UI.damageText.text = (str + num).ToString();
                UI.damageText.text = string.Format("{0}{1:F1}", str, num);
            } else if (text == 3) {
                UI.extraText.text = (str + num).ToString();
            }
        }
    }

    public void ParryAnimation()
    {
        if (canPlayAnimation == false) return;
        canPlayAnimation = false; 
        AnimationMovement anim = animationRef.GetComponent<AnimationMovement>();
        anim.autoMove = false;
        animationRef.GetComponent<AnimationMovement>().ParryPoint();
        animationRef.SetTrigger("Parry");
        StartCoroutine(AllowAnimationPlay());
    }

    public void HurtAnimation()
    {
        if (canPlayAnimation == false || animationRef == null) return;
        canPlayAnimation = false; animationRef.GetComponent<AnimationMovement>().autoMove = false;
        animationRef.GetComponent<AnimationMovement>().StartingPoint();
        animationRef.SetTrigger("Pain");
        StartCoroutine(AllowAnimationPlay());
    }

    public virtual void AttackAnimation(Fighter otherFighter)
    {
        UpdateUI();
        if (canPlayAnimation == false || animationRef == null) return;
        canPlayAnimation = false;
        AnimationMovement anim = animationRef.GetComponent<AnimationMovement>();
        anim.autoMove = false;
        if(otherFighter != null && otherFighter.animationRef != null)
        {
            anim.attackPoint.position = otherFighter.animationRef.GetComponent<AnimationMovement>().startingPoint.position;
            if (isPlayer == true) { anim.attackPoint.transform.position -= new Vector3(1.5f, 0, 0); }
            else { anim.attackPoint.transform.position += new Vector3(1.5f, 0, 0); }
        }

        anim.AttackPoint();
        animationRef.SetTrigger("Attack");
        StartCoroutine(AllowAnimationPlay());
    }

    public virtual void AttackOnParryAnimation()
    {
        if (canPlayAnimation == false) return;
        canPlayAnimation = false; animationRef.GetComponent<AnimationMovement>().autoMove = false;
        animationRef.GetComponent<AnimationMovement>().ParryPoint();
        animationRef.SetTrigger("Attack");
        StartCoroutine(AllowAnimationPlay());
    }

    public void DelayedHurtAnimation(float amount)
    {
        if (canPlayAnimation == false) return;
        canPlayAnimation = false; animationRef.GetComponent<AnimationMovement>().autoMove = false;
        StartCoroutine(DelayedHurt(amount));
    }

    public IEnumerator DelayedHurt(float amount)
    {
        yield return new WaitForSeconds(amount);

        animationRef.GetComponent<AnimationMovement>().StartingPoint();
        animationRef.SetTrigger("Pain");
        StartCoroutine(AllowAnimationPlay());
    }

    public IEnumerator AllowAnimationPlay()
    {
        yield return new WaitForSeconds(0.3f);
        canPlayAnimation = true;
        animationRef.GetComponent<AnimationMovement>().autoMove = true;
    }


    //Dagger only
    public void IncreaseSpeed()
    {
        if (spinMult < 0)
        {
            spinMult -= 200;
        }
        if (spinMult > 0)
        {
            spinMult += 200;
        }
    }

    //Scythe only and Poison
    public void ApplyPoison()
    {
        StartCoroutine(PoisonTick());
    }

    public IEnumerator PoisonTick()
    {
        
        //Take Damage to HP
        hp -= 1;
        UpdateUI();

        AudioSource.PlayClipAtPoint(hit, transform.position, 0.8f);

        if (poisonLeech)
        {
            var player = FindObjectOfType<Fighter>();
            if(player != null && player.isPlayer)
            {
                player.hp += 1;
                player.UpdateUI();
            }
        }

        GetComponentInChildren<Renderer>().material.color = Color.magenta;
        if (animationRef != null) { animationRef.GetComponent<SpriteRenderer>().color = Color.magenta; }

        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<Renderer>().material.color = originalColor;
        if (animationRef != null) { animationRef.GetComponent<SpriteRenderer>().color = Color.white; }
        yield return new WaitForSeconds(4.8f); // wait 5 seconds for this stack
        ApplyPoison();
    }

    public void PoisonExplosion()
    {
        if (poisonStacks > 0)
        {
            float totalDamage = poisonStacks * 5f;
            HitDetect(totalDamage);
            poisonStacks = 0; // remove poison stacks
            StopCoroutine(PoisonTick());
        }
    }

    public void BleedDamage(int amount)
    {

        //Take Damage to HP
        hp -= amount;
        UpdateUI();

        AudioSource.PlayClipAtPoint(hit, transform.position, 0.8f);
    }

    public IEnumerator BleedVisual()
    {
        GetComponentInChildren<Renderer>().material.color = Color.red;
        animationRef.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<Renderer>().material.color = originalColor;
        animationRef.GetComponent<SpriteRenderer>().color = Color.white;
    }
}