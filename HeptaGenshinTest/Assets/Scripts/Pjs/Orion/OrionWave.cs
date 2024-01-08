using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrionWave : Projectile
{

    Orion user;
    float dmg;
    float debuff;
    float time;
    public void SetUp(Orion user,  float speed,  float range, float dmg, float debuff, float time)
    {
        this.user = user;
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
        this.debuff = debuff;
        this.time = time;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.lightning);
            user.DamageDealed(user, collision.GetComponent<Enemy>(),dmg, HitData.Element.lightning, HitData.AttackType.range, HitData.HabType.hability);
            collision.gameObject.AddComponent<OrionDebuff>().SetUp(user, debuff, time);
            if (CharacterManager.Instance.data[7].convergence >= 6)
            {
                Vector3 missileRange = collision.gameObject.transform.position - user.transform.position;
                user.controller.pointer.transform.up = missileRange;
                user.LaunchMissile(user.controller.pointer.transform.rotation, user.a2StunTime, missileRange.magnitude);
            }
        }
        base.OnTriggerEnter2D(collision);
    }
}
