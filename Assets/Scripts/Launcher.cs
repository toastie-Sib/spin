using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    //Used for Launcher
    public GameObject projectilePrefab;
    public float launchSpeed = 10f;
    public Camera cam;

    public bool shotDone = false;
    public bool isPlayer = false;

    //Used for HP Tracking
    public Transform target;       // The GameObject the HP follows
    public Vector3 offset;
    public Text hpText;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for Left click to start
        {
            if (shotDone == true) return;
            if (isPlayer == true)
            {
                ShootTowardsMouse();
            }
            if (isPlayer == false)
            {
                Spawn();
            }
        }

        if (target != null) // HP follow target
        {
            Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
            hpText.transform.position = screenPos;
        }
    }

    void ShootTowardsMouse() //For Player
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            Fighter fighter = projectile.GetComponent<Fighter>(); // Get Fighter script
            if (fighter != null)
            {
                // Assign HP UI
                target = fighter.transform; // This is still for your own UI movement
                fighter.hpUI = this;        // Let the fighter know which Launcher controls its HP UI
            }

            if (rb != null)
            {
                Vector3 direction = (hitPoint - transform.position).normalized;
                rb.velocity = direction * launchSpeed;

                target = rb.transform;
            }
        }
        shotDone = true;
        transform.position = new Vector3(50, 0, 0);
    }

    void Spawn() //For Other Spawner
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        Fighter fighter = projectile.GetComponent<Fighter>(); // Get Fighter script
        if (fighter != null)
        {
            // Assign HP UI
            target = fighter.transform; // This is still for your own UI movement
            fighter.hpUI = this;        // Let the fighter know which Launcher controls its HP UI
        }

        if (rb != null)
        {
            Vector2 randomDir2D = Random.insideUnitCircle.normalized;
            Vector3 direction = new Vector3(randomDir2D.x, randomDir2D.y, 0f);
            rb.velocity = direction * launchSpeed;
        }
        shotDone = true;
        transform.position = new Vector3(50, 0, 0);
    }
}
