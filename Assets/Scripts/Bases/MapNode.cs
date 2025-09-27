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
    public float enemyHPMult = 0;
    public bool isChapterFinal = false;

    [Header("Path Settings")]
    public Material lineMaterial;   // Assign a simple UI/Unlit material
    private List<Image> paths = new List<Image>();
    public float lineLength;

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
            if (isChapterFinal == true)
            {
                SceneSwitcher.Instance.chapter += 1;
                SceneSwitcher.Instance.chapterEnd = true;
            }
            if(enemyHPMult != 0)
            {
                SceneSwitcher.Instance.enemyHP = enemyHPMult;
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
        RectTransform canvasRect = GameObject.Find("MapLineContainer").GetComponent<RectTransform>();

        foreach (var target in connectedNodes)
        {
            if (target == null) continue;

            // Create a UI line
            GameObject lineObj = new GameObject("PathLine", typeof(Image));
            lineObj.transform.SetParent(canvasRect, false);

            Image img = lineObj.GetComponent<Image>();
            img.color = Color.black; // default color

            RectTransform rt = lineObj.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0.5f); // center pivot

            // Get positions in canvas space
            Vector3 startPos = GetComponent<RectTransform>().position;
            Vector3 endPos = target.GetComponent<RectTransform>().position;

            // Direction and distance
            Vector3 dir = endPos - startPos;
            float dist = dir.magnitude;

            // Base thickness
            float baseThickness = 5f;

            // Apply scaling (x length, y thickness)
            float scaledLength = dist * lineLength;           // shrink horizontal scale
            float scaledThickness = baseThickness * 2.5f; // increase vertical scale

            // Set size
            rt.sizeDelta = new Vector2(scaledLength, scaledThickness);

            // Position at the midpoint
            rt.position = (startPos + endPos) / 2f;

            // Rotate so x-axis points toward target
            rt.rotation = Quaternion.FromToRotation(Vector3.right, dir);

            // Store reference
            paths.Add(img);
        }
    }





    private void UpdatePathColors()
    {
        foreach (var img in paths)
        {
            if (isUnlocked)
            {
                img.color = Color.red;
            }
            else
            {
                img.color = Color.black;
            }
        }
    }

}
