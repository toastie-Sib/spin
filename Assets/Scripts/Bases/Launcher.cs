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

    void Awake()
    {

        GameObject esObject = GameObject.Find("EventSystem");
        es = esObject.GetComponent<SceneSwitcher>();
        cam = Camera.main;

        StartCoroutine(Initialize());
    }


    private IEnumerator Initialize()
    {
        yield return null;


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

                // Give enemy random items based on how many enemy nodes the player has completed.
                // Enemy receives 1 item for every 5 completed enemy nodes (integer division).
                // Use SeedManager sub-seed so choice is deterministic per node/seed.
                int itemsToGive = SceneSwitcher.Instance.enemyNodesCompleted / 5;
                if (itemsToGive > 0)
                {
                    // Get candidate item names from SceneSwitcher (dictionary of known items)
                    List<string> candidateItems = SceneSwitcher.Instance.GetAllItemNames();

                    string seedNameBase = "EnemyItemSystem";

                    // Aggregate chosen items into stacks so duplicates become stacks on the same item
                    Dictionary<string, int> enemyItemStacks = new Dictionary<string, int>();

                    var asmCheck = System.Reflection.Assembly.GetExecutingAssembly();

                    for (int i = 0; i < itemsToGive; i++)
                    {
                        string rngName = seedNameBase.Replace("System", SceneSwitcher.Instance.currentNodeID + (SceneSwitcher.Instance.enemyNodesCompleted + i));
                        SeedManager.Instance.UseSubSeed(rngName);

                        if (candidateItems == null || candidateItems.Count == 0)
                        {
                            SeedManager.Instance.RestoreMasterSeed();
                            break;
                        }

                        // Try multiple attempts to pick a valid item (script exists and not banned if prefab available)
                        string chosen = null;
                        int attempts = 0;
                        while (attempts < 15 && chosen == null)
                        {
                            string candidate = candidateItems[Random.Range(0, candidateItems.Count)];

                            bool blocked = false;
                            // Optional: if a prefab exists in Resources, check ItemBans
                            GameObject prefab = Resources.Load<GameObject>(candidate);
                            if (prefab != null)
                            {
                                var bans = prefab.GetComponent<ItemBans>();
                                if (bans != null && bans.CannotBeUsedBy(es.fighterPrefab))
                                {
                                    blocked = true;
                                }
                            }

                            // Check that a matching item script/type exists and derives from ItemBase
                            System.Type itemTypeCheck = null;
                            foreach (var t in asmCheck.GetTypes())
                            {
                                if (t.Name == candidate)
                                {
                                    itemTypeCheck = t;
                                    break;
                                }
                            }

                            if (!blocked && itemTypeCheck != null && itemTypeCheck.IsSubclassOf(typeof(ItemBase)))
                            {
                                chosen = candidate;
                                break;
                            }

                            attempts++;
                        }

                        SeedManager.Instance.RestoreMasterSeed();

                        if (string.IsNullOrEmpty(chosen))
                            continue;

                        // increment stack count for the chosen item
                        if (!enemyItemStacks.ContainsKey(chosen)) enemyItemStacks[chosen] = 0;
                        enemyItemStacks[chosen]++;
                    }

                    // Apply stacks by adding the item component to the stashed projectile
                    var asm = System.Reflection.Assembly.GetExecutingAssembly();
                    foreach (var kvp in enemyItemStacks)
                    {
                        string itemName = kvp.Key;
                        int stacks = kvp.Value;

                        System.Type itemType = null;
                        foreach (var t in asm.GetTypes())
                        {
                            if (t.Name == itemName)
                            {
                                itemType = t;
                                break;
                            }
                        }

                        if (itemType != null && itemType.IsSubclassOf(typeof(ItemBase)))
                        {
                            var comp = stashedProjectile.GetComponent<Fighter>().weapon.AddComponent(itemType) as ItemBase;
                            if (comp != null) comp.stacks = stacks;
                        }
                        else
                        {
                            Debug.LogWarning($"Launcher: could not give enemy item '{itemName}' (type not found or not an ItemBase)");
                        }
                    }
                }
                //Set up HP Healing if Elite
                Scene activeScene = SceneManager.GetActiveScene();
                if (SceneManager.GetActiveScene().name.Contains("EliteArena")) { fighter.bonusDamage += SceneSwitcher.Instance.chapter; }
                if (activeScene.name == "EliteArena14")
                {
                    fighter.gameObject.AddComponent<GlassBall>();
                    fighter.hp *= 2.5f;
                    fighter.maxHp *= 2.5f;
                    fighter.gameObject.transform.localScale *= 1.75f;

                } else if (activeScene.name == "EliteArena24")
                {
                    fighter.gameObject.AddComponent<EliteHealing>();
                }
            }
        }
    }

    void Update()
    {
        if (cam == null || lineRenderer == null) return;

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

        foreach (var dummyFighter in FindObjectsOfType<TrainingDummyFighter>())
        {
            Rigidbody dummyFighterRb = dummyFighter.GetComponent<Rigidbody>();

            dummyFighter.isInvincible = false;
            dummyFighterRb.useGravity = true;
            dummyFighterRb.isKinematic = false;

            Vector2 randomDir2D = Random.insideUnitCircle.normalized;
            Vector3 dummyDirection = new Vector3(randomDir2D.x, randomDir2D.y, 0f);
            dummyFighterRb.velocity = dummyDirection * (launchSpeed / 2);
        }
        foreach (var bouncyBall in FindObjectsOfType<BouncyBallFighter>())
        {
            Rigidbody bouncyBallRb = bouncyBall.GetComponent<Rigidbody>();

            bouncyBall.isInvincible = true;
            bouncyBallRb.useGravity = true;
            bouncyBallRb.isKinematic = false;

            Vector2 randomDir2D = Random.insideUnitCircle.normalized;
            Vector3 ballDirection = new Vector3(randomDir2D.x, randomDir2D.y, 0f);
            bouncyBallRb.velocity = ballDirection * launchSpeed;
        }
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
