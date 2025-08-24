using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Fighter shooter; // This one auto assigned
    [HideInInspector] private Weapon weapon;
    [HideInInspector] public Weapon reflector;
    [HideInInspector] public bool reflected = false;
    [HideInInspector] public bool explosionDone = false;
    [HideInInspector] public bool side;


    [Header("Values")]
    public AudioClip parry; //This one can be assigned in 
    public float damage = 1.0f;
    public float speed = 10.0f;

    public virtual void Start()
    {
        if (shooter != null) { weapon = shooter.GetComponentInChildren<Weapon>(); }
        
    }


    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Fighter otherFighter = other.GetComponent<Fighter>();
        Weapon otherWeapon = other.gameObject.GetComponentInParent<Weapon>();

        if (other.gameObject.CompareTag("Weapon"))
        {
            if (side == otherWeapon.side) return; //preventing hitting same team
            if (otherWeapon == weapon /*&& reflected == false*/) return; //avoid hitting bow's weapon

            AudioSource.PlayClipAtPoint(parry, transform.position);
            Fighter otherParentFighter = other.GetComponentInParent<Fighter>();
            
            otherParentFighter.ParryAnimation(); // Maybe should be delayed?
            shooter.AttackAnimation();

            //Destroy Game object if it isn't a shield or reflected
            if (otherWeapon.shield == false) {  if (reflected == false) { DestroySelf(); } } 
            else //If it is Parried by Shield go to shooter
            {
                if (explosionDone == false) { if (shooter == null) { DestroySelf(); } else {
                        // Get target's rigidbody to read velocity
                        Rigidbody targetRb = shooter.GetComponent<Rigidbody>();

                        // Current shooter position
                        Vector3 shooterPos = shooter.transform.position;
                        Vector3 shooterVel = Vector3.zero;

                        if (targetRb != null)
                            shooterVel = targetRb.velocity;

                        // Solve intercept point
                        Vector3 interceptPoint = FirstOrderIntercept(
                            transform.position,
                            Vector3.zero,        // arrow velocity handled below
                            shooterPos,
                            shooterVel
                        );

                        // Aim the arrow toward the intercept point
                        Vector3 dirToShooter = (interceptPoint - transform.position).normalized;
                        transform.up = dirToShooter;
                        reflected = true;
                        reflector = otherWeapon;
                        side = otherWeapon.side;
                    }
                }
            }
        
        }
        
        if (other.gameObject.CompareTag("Fighter"))
        {

            if (side == otherFighter.isPlayer) return;
            if (otherFighter.isInvincible) return;
            if (otherFighter == shooter && reflected == false) return;
            if (reflected == true)
            {
                Fighter reflectorSheild = reflector.GetComponentInParent<Fighter>();
                if (otherFighter == reflectorSheild) return;
            }

            shooter.AttackAnimation();
            otherFighter.HitDetect(damage);
            if (shooter.GetComponent<Staff>() != null)
            {
                otherFighter.DelayedHurtAnimation(0.5f);
            } else { otherFighter.DelayedHurtAnimation(0.1f); }
            


            if (reflected == false) { ScalingTrigger(); // Increase Whatever
                //Items
                if (shooter.GetComponentInChildren<TriTippedDagger>() != null)
                {
                    TriTippedDagger tTD = shooter.GetComponentInChildren<TriTippedDagger>();
                    SeedManager.Instance.UseSubSeed("TriTippedDagger"); //generate random 

                    int randomInt = Random.Range(0, 100);
                    if (randomInt <= 20 * tTD.stacks)
                    {
                        otherFighter.bleedStacks += 1;
                    }

                    SeedManager.Instance.RestoreMasterSeed();
                }
                //Item
                if (shooter.GetComponentInChildren<BloodoftheBandit>() != null)
                {
                    for (int i = 0; i < shooter.GetComponentInChildren<BloodoftheBandit>().stacks; i++)
                    {
                        if (shooter.GetComponent<Bow>().refreshInterval > 0.1f) { shooter.GetComponent<Bow>().refreshInterval -= 0.1f; }
                    }
                    
                }
            } 
            else // If it was reflected though
            {
                if (otherFighter == shooter)
                {
                    reflector.ShieldGrow(damage);
                }
            }
            
            DestroySelf();
        }

        // LEFT WALL
        if (other.gameObject.CompareTag("LeftWall"))
        {
            DestroySelf();
        }

        // RIGHT WALL
        if (other.gameObject.CompareTag("RightWall"))
        {
            DestroySelf();
        }

        // Bottom WALL
        if (other.gameObject.CompareTag("BottomWall"))
        {
            DestroySelf();
        }

        // Top WALL
        if (other.gameObject.CompareTag("Wall"))
        {
            DestroySelf();
        }
    }

    public void ScalingTrigger() // call this
    {
        if (shooter.GetComponentInChildren<GlassBall>() != null)
        {
            GlassBall glassBall = shooter.GetComponentInChildren<GlassBall>();
            for (int i = -1; i < glassBall.stacks; i++)
            {
                ScalingIncrease(); // Call the actual scaling logic 'stacks' times
            }
        }
        else
        {
            // If there's no GlassBall, perhaps just apply the single stack effect once?
            ScalingIncrease();
        }
    }

    public virtual void ScalingIncrease() //( Don't call ) Increase scaling for each type
    {
        if (shooter == null) return;
        if (shooter.GetComponentInChildren<BloodoftheKnight>() != null)
        { //Item
            BloodoftheKnight BotK = shooter.GetComponentInChildren<BloodoftheKnight>();
            BotK.IncreaseScaling();
        }
    }

    // Destroy(gameObject);
    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }

    //Reflection stuff
    private Vector3 FirstOrderIntercept(
    Vector3 shooterPosition,
    Vector3 shooterVelocity,
    Vector3 targetPosition,
    Vector3 targetVelocity)
    {
        Vector3 relPosition = targetPosition - shooterPosition;
        Vector3 relVelocity = targetVelocity - shooterVelocity;

        float t = FirstOrderInterceptTime(speed, relPosition, relVelocity);
        return targetPosition + t * targetVelocity;
    }

    private float FirstOrderInterceptTime(float projectileSpeed, Vector3 relPosition, Vector3 relVelocity)
    {
        float velocitySquared = relVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - projectileSpeed * projectileSpeed;

        // Handle straight line cases
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -relPosition.sqrMagnitude /
                      (2f * Vector3.Dot(relVelocity, relPosition));
            return Mathf.Max(t, 0f);
        }

        float b = 2f * Vector3.Dot(relVelocity, relPosition);
        float c = relPosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        {
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a);
            float t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2);
                else
                    return t1;
            }
            else
                return Mathf.Max(t2, 0f);
        }
        else if (determinant < 0f)
            return 0f;
        else
            return Mathf.Max(-b / (2f * a), 0f);
    }
}