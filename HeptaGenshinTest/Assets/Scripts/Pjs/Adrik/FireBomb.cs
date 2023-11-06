using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBomb : Projectile
{

    Adrik user;
    float dmg;
    public float area;
    public ParticleSystem explosion;
    bool triggered;
    public void SetUp(PjBase user, float speed, float range, float dmg)
    {
        this.user = user.GetComponent<Adrik>();
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Enemy") || collision.CompareTag("Wall")) && speed > 0) 
        {
            Explode();

        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.CompareTag("Enemy") || collision.CompareTag("Wall")) && speed > 0)
        {
            Explode();

        }
    }

    void Explode()
    {
        triggered = true;
        explosion.Play();
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, area, GameManager.Instance.enemyLayer);
        foreach (Collider2D enemyColl in enemiesHit)
        {

            enemyColl.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(dmg, HitData.Element.fire);
            user.DamageDealed(user, enemyColl.GetComponent<Enemy>(), HitData.Element.fire, HitData.AttackType.range, HitData.HabType.hability);
            if (enemyColl.gameObject.GetComponent<AdrikMark>())
            {
                enemyColl.gameObject.GetComponent<AdrikMark>().Explode();
            }
        }
        Die();

    }

    public override void Die()
    {
        if (!triggered)
        {
            Explode();
        }
        else
        {
            base.Die();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, area);
    }
}
