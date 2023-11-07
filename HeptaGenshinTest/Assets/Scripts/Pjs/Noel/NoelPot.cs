using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NoelPot : Buff
{
    float pot;
    public void SetUp(PjBase user, float pot)
    {
        untimed = true;
        this.user = user;
        target = GetComponent<PjBase>();
        this.pot = pot;
        target.stats.sinergy += pot;
    }

    public void BuffUpdate(float pot)
    {
        this.pot += pot;
        target.stats.sinergy += pot;
    }

    public override void Die()
    {
        target.stats.sinergy -= pot;
        base.Die();
    }
}
