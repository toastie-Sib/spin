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
    [HideInInspector] public Launcher hpUI;
    [HideInInspector] private Vector3 storedVelocity;
    [HideInInspector] private Vector3 storedAngularVelocity;
    [HideInInspector] private int storedDirection;
    [HideInInspector] public bool isActive = false;
    [HideInInspector] public bool isUnarmed = false;
    [HideInInspector] public bool isPlayer = false;
    [HideInInspector] public static event Action<Fighter> OnFighterDied; // global event
    [HideInInspector] public Animator animationRef;

    [Header("Set then Static Shouldnt need to touch")]
    public AudioClip parry;
    public AudioClip hit;
    public AudioClip click;
    public GameObject weapon;
    public GameObject Animation;
    [HideInInspector] public float invincibilityDuration = 0.2f;

    [Header("Modifyable")]
    public float hp = 100;
    [HideInInspector] public float maxHp = 100;
    public float spinMult = 100f;
    //public float damage = 1.0f;
    //Nudge shit
    [HideInInspector] public float velocityThreshold = 1f;
    [HideInInspector] public float nudgeForce = 3f;
    [HideInInspector] public float nudgeCooldown = 3f;

    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectRenderer = GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;

        if (hpUI != null) { hpUI.hpText.text = Mathf.Round(hp).ToString(); }

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
        }
        else
        {
            GameObject pA = GameObject.Find("EnemyAnim");
            AssignAnimation aA = pA.GetComponent<AssignAnimation>();
            animationRef = aA.stashedAnimation;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (hp > maxHp)
        {
            hp = maxHp;
            hpUI.hpText.text = (hp).ToString();
        }

        if (isActive == false) return;

        if (hp <= 0)
        {
            hpUI.hpText.text = (" ").ToString();
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
        StartCoroutine(GetHit(amount));
    }
    private IEnumerator GetHit(float amount)
    {
        //Take Damage to HP
        hp -= amount;
        hp = Mathf.Max(hp, 0);
        if (hpUI != null) { hpUI.hpText.text = Mathf.Round(hp).ToString(); }

        AudioSource.PlayClipAtPoint(hit, transform.position, 0.8f);
        // Trigger impact frames
        GetComponentInChildren<Renderer>().material.color = Color.white;
        //StartCoroutine(ImpactFrames(0.2f));

        if (hpUI != null) { GameSpeedManager.Instance.PauseForImpact(0.2f); }
        
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<Renderer>().material.color = originalColor;
    }

    //Keep bounce going
    private void OnCollisionEnter(Collision collision)
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
        }

        // Bottom WALL
        //if (collision.gameObject.CompareTag("BottomWall"))
        //{
        //
        //    
        //}
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

    public void ParryAnimation()
    {
        animationRef.SetTrigger("Parry");
    }

    public void HurtAnimation()
    {
        animationRef.SetTrigger("Pain");
    }

    public void AttackAnimation()
    {
        animationRef.SetTrigger("Attack");
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
        hp = Mathf.Max(hp, 0);
        hpUI.hpText.text = Mathf.Round(hp).ToString();
        if (hp == 0)
        {
            hpUI.hpText.text = (" ").ToString();
        }

        AudioSource.PlayClipAtPoint(hit, transform.position, 0.8f);

        GetComponentInChildren<Renderer>().material.color = Color.magenta;


        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<Renderer>().material.color = originalColor;
        yield return new WaitForSeconds(4.8f); // wait 5 seconds for this stack
        ApplyPoison();
    }
}