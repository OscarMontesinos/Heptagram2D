using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ValiArrow : Projectile
{
    PjBase user;
    float dmg;
    bool isMistArrow;
    public void SetUp(PjBase user, float speed, float range, float dmg, bool isMistArrow)
    {
        this.user = user;
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
        this.isMistArrow = isMistArrow;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(dmg, HitData.Element.ice );
            user.DamageDealed(user, collision.GetComponent<Enemy>(), HitData.Element.ice, HitData.AttackType.range, HitData.HabType.basic);

            if (isMistArrow)
            {
                ValiMistMark mark = collision.GetComponent<Enemy>().AddComponent<ValiMistMark>();
                mark.SetUp(user.GetComponent<Vali>(), user.CalculateSinergy( user.GetComponent<Vali>().h1MarkDmg), user.GetComponent<Vali>().h1MarkTime);

                if (CharacterManager.Instance.data[0].convergence >= 3)
                {
                    user.currentHab1Cd -= 1.5f;
                }

            }

            Die();
        }
        base.OnTriggerEnter2D(collision);
    }
}
