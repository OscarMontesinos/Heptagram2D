using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Piroclast : Projectile
{
    Adrik user;
    float dmg;
    public float area;
    public ParticleSystem explosion;
    public void SetUp(PjBase user, float speed, float range, float dmg)
    {
        this.user = user.GetComponent<Adrik>();
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (CharacterManager.Instance.data[3].convergence >= 6)
        {
            if (collision.CompareTag("Enemy") || collision.CompareTag("Wall"))
            {
                Explode();

            }
        }
        else
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.fire);
                user.DamageDealed(user, collision.GetComponent<Enemy>(), HitData.Element.fire, HitData.AttackType.range, HitData.HabType.hability);

                if (CharacterManager.Instance.data[3].convergence >= 5 && collision.gameObject.GetComponent<AdrikMark>())
                {
                    user.CreateHeal(collision.gameObject.GetComponent<AdrikMark>().target.GetComponent<Enemy>(), user.CalculateControl(user.a2Heal * 0.5f));
                    collision.gameObject.GetComponent<AdrikMark>().Die();
                }

                collision.gameObject.AddComponent<AdrikMark>().SetUp(user, user.CalculateSinergy(user.h1MarkDmg), user.h1MarkDuration, user.CalculateControl(user.h1MarkHeal));
                Die();
            }
        }
        base.OnTriggerEnter2D(collision);
    }

    void Explode()
    {
        explosion.Play();
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, area, GameManager.Instance.enemyLayer);
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemyColl.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.fire);
            user.DamageDealed(user, enemyColl.GetComponent<Enemy>(), HitData.Element.fire, HitData.AttackType.range, HitData.HabType.hability);

            if (CharacterManager.Instance.data[3].convergence >= 5 && enemyColl.gameObject.GetComponent<AdrikMark>())
            {
                user.CreateHeal(enemyColl.gameObject.GetComponent<AdrikMark>().target.GetComponent<Enemy>(), user.CalculateControl(user.a2Heal * 0.5f));
                enemyColl.gameObject.GetComponent<AdrikMark>().Die();
            }

            enemyColl.gameObject.AddComponent<AdrikMark>().SetUp(user, user.CalculateSinergy(user.h1MarkDmg), user.h1MarkDuration, user.CalculateControl(user.h1MarkHeal));

        }
        Die();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, area);
    }
}
