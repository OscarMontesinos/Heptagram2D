using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoanaBuff : Buff
{
    float pot;
    public float prot;
    public void SetUp(PjBase user, float pot, float prot)
    {
        untimed = true;
        this.user = user;
        target = GetComponent<PjBase>();
        this.pot = pot;
        target.stats.sinergy += pot;
        this.prot = prot;
        target.stats.waterResist += prot;
    }

    public override void Die()
    {
        target.stats.sinergy -= pot;
        target.stats.waterResist -= prot;
        base.Die();
    }
}
