using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : Fighter
{
    private bool oceansFloor = false;

    public override void Update()
    {
        base.Update();
        Vector3 velocity = rb.velocity;

        if (velocity.sqrMagnitude > 0.01f) // make sure it's actually moving
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("BottomWall"))
        {
            Sword weapon = GetComponentInChildren<Sword>();
            weapon.IncreaseScaling();
            weapon.damage = Mathf.Round(weapon.damage * 10.0f) * 0.1f;
        }
    }


    public override void IncreaseBaseAtkSpeed()
    {
        weapon.GetComponent<Sword>().damage += SceneSwitcher.Instance.playerBonusAtkSpd;
        UpdateDynamicUI("Damage: ", Mathf.Round(weapon.GetComponent<Sword>().damage * 10.0f) * 0.1f, 2);
    }

    //Boss Item
    void FixedUpdate()
    {
        if (oceansFloor == false) return;
        GetComponent<Rigidbody>().AddForce(Physics.gravity * 2, ForceMode.Acceleration);
    }

    public void OceansFloor()
    {
        oceansFloor = true;
        StartCoroutine(BerserkerTimer());
    }

    private IEnumerator BerserkerTimer()
    {
        yield return new WaitForSeconds(5.0f);
        oceansFloor = false;
    }
}