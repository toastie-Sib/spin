using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private HashSet<Collider> currentContacts = new HashSet<Collider>();

    public float damage = 1.0f;
    [Header("Type")]
    public bool sword = false;
    public bool dagger = false;
    public bool doNotHurt = false;
    public bool shield = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon")) //Parry
        {
            // Prevent both sides from triggering — only do it on the one with lower ID
            if (gameObject.GetInstanceID() < other.gameObject.GetInstanceID())
            {

                Fighter myFighter = GetComponentInParent<Fighter>();
                if (myFighter != null)
                    myFighter.ReverseDirection();
                    myFighter.isInvincible = true;

                Fighter otherFighter = other.gameObject.GetComponentInParent<Fighter>();
                if (otherFighter != null)
                    otherFighter.ReverseDirection();
                    if (shield == false) { otherFighter.isInvincible = true; }

                if (shield == true)
                { // reflect arrows
                    Weapon otherWeapon = other.gameObject.GetComponentInParent<Weapon>();
                    Weapon myWeapon = other.gameObject.GetComponentInParent<Weapon>();
                    if (otherWeapon.doNotHurt == true) return;
                    otherFighter.HitDetect(otherWeapon.damage);
                    ShieldGrow(otherWeapon.damage);
                }
            }
        }

        if (other.gameObject.CompareTag("Fighter")) //Damage
        {
            Fighter otherFighter = other.GetComponent<Fighter>();
            Fighter myFighter = GetComponentInParent<Fighter>();

            if (otherFighter.isInvincible || doNotHurt == true) return;
                otherFighter.HitDetect(damage);
                myFighter.ReverseDirection();
                if (sword == true)
                {
                    damage += 1;
                }
                if (dagger == true)
                {
                    myFighter.IncreaseSpeed();
                }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            currentContacts.Remove(other);
        }
    }

    public void ShieldGrow(float damage)
    {
        Vector3 scale = transform.localScale;
        scale += new Vector3(0f, ((0.01f) * damage), 0f);

        transform.localScale = scale;
    }
}
