using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrikMark : Buff
{
    float dmg;
    float heal;
    public void SetUp(Adrik user, float dmg, float time, float heal)
    {
        this.time = time;
        this.user = user;
        this.dmg = dmg;
        target = GetComponent<PjBase>();
    }
    public void Explode()
    {
        target.GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.fire);
        user.DamageDealed(user, target, dmg, HitData.Element.fire, HitData.AttackType.range, HitData.HabType.hability);
        Die();
    }
}
