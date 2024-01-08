using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : Projectile
{

    Claus user;
    float dmg;
    public void SetUp(Claus user, float speed, float range, float dmg)
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
            collision.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.ice);
            user.DamageDealed(user, collision.GetComponent<Enemy>(), dmg,HitData.Element.ice, HitData.AttackType.range, HitData.HabType.basic);
            user.AddMana(user.manaPerHit);
            Die();
        }
        base.OnTriggerEnter2D(collision);
    }
}
