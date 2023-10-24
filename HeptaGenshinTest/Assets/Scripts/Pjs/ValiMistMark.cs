using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ValiMistMark : Buff, HitInteract
{
    float dmg;
    bool triggered;
    public void SetUp(Vali user, float dmg, float time)
    {
        this.time = time;
        this.user = user;
        this.dmg = dmg;
        eTarget = GetComponent<Enemy>();
    }
    void HitInteract.Interact(PjBase user, Enemy enemy, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {
        if (!triggered && user != this.user)
        {
            triggered = true;
            eTarget.GetComponent<TakeDamage>().TakeDamage(dmg, HitData.Element.ice);
            user.DamageDealed(user, eTarget, HitData.Element.ice, HitData.AttackType.range, HitData.HabType.hability);
            Die();
        }
    }
}
