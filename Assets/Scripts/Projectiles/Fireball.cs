using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Projectile
{
    [Header("Fireball")]
    public GameObject explosionEffect;
    private SpriteRenderer childSpriteRenderer;
    private CapsuleCollider capsuleCollider;
    private Staff myFighter;

    public override void Start()
    {
        base.Start();
        myFighter = shooter.GetComponent<Staff>();
        childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        damage += myFighter.damageIncrease;
    }


    public override void DestroySelf()
    {
        if (explosionDone == true) return;
        if (explosionEffect != null) //Visual Effect
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            float scaleFactor = myFighter.explosionRadius;
            Vector3 Scale = explosion.transform.localScale;
            Scale = Scale * scaleFactor; 
            explosion.transform.localScale = Scale;
        }

        Color finalColor = Color.red;
        finalColor.a = 0.0f;
        childSpriteRenderer.color = finalColor;
        capsuleCollider.center = new Vector3 (0,0,0);
        capsuleCollider.radius = 2f *(myFighter.explosionRadius);
        speed = 0;

        explosionDone = true;
        StartCoroutine(ActuallyDestroy());
    }

    private IEnumerator ActuallyDestroy()
    {
        yield return new WaitForSeconds(0.5f); //MAKE SURE THIS IS THE SAME AS THE EXPLOSION VALUE
        Destroy(gameObject);
    }

    public override void ScalingIncrease()
    {
        base.ScalingIncrease();
        myFighter.explosionRadius += 0.25f;
        myFighter.damageIncrease += 1f;
    }
}
