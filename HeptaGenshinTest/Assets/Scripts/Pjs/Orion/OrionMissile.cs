using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrionMissile : Projectile
{
    Orion user;
    float dmg;
    float stunTime;
    public float area;
    public ParticleSystem explosion;
    public void SetUp(PjBase user, float speed, float range, float dmg, float stunnTime)
    {
        this.user = user.GetComponent<Orion>();
        this.stunTime = stunnTime;
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
    }

    public override void FixedUpdate()
    {
        Vector2 dir = transform.up;
        _rigidbody.velocity = dir.normalized * speed;
        if (!withoutRange)
        {
            Vector2 dist = startPos - new Vector2(transform.position.x, transform.position.y);
            if (dist.magnitude > range)
            {
                Explode();
            }
        }
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Wall"))
        {
            Explode();

        }
    }

    void Explode()
    {
        explosion.Play();
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, area, GameManager.Instance.enemyLayer);
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemyColl.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.lightning);
            user.DamageDealed(user, enemyColl.GetComponent<Enemy>(), dmg, HitData.Element.lightning, HitData.AttackType.range, HitData.HabType.hability);
            user.Stunn(enemyColl.GetComponent<Enemy>(), stunTime);
            if (CharacterManager.Instance.data[7].convergence >= 6)
            {
                user.actualShield.ChangeShieldAmount(user.CalculateControl(user.c4ExtraShield));
            }
        }
        Die();

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, area);
    }
}
