using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactPause : MonoBehaviour
{
    public static ImpactPause Instance;
    public bool overridePuase = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PauseForImpact(float duration)
    {
        StartCoroutine(DoImpactPause(duration));
    }

    private IEnumerator DoImpactPause(float duration)
    {
        Time.timeScale = 0f;
        overridePuase = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        overridePuase = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Speed up function
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (overridePuase == true) return;
            Time.timeScale = 3.0f; // 3x speed
        }
        else
        {
            if (overridePuase == true) return;
            Time.timeScale = 1.0f; // Normal speed
        }
    }
}
