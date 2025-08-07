using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    private Rigidbody rb;
    private int direction = 1;
    private float lastNudgeTime = -Mathf.Infinity;

    public float spinMult = 10f;
    public float velocityThreshold = 1f;
    public float nudgeForce = 5f;
    public float nudgeCooldown = 3f; 

    public AudioClip parry;
    public AudioClip hit;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //Vector3 velocityBoost = new Vector3(Random.Range(5f, 8f), Random.Range(4f, 6f), 0f);
        Vector3 velocityBoost = new Vector3(Random.Range(100f, 100f), Random.Range(100f, 100f), 0f);
        rb.velocity += velocityBoost;

    }

    // Update is called once per frame
    void Update()
    {
        float speed = rb.velocity.magnitude;

        transform.Rotate(0f, 0f, spinMult * direction * Time.deltaTime);

        Vector3 velocity = rb.velocity;

        bool isMovingSlow = velocity.magnitude < velocityThreshold && velocity.magnitude >= 0.0f;
        bool cooldownPassed = Time.time - lastNudgeTime >= nudgeCooldown;


        if (isMovingSlow && cooldownPassed && (transform.position.x < -4.3f || transform.position.x > 4.3f))
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
        Vector3 velocityBoost = new Vector3(Random.Range(1f, 3f), Random.Range(2f, 4f), 0f);
        rb.velocity += velocityBoost;
        direction *= -1;
        AudioSource.PlayClipAtPoint(parry, transform.position);
    }

    //Ouchy
    public void HitDetect()
    {
        AudioSource.PlayClipAtPoint(hit, transform.position);
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
            if (rb.velocity.x < -1 && horizontalSpeed < 15f)
            {
                Vector3 wallBoost = new Vector3(Random.Range(4f, 7f), Random.Range(-2f, 2f), 0f);
                rb.velocity += wallBoost;
            }
        }

        // RIGHT WALL
        if (collision.gameObject.CompareTag("RightWall"))
        {

            // If moving toward the wall (x is positive) and slow
            if (rb.velocity.x > -1 && horizontalSpeed < 15f)
            {
                Vector3 wallBoost = new Vector3(Random.Range(4f, 7f), Random.Range(-2f, 2f), 0f);
                rb.velocity -= wallBoost;
            }
        }

        // Bottom WALL
        if (collision.gameObject.CompareTag("BottomWall"))
        {

            // If moving toward the wall (x is positive) and slow
            if (rb.velocity.y < -1 && horizontalSpeed < 10f)
            {
                Vector3 wallBoost = new Vector3(Random.Range(-2f, 2f), Random.Range(3f, 5f), 0f);
                rb.velocity += wallBoost;
                Debug.Log("Bot help");
            }
        }
    }

}