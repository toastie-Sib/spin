using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private HashSet<Collider> currentContacts = new HashSet<Collider>();
    private bool reflected = false;
    public Bow shooter;
    private Weapon reflector;

    public AudioClip parry;
    public float damage = 1.0f;
    public float speed = 10.0f;
    

    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Weapon otherWeapon = other.gameObject.GetComponentInParent<Weapon>();
            AudioSource.PlayClipAtPoint(parry, transform.position);

            //Destroy Game object if it isn't a shield or reflected
            if (otherWeapon.shield == false) {  if (reflected == false) {
                    //Fighter otherFighter = otherWeapon.GetComponentInParent<Fighter>(); //Potential Parry Reflect
                    //transform.Rotate(0f, 0f, (otherFighter.direction) * -125f, Space.World); 
                    Destroy(gameObject);
                }
            } 
            else //If it is Parried by Shield go to shooter
            {
                if (shooter == null) { Destroy(gameObject); }
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
            }
        
        }
        
        if (other.gameObject.CompareTag("Fighter"))
        {
            Fighter otherFighter = other.GetComponent<Fighter>();
        
            if (otherFighter.isInvincible) return;
            if (otherFighter == shooter && reflected == false) return;
            if (reflected == true)
            {
                Fighter reflectorSheild = reflector.GetComponentInParent<Fighter>();
                if (otherFighter == reflectorSheild) return;
            }

            otherFighter.HitDetect(damage);

            if (reflected == false) {shooter.IncreaseFireRate(); } // Increase Firerate on Hit
            else // If it was reflected though
            { if (otherFighter == shooter) {
                    reflector.ShieldGrow(damage); } }
            
            Destroy(gameObject);
        }

        // LEFT WALL
        if (other.gameObject.CompareTag("LeftWall"))
        {
            Destroy(gameObject);
        }

        // RIGHT WALL
        if (other.gameObject.CompareTag("RightWall"))
        {
            Destroy(gameObject);
        }

        // Bottom WALL
        if (other.gameObject.CompareTag("BottomWall"))
        {
            Destroy(gameObject);
        }

        // Top WALL
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
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
//reflect 90o