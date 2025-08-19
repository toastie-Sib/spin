using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrench : Weapon
{
    public GameObject turretPrefab;
    public Transform spawnPoint;
    public override void IncreaseScaling()
    {
        GameObject turret = Instantiate(turretPrefab, spawnPoint.position, spawnPoint.rotation);

        Turret cannon = turret.GetComponent<Turret>();
        if (cannon != null)
        {
            Fighter myFighter = GetComponentInParent<Fighter>();
            cannon.owner = myFighter;
        }

        Rigidbody rb = turret.GetComponent<Rigidbody>();

        // Ignore collision between bow and projectile
        Collider projectileCollider = turret.GetComponent<Collider>();
        Collider[] weaponColliders = GetComponentsInChildren<Collider>();
        foreach (Collider weaponCol in weaponColliders)
        {
            if (weaponCol != GetComponent<Collider>()) // skip parent's collider
            {
                Physics.IgnoreCollision(weaponCol, projectileCollider);
            }
        }

    }
}
