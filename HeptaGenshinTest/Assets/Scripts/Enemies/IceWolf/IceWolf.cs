using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IceWolf : Enemy
{
    public float attackDmg;
    public float attackArea;
    public float attackRange;
    public float attackSpd;
    public float attackCd;
    public override void Start()
    {
        base.Start();
        AI();
    }
    public override void AI()
    {
        if (target != null && stunTime <= 0)
        {
            Vector2 dir = target.transform.position - transform.position;
            if (dir.magnitude <= attackRange && currentHab1Cd <= 0)
            {
                StartCoroutine(AttackDash());
            }
            else
            {
                transform.Translate(dir.normalized * stats.spd * Time.deltaTime);
                StartCoroutine(RestartAi());
            }
        }
        else
        {
            StartCoroutine(RestartAi());
        }
        base.AI();
    }

    IEnumerator AttackDash()
    {
        yield return (StartCoroutine(Cast(0.5f)));
        if (target != null)
        {
            Vector2 dir = target.transform.position - transform.position;
            StartCoroutine(Dash(dir, attackSpd, attackRange * 2, false));

            List<PjBase> enemiesHitted = new List<PjBase>();

            while (dashing)
            {
                Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, attackArea, GameManager.Instance.playerLayer);
                PjBase enemy;
                foreach (Collider2D enemyColl in enemiesHit)
                {
                    enemy = enemyColl.GetComponent<PjBase>();
                    if (!enemiesHitted.Contains(enemy))
                    {
                        enemy.GetComponent<TakeDamage>().TakeDamage(CalculateSinergy(attackDmg), HitData.Element.ice);

                        enemiesHitted.Add(enemy);
                    }
                }
                yield return null;
            }
            yield return StartCoroutine(Cast(1));
            currentHab1Cd = CDR(attackCd);
        }
        StartCoroutine(RestartAi());
    }


    public override void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackArea);
        base.OnDrawGizmosSelected();
    }
}
