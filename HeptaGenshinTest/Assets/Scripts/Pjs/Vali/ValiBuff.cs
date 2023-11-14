using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValiBuff : Buff
{
    float atSpd;
    public void SetUp(Vali user, float atSpd)
    {
        untimed = true;
        this.user = user;
        target = GetComponent<PjBase>();
        this.atSpd = target.stats.atSpd * atSpd;
        target.stats.atSpd += this.atSpd;
    }

    public override void Die()
    {
        base.Die();
    }
}
