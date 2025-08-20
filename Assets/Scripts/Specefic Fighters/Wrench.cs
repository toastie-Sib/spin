using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrench : Weapon
{
    public GameObject turretPrefab;
    public Transform spawnPoint;
    private bool hasSpawnedTurretThisSwing = false;
    public override void IncreaseScaling()
    {
        if (hasSpawnedTurretThisSwing) return;
        hasSpawnedTurretThisSwing = true;

        GameObject turret = Instantiate(turretPrefab, spawnPoint.position, spawnPoint.rotation);

        Turret cannon = turret.GetComponent<Turret>();
        if (cannon != null)
        {
            Fighter myFighter = GetComponentInParent<Fighter>();
            cannon.owner = myFighter;
        }

        // Ignore collision between bow and projectile   Code: Bower SHOULD THIS BE HERE????
        Collider projectileCollider = turret.GetComponent<Collider>();
        Collider[] weaponColliders = GetComponentsInChildren<Collider>();
        foreach (Collider weaponCol in weaponColliders)
        {
            if (weaponCol != GetComponent<Collider>()) // skip parent's collider
            {
                Physics.IgnoreCollision(weaponCol, projectileCollider);
            }
        }
        StartCoroutine(ResetSwing());
    }

    public void ShieldTurret(Fighter shieldFighter) // For shield make sure same as above
    {
        if (hasSpawnedTurretThisSwing) return;
        hasSpawnedTurretThisSwing = true;

        GameObject turret = Instantiate(turretPrefab, spawnPoint.position, spawnPoint.rotation);

        Turret cannon = turret.GetComponent<Turret>();
        if (cannon != null)
        {
            Fighter myFighter = GetComponentInParent<Fighter>();
            cannon.owner = shieldFighter;
            cannon.GetComponentInChildren<Renderer>().material.color = shieldFighter.originalColor;
            cannon.nose.GetComponentInChildren<Renderer>().material.color = shieldFighter.originalColor;
        }



        // Ignore collision between bow and projectile   Code: Bower SHOULD THIS BE HERE????
        //Collider projectileCollider = turret.GetComponent<Collider>();
        //Collider[] weaponColliders = GetComponentsInChildren<Collider>();
        //foreach (Collider weaponCol in weaponColliders)
        //{
        //    if (weaponCol != GetComponent<Collider>()) // skip parent's collider
        //    {
        //        Physics.IgnoreCollision(weaponCol, projectileCollider);
        //    }
        //}
        StartCoroutine(ResetSwing());
    }

    private IEnumerator ResetSwing() // don't spawn a fuck ton of turrets fix
    {
        yield return new WaitForSeconds(0.1f);
        hasSpawnedTurretThisSwing = false;
    }
}
