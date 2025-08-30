using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assign : MonoBehaviour
{
    [HideInInspector] public SceneSwitcher es;
    
    public virtual void Start()
    {
        GameObject esObject = GameObject.Find("EventSystem");
        es = esObject.GetComponent<SceneSwitcher>();
    }
}
