using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    [Header("Launcher Settings")]
    public GameObject projectilePrefab;
    public Camera cam;
    public float launchSpeed = 10f;
    public bool shotDone = false;
    public bool isPlayer = false;

    [Header("HP Tracking")]
    public Transform target;       // The GameObject the HP follows
    public Vector3 offset;
    public Text hpText;

    [Header("Trajectory Settings")]
    public LineRenderer lineRenderer;
    public LineRenderer aiLineRenderer;
    public Vector3 direction;
    public LayerMask collisionMask;         // Layers to detect
    public int maxPredictionSteps = 100;    // Max segments to draw
    public float timeStep = 0.05f;          // Simulation step size

    void Start()
    {
        cam = Camera.main;

        if (lineRenderer == null) // Player trajectory
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            lineRenderer.widthMultiplier = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")); //Can change how looks
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.yellow;
        }

        if (aiLineRenderer == null)
        {
            GameObject aiLineObj = new GameObject("AI_Trajectory");
            aiLineRenderer = aiLineObj.AddComponent<LineRenderer>();
            aiLineRenderer.positionCount = 0;
            aiLineRenderer.widthMultiplier = 0.05f;
            aiLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            aiLineRenderer.startColor = Color.red;
            aiLineRenderer.endColor = Color.magenta;
        }

        if (!shotDone && !isPlayer)
        {
            // Generate preview direction for AI
            Vector2 randomDir2D = Random.insideUnitCircle.normalized;
            direction = new Vector3(randomDir2D.x, randomDir2D.y, 0f);
            DrawOtherTrajectoryFromDirection(direction, aiLineRenderer);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for Left click to start
        {
            if (shotDone == true) return;
            if (isPlayer == true)
            {
                ShootTowardsMouse();
                lineRenderer.positionCount = 0; // Hide arc after shooting
            }
            if (isPlayer == false)
            {
                Spawn();
                aiLineRenderer.positionCount = 0; // Hide AI arc after spawn
            }
        }

        if (!shotDone && isPlayer) //Update trajectory
        {
            DrawTrajectory();
        }
        

        if (target != null) // HP follow target
        {
            Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
            hpText.transform.position = screenPos;
        }
    }

    void DrawTrajectory()
    {
        Vector3 hitPoint;
        if (!GetMouseWorldPoint(out hitPoint)) return;

        Vector3 velocity = (hitPoint - transform.position).normalized * launchSpeed;
        Vector3 currentPosition = transform.position;

        List<Vector3> points = new List<Vector3>();
        points.Add(currentPosition);

        for (int i = 0; i < maxPredictionSteps; i++)
        {
            Vector3 newPosition = currentPosition + velocity * timeStep + 0.5f * Physics.gravity * (timeStep * timeStep);

            // Check for collision between current and new position
            if (Physics.Raycast(currentPosition, newPosition - currentPosition, out RaycastHit hit, (newPosition - currentPosition).magnitude, collisionMask))
            {
                points.Add(hit.point);
                break;
            }

            points.Add(newPosition);
            velocity += Physics.gravity * timeStep;
            currentPosition = newPosition;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    bool GetMouseWorldPoint(out Vector3 hitPoint)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            hitPoint = ray.GetPoint(enter);
            return true;
        }

        hitPoint = Vector3.zero;
        return false;
    }

    void DrawOtherTrajectoryFromDirection(Vector3 direction, LineRenderer lr)
    {
        Vector3 velocity = direction.normalized * launchSpeed;
        Vector3 currentPosition = transform.position;

        List<Vector3> points = new List<Vector3>();
        points.Add(currentPosition);

        for (int i = 0; i < maxPredictionSteps; i++)
        {
            Vector3 newPosition = currentPosition + velocity * timeStep + 0.5f * Physics.gravity * (timeStep * timeStep);

            if (Physics.Raycast(currentPosition, newPosition - currentPosition, out RaycastHit hit, (newPosition - currentPosition).magnitude, collisionMask))
            {
                points.Add(hit.point);
                break;
            }

            points.Add(newPosition);
            velocity += Physics.gravity * timeStep;
            currentPosition = newPosition;
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
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
            
            rb.velocity = direction * launchSpeed;
        }
        shotDone = true;
        transform.position = new Vector3(50, 0, 0);
    }
}
