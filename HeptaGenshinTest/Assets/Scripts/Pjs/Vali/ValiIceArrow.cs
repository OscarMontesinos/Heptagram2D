using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValiIceArrow : Projectile
{
    PjBase user;
    float dmg;
    bool piercing;
    public void SetUp(PjBase user, float speed, float range, float dmg, bool piercing)
    {
        this.user = user;
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
        this.piercing = piercing;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.ice);
            user.DamageDealed(user, collision.GetComponent<Enemy>(),HitData.Element.ice, HitData.AttackType.range, HitData.HabType.basic);
            if (!piercing)
            {
                Die();
            }
        }
        base.OnTriggerEnter2D(collision);
    }
}
