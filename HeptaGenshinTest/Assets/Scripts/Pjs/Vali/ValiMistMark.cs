using System.Collections;
using System.Collections.Generic;
using TMPro.SpriteAssetUtilities;
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
        target = GetComponent<PjBase>();
    }
    void HitInteract.Interact(PjBase user, PjBase enemy, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {
        if (!triggered && user != this.user)
        {
            triggered = true;
            target.GetComponent<TakeDamage>().TakeDamage(dmg, HitData.Element.ice);
            user.DamageDealed(user, target, HitData.Element.ice, HitData.AttackType.range, HitData.HabType.hability);
            Die();
        }
    }
}
