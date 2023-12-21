using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrionBuff : Buff
{
    Orion orion;
    float pot;
    public void SetUp(Orion user, float pot)
    {
        untimed = true;
        this.user = user;
        orion = user;
        target = GetComponent<PjBase>();
            this.pot = pot;
        target.stats.sinergy += this.pot;
    }

    public override void Update()
    {
        base.Update();

        if (orion.charge <= orion.maxCharge)
        {
            Die();
        }
    }
    public override void Die()
    {
        target.stats.sinergy -= pot;
        base.Die();
    }
}
