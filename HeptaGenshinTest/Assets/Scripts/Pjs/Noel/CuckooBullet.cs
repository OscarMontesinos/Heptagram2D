using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuckooBullet : Projectile
{
    PjBase user;
    float dmg;
    CuckooTurret turret;
    public GameObject rayLine;
    bool first;
    public void SetUp(PjBase user, float speed, float range, float dmg, CuckooTurret turret, bool first)
    {
        this.user = user;
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
        this.turret = turret;
        this.first = first;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(dmg, HitData.Element.lightning);
            if (!first)
            {
                user.DamageDealed(user, collision.GetComponent<Enemy>(), HitData.Element.lightning, HitData.AttackType.range, HitData.HabType.hability);
            }
            else
            {
                user.DamageDealed(user, collision.GetComponent<Enemy>(), HitData.Element.lightning, HitData.AttackType.range, HitData.HabType.basic);
            }

            if (CharacterManager.Instance.data[4].convergence >= 4 && first)
            {
                TurretSlow slow = collision.GetComponent<Enemy>().gameObject.AddComponent<TurretSlow>();
                slow.SetUp(user, turret.user.c4Slow, turret.user.c4Duration);
            }

            if (CharacterManager.Instance.data[4].convergence >= 5 && first && turret.targetList.Count > 1)
            {
                LineRenderer line = Instantiate(this.rayLine, collision.transform.position,transform.rotation).GetComponent<LineRenderer>();
                List<Enemy> list = new List<Enemy>(turret.targetList);
                line.SetPosition(0, collision.transform.position);
                line.startWidth = 0.4f;
                line.endWidth = 0.4f;
                line.positionCount = list.Count;
                int count = 1;
                foreach(Enemy enemy in list)
                {
                    if (enemy != collision.GetComponent<Enemy>() && enemy != null)
                    {
                        line.SetPosition(count, enemy.transform.position);

                        enemy.GetComponent<TakeDamage>().TakeDamage((dmg * turret.attackCount) * 0.5f, HitData.Element.lightning);
                        user.DamageDealed(user, enemy, HitData.Element.lightning, HitData.AttackType.range, HitData.HabType.hability);

                        TurretSlow slow = enemy.gameObject.AddComponent<TurretSlow>();
                        slow.SetUp(user, turret.user.c4Slow, turret.user.c4Duration);

                        count++;
                    }
                    else if(enemy == null)
                    {
                        line.SetPosition(count, line.GetPosition(count-1));
                        count++;
                    }
                }
            }

            Die();
        }
        base.OnTriggerEnter2D(collision);
    }
}
