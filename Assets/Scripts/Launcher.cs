using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float launchSpeed = 10f;
    public Camera cam;

    public Transform target;       // The GameObject the HP follows
    public Vector3 offset;         // Offset from the target (e.g. new Vector3(0, 2, 0))
    public Text hpText;            // Drag your Text UI object here

    public bool shotDone = false;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (shotDone == true) return;
            ShootTowardsMouse();
        }

        if (target != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
            hpText.transform.position = screenPos;
        }
    }

    void ShootTowardsMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

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
}
