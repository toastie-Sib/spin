using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodoftheReaper : ItemBase
{


    public void StartAttack(float damage, Fighter target)
    {

        StartCoroutine(DamageTick(damage, target));
    }

    public IEnumerator DamageTick(float damage, Fighter target)
    {
        float tickDamage = damage / (2);

        ApplyDamage(tickDamage, target);

        for (int i = -1; i < stacks; i++)
        {
            yield return new WaitForSeconds(1.0f);
            ApplyDamage(tickDamage, target);
        }
    }

    public void ApplyDamage(float damage, Fighter target)
    {

        target.hp -= damage;
        target.hp = Mathf.Round(target.hp * 10.0f) * 0.1f;
        target.hp = Mathf.Max(target.hp, 0);
        target.UpdateUI();
    }
}
