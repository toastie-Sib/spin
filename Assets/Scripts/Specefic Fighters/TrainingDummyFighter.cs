using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummyFighter : Fighter
{
    //[HideInInspector] public Rigidbody rb;
    //[HideInInspector] public Color originalColor;
    //[HideInInspector] public Renderer objectRenderer;
    //[HideInInspector] public Fighter player;
    //public AudioClip hit;
    //public AudioClip click;
    public bool hitable = true;


    //all references of isPlayer to change to allow hit from player and enemy


    public override void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        objectRenderer = GetComponentInParent<Renderer>();
        originalColor = objectRenderer.material.color;

        isPlayer = hitable;
    }

    public override void HitDetect(float amount)
    {
        if (isInvincible) return; // Don't get hurt
    
        
        StartCoroutine(GetHit(amount));
        
    }
    private IEnumerator GetHit(float amount)
    {
        

        AudioSource.PlayClipAtPoint(hit, transform.position, 0.8f);
        // Trigger impact frames
        GetComponentInChildren<Renderer>().material.color = Color.white;
        //StartCoroutine(ImpactFrames(0.2f));

        GameSpeedManager.Instance.PauseForImpact(0.2f);

        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<Renderer>().material.color = originalColor;
    }

    //public override void OnCollisionEnter(Collision collision)
    //{
    //    if (click != null) { AudioSource.PlayClipAtPoint(click, transform.position); } //bounce sound :D
    //    float horizontalSpeed = Mathf.Abs(rb.velocity.x);
    //    //float verticalSpeed = Mathf.Abs(rb.velocity.y);
    //
    //    // LEFT WALL
    //    if (collision.gameObject.CompareTag("LeftWall"))
    //    {
    //
    //        // If moving toward the wall (x is negative) and slow
    //        //if (horizontalSpeed < 1f) {
    //            //Vector3 wallBoost = new Vector3(Random.Range(4f, 7f), Random.Range(-2f, 2f), 0f);
    //            //rb.velocity += wallBoost;
    //        //}
    //
    //        
    //    }
    //
    //    // RIGHT WALL
    //    if (collision.gameObject.CompareTag("RightWall"))
    //    {
    //
    //        // If moving toward the wall (x is positive) and slow
    //        //if (horizontalSpeed < 1f) {
    //            //Vector3 wallBoost = new Vector3(Random.Range(4f, 7f), Random.Range(-2f, 2f), 0f);
    //            //rb.velocity -= wallBoost;
    //        //}
    //
    //        
    //    }
    //
    //    //Bottom WALL
    //    if (collision.gameObject.CompareTag("BottomWall"))
    //    {
    //
    //        
    //    }
    //
    //    // Top WALL
    //    if (collision.gameObject.CompareTag("Wall"))
    //    {
    //        
    //    }
    //}
}
