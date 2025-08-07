using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactPause : MonoBehaviour
{
    public static ImpactPause Instance;

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
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
