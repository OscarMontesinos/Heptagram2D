using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : Projectile
{
    PjBase user;
    float dmg;
    public void SetUp(PjBase user, float speed, float range, float dmg)
    {
        this.user = user;
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.fire);
            user.DamageDealed(user, collision.GetComponent<Enemy>(), dmg, HitData.Element.fire, HitData.AttackType.range, HitData.HabType.basic);

            Die();
        }
        base.OnTriggerEnter2D(collision);
    }
}
