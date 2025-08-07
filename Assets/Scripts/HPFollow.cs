using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPFollow : MonoBehaviour
{
    public Transform target;       // The GameObject the HP follows
    public Vector3 offset;         // Offset from the target (e.g. new Vector3(0, 2, 0))
    public Text hpText;            // Drag your Text UI object here

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
            hpText.transform.position = screenPos;
        }
    }
}
