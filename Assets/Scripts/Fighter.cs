using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [Header("Unmodifyable or Dynamic Do not Touch")]
    public Rigidbody rb;
    public float lastNudgeTime = -Mathf.Infinity;
    public bool isInvincible = false;
    public float invincibleUntil = 0f;
    public int direction = 1;

    [Header("Set then Static Shouldnt need to touch")]
    public AudioClip parry;
    public AudioClip hit;
    public Launcher hpUI;
    public float invincibilityDuration = 0.1f;

    [Header("Modifyable")]
    public float hp = 100;
    public float spinMult = 10f;
    //Nudge shit
    public float velocityThreshold = 1f;
    public float nudgeForce = 5f;
    public float nudgeCooldown = 3f; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        float myXValue = transform.position.x;
        if (myXValue < 0)
        {
            direction = 1;
        }
        if (myXValue > 0)
        {
            direction = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
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
        Vector3 velocityBoost = new Vector3(Random.Range(0.5f, 1.5f), Random.Range(1f, 2f), 0f);
        rb.velocity += velocityBoost;
        direction *= -1;

        isInvincible = true;
        invincibleUntil = Time.time + invincibilityDuration;

        AudioSource.PlayClipAtPoint(parry, transform.position);

        // Trigger impact frames
        ImpactPause.Instance.PauseForImpact(0.05f);
    }

    //Ouchy
    public void HitDetect(float amount)
    {
        if (isInvincible) return; // Don't get hurt

        AudioSource.PlayClipAtPoint(hit, transform.position);
        hp -= amount;
        hp = Mathf.Max(hp, 0);
        hpUI.hpText.text = hp.ToString();
        if (hp == 0)
        {
            hpUI.hpText.text = (" ").ToString();
        }

        //How tf do you get this change to go RAHHHHHHHHHHHHHHHHHHHHHHH FIXXXXXXXXXXXXXXXXXXX
        GetComponentInChildren<Renderer>().material.color = Color.white;
        // Trigger impact frames
        ImpactPause.Instance.PauseForImpact(0.2f);
        GetComponentInChildren<Renderer>().material.color = Color.cyan;
    }

    //Dagger
    public void IncreaseSpeed()
    {
        if (spinMult < 0)
        {
            spinMult -= 250f;
        }
        if (spinMult > 0)
        {
            spinMult += 250f;
        }
    }

    //Keep bounce going
    private void OnCollisionEnter(Collision collision)
    {
        float horizontalSpeed = Mathf.Abs(rb.velocity.x);
        float verticalSpeed = Mathf.Abs(rb.velocity.y);

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

}