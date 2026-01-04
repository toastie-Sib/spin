using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedManager : MonoBehaviour
{
    private bool speedUp = false;
    public bool overridePuase = false;
    public bool stopImpactFrames = false;
    public float timeScale = 2.0f;
    public static GameSpeedManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
    }

    public void PauseForImpact(float duration)
    {
        StartCoroutine(DoImpactPause(duration));
    }

    private IEnumerator DoImpactPause(float duration)
    {
        if (stopImpactFrames == true) yield break;
        Time.timeScale = 0f;
        overridePuase = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        overridePuase = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Speed up function
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (speedUp == false)
            {
                speedUp = true;
            } else {
                speedUp = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (stopImpactFrames == false)
            {
                stopImpactFrames = true;
            }
            else
            {
                stopImpactFrames = false;
            }
        }

        if (speedUp == true)
        {
            if (overridePuase == true) return;
            Time.timeScale = timeScale;
            if (Input.GetMouseButtonDown(0))
            {
                if (timeScale <= 0.5f)
                {
                    timeScale *= (0.5f);
                }
                else { timeScale -= 0.5f; }

            }
            if (Input.GetMouseButtonDown(1))
            {
                if (timeScale <= 0.5f)
                {
                    timeScale *= (2f);
                }
                else { timeScale += 0.5f; }
            }
        } else {
            if (overridePuase == true) return;
            Time.timeScale = 1.0f; // Normal speed
        }
    }
}
