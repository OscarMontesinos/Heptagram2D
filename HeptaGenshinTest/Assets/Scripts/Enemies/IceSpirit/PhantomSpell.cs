using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomSpell : Projectile
{
    Enemy user;
    float dmg;
    public void SetUp(Enemy user, float speed, float range, float dmg)
    {
        this.user = user;
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerBarrier"))
        {
            collision.GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.ice);
            Die();
        }
        base.OnTriggerEnter2D(collision);
    }
}
