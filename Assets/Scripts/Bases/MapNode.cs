using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapNode : MonoBehaviour
{
    public string sceneName;                     // Scene to load
    public List<MapNode> connectedNodes;         // What nodes can be reached from here
    public bool isUnlocked = false;              // Can you click this node?
    private Button button;
    public string nodeID;
    public bool startingNode = false;
    public int enemyHP = 0;

    [Header("Path Settings")]
    public Material lineMaterial;   // Assign a simple UI/Unlit material
    private List<LineRenderer> paths = new List<LineRenderer>();

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void Start()
    {
        var current = SceneSwitcher.Instance.GetCurrentNode();
        if (current != null)
        {
            current.isUnlocked = true;

            
        }

        DrawPaths();

        UpdateVisual();

        UpdatePathColors();



        StartCoroutine(TryUnlockConnectedNodes());


        StartCoroutine(TryInitSpace());
        
    }

    void OnClick()
    {
        if (isUnlocked)
        {
            // Save which node you're at
            SceneSwitcher.Instance.SetCurrentNode(nodeID);
            if(enemyHP != 0)
            {
                SceneSwitcher.Instance.enemyHP = enemyHP;
            }

            var current = SceneSwitcher.Instance.GetCurrentNode();
            SphereCollider collider = current.GetComponentInChildren<SphereCollider>();
            GameObject startPoint = GameObject.Find("Start Point");
            AssignAnimation animMovement = GameObject.Find("PlayerAnim").GetComponent<AssignAnimation>();
            animMovement.stashedAnimation.GetComponent<AnimationMovement>().slideDuration = 1.5f;
            animMovement.stashedAnimation.GetComponent<AnimationMovement>().StartingPoint();        //This should be making the animation move immedietly but it stays still for a moment

            startPoint.transform.position = collider.transform.position;
            startPoint.transform.position += new Vector3(0f, 1.4f, 0f);

            SceneSwitcher.Instance.SlowLoadSpecificSceneDelay(sceneName);
            isUnlocked = false;
            UpdateVisual();


        }
    }

    private IEnumerator TryInitSpace()
    {

        yield return new WaitForSeconds(0.01f);
        var current = SceneSwitcher.Instance.GetCurrentNode();
        if (current == null && startingNode == true)
        {
            SceneSwitcher.Instance.SetCurrentNode(nodeID);
            Unlock();
        }
    }

    private IEnumerator TryUnlockConnectedNodes()
    {
        var current = SceneSwitcher.Instance.GetCurrentNode();
        yield return new WaitForSeconds(0.01f);
        if (current != null)
        {
            SphereCollider collider = current.GetComponentInChildren<SphereCollider>();
            GameObject startPoint = GameObject.Find("Start Point");

            startPoint.transform.position = collider.transform.position;
            startPoint.transform.position += new Vector3(0f, 1.4f, 0f);

            current.isUnlocked = false;
            UpdateVisual();

            foreach (var item in current.connectedNodes)
            {
                item.GetComponent<MapNode>().Unlock();
            }
        }
    }

    public void Unlock()
    {
        isUnlocked = true;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        button.interactable = isUnlocked;
    }

    private void DrawPaths()
    {
        foreach (var target in connectedNodes)
        {
            if (target == null) continue;

            GameObject lineObj = new GameObject("PathLine");
            lineObj.transform.SetParent(GameObject.Find("MapLineContainer").transform, false); // put under root Canvas

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.widthMultiplier = 0.15f; // adjust thickness
            lr.positionCount = 2;
            lr.sortingOrder = 200; // ensure above background

            // Use UI space conversion
            Vector3 startPos = GetComponentInChildren<SphereCollider>().transform.position;
            Vector3 endPos = target.GetComponentInChildren<SphereCollider>().transform.position;

            lr.useWorldSpace = true;
            lr.SetPosition(0, startPos);
            lr.SetPosition(1, endPos);

            // Start all inactive
            lr.startColor = Color.black;
            lr.endColor = Color.black;

            paths.Add(lr);
        }
    }

    private void UpdatePathColors()
    {
        foreach (var lr in paths)
        {
            if (isUnlocked)
            {
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                lr.material.color = Color.red;
            }
            else
            {
                lr.startColor = Color.black;
                lr.endColor = Color.black;
                lr.material.color = Color.black;
            }
        }
    }
}
