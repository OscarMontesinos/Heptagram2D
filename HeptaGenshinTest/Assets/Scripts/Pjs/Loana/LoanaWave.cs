using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoanaWave : Projectile
{
    PjBase user;
    float dmg;
    float shieldAmount;
    LoanaShield shield;
    public void SetUp(PjBase user, LoanaShield shield, float speed, float spdOverTime, float range, float dmg, float shieldAmount)
    {
        this.user = user;
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
        this.spdOverTime = spdOverTime;
        this.shieldAmount = shieldAmount;
        this.shield = shield;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.water);
            user.DamageDealed(user, collision.GetComponent<Enemy>(), HitData.Element.water, HitData.AttackType.range, HitData.HabType.hability);
            shield.ChangeShieldAmount(shieldAmount);
        }
        base.OnTriggerEnter2D(collision);
    }
}
