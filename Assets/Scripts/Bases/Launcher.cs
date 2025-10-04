using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
    [Header("Launcher Settings")]
    public GameObject projectilePrefab;
    public Camera cam;
    public float launchSpeed = 10f;
    private bool shotDone = false;
    public bool isPlayer = false;
    [HideInInspector] public GameObject stashedProjectile;
    [HideInInspector] public Fighter fighter;
    private SceneSwitcher es;
    public GameObject[] enemyPrefabs;

    [Header("UI Tracking")]
    public Vector3 offset;
    public Text hpText;
    private Transform target;       // The GameObject the HP follows
    public Text nameUIText;
    public Text hpUIText;
    public Text stacksText;
    public Text damageText;
    public Text spinText;
    public Text extraText;

    [Header("Trajectory Settings")]
    public LayerMask collisionMask;         // Layers to detect
    public int maxPredictionSteps = 100;    // Max segments to draw
    public float timeStep = 0.05f;          // Simulation step size
    private LineRenderer lineRenderer;
    private LineRenderer aiLineRenderer;
    private Vector3 direction;

    void Start()
    {
        GameObject esObject = GameObject.Find("EventSystem");
        es = esObject.GetComponent<SceneSwitcher>();
        cam = Camera.main;

        es.fighterAmount += 1; // Count the amount of Fighters spawned

        if (lineRenderer == null) // Player trajectory
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            lineRenderer.widthMultiplier = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")); //Can change how looks
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.yellow;
        }

        if (aiLineRenderer == null) // Other trajectory
        {
            GameObject aiLineObj = new GameObject("AI_Trajectory");
            aiLineRenderer = aiLineObj.AddComponent<LineRenderer>();
            aiLineRenderer.positionCount = 0;
            aiLineRenderer.widthMultiplier = 0.05f;
            aiLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            aiLineRenderer.startColor = Color.red;
            aiLineRenderer.endColor = Color.magenta;
        }

        if (!shotDone && !isPlayer) // Set up Other trajectory
        {
            // Generate preview direction for AI
            Vector2 randomDir2D = Random.insideUnitCircle.normalized;
            direction = new Vector3(randomDir2D.x, randomDir2D.y, 0f);
            DrawOtherTrajectoryFromDirection(direction, aiLineRenderer);

            string name = "EnemySystem";
            string rngName = name.Replace("System", SceneSwitcher.Instance.currentNodeID + maxPredictionSteps);
            SeedManager.Instance.UseSubSeed(rngName); //generate random enemy

            int enemyTypeIndex = Random.Range(0, enemyPrefabs.Length);
            projectilePrefab = enemyPrefabs[enemyTypeIndex];

            SeedManager.Instance.RestoreMasterSeed();

        }
        if (!shotDone && isPlayer)
        {
            projectilePrefab = es.fighterPrefab;
        }

        stashedProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        fighter = stashedProjectile.GetComponent<Fighter>();    if (isPlayer) { fighter.isPlayer = true; } //Know that is player
        if (fighter != null)
        {
            // Assign HP UI
            target = fighter.transform; // This is still for your own UI movement
            fighter.UI = this;        // Let the fighter know which Launcher controls its HP UI
        }

        transform.position += new Vector3(0, 0, 0.5f);

        if (nameUIText != null)
        {
            if (isPlayer == true)
            {
                string objectName = stashedProjectile.name.Replace("(Clone)", "");
                nameUIText.text = ("Player: " + objectName).ToString();
            }
            else
            {
                string objectName = stashedProjectile.name.Replace("(Clone)", "");
                nameUIText.text = ("Enemy: " + objectName).ToString();



                //Set up enemy HP
                fighter.maxHp *= SceneSwitcher.Instance.enemyHP;
                fighter.hp *= SceneSwitcher.Instance.enemyHP;
                //Set up HP Healing if Elite
                Scene activeScene = SceneManager.GetActiveScene();
                if (activeScene.name == "EliteArena14")
                {
                    fighter.gameObject.AddComponent<EliteHealing>();
                }
            }
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
            Vector2 screenPos = cam.WorldToScreenPoint(target.position + offset);
            hpText.transform.position = screenPos;
        }
    }

    void DrawTrajectory() //Players Trajectory Drawn
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
            newPosition.z = 0f; // Keep flat

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

    bool GetMouseWorldPoint(out Vector3 hitPoint) //Follow mouse
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

    void DrawOtherTrajectoryFromDirection(Vector3 direction, LineRenderer lr) //For Other Spawner
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
        Rigidbody projectileRb = stashedProjectile.GetComponent<Rigidbody>();

        Fighter fighter = stashedProjectile.GetComponent<Fighter>(); // Get Fighter script

        projectileRb.useGravity = true;
        fighter.isActive = true;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.forward, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            if (projectileRb != null)
            {
                Vector3 direction = (hitPoint - transform.position).normalized;
                direction.z = 0f;
                projectileRb.velocity = direction * launchSpeed;

                target = projectileRb.transform;
            }
        }
        shotDone = true;
        
        transform.position = new Vector3(50, 0, 0);
    }

    void Spawn() //For Other Spawner
    {
        Rigidbody projectileRb = stashedProjectile.GetComponent<Rigidbody>();

        Fighter fighter = stashedProjectile.GetComponent<Fighter>(); // Get Fighter script

        projectileRb.useGravity = true;
        fighter.isActive = true;

        if (projectileRb != null)
        {

            projectileRb.velocity = direction * launchSpeed;
        }
        shotDone = true;
        
        transform.position = new Vector3(50, 0, 0);
    }
}
