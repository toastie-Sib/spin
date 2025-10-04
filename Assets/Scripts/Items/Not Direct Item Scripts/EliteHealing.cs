using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteHealing : MonoBehaviour
{
    public float refreshInterval = 1.5f;         // Fire every second
    public float healAmount = 0.5f;
    private float nextRefreshTime = 1f;
    private Fighter fighter;

    void Start()
    {
        fighter = GetComponent<Fighter>();
    }


    // Update is called once per frame
    void Update()
    {

        //Timer for Refresh
        if (Time.time >= nextRefreshTime)
        {
            if (fighter.hp >= fighter.maxHp) return;
            StartCoroutine(Heal());
            nextRefreshTime = Time.time + refreshInterval;
        }
        
    }

    
    private IEnumerator Heal()
    {
        //Take Damage to HP
        fighter.hp += healAmount;
        fighter.UpdateUI();

        //AudioSource.PlayClipAtPoint(hit, transform.position, 0.8f);
        // Trigger impact frames
        GetComponentInChildren<Renderer>().material.color = Color.green;
        //StartCoroutine(ImpactFrames(0.2f));

        if (fighter.UI != null) { GameSpeedManager.Instance.PauseForImpact(0.2f); }

        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<Renderer>().material.color = fighter.originalColor;
    }
}
