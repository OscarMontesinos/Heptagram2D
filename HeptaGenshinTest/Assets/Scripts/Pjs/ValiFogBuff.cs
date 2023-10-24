using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ValiFogBuff : Buff
{
    float slow;
    float spd;
    float atSpd;
    public float iceDeb;
    public void SetUp(Vali user, float duration, float slow, float spd, float atSpd, float iceDeb)
    {
        this.user = user;
        time = duration;
        this.spd = spd;
        this.slow = slow;
        this.iceDeb = iceDeb;
        target = GetComponent<PjBase>();
        if (target == null)
        {
            eTarget = GetComponent<Enemy>();
            eTarget.stats.spd += this.spd;
            eTarget.stats.spd -= this.slow;
            eTarget.stats.iceResist -= this.iceDeb;
        }
        else
        {
            this.atSpd = target.stats.atSpd * atSpd / 100;

            target.stats.atSpd += this.atSpd;
            target.stats.spd += this.spd;
            target.stats.spd -= this.slow;
            target.stats.iceResist -= this.iceDeb;
        }
    }

    public override void Die()
    {
        if (target != null)
        {
            target.stats.atSpd -= atSpd;
            target.stats.spd -= spd;
            target.stats.spd += slow;
            target.stats.iceResist += iceDeb;
        }
        else
        {
            eTarget.stats.atSpd -= atSpd;
            eTarget.stats.spd -= spd;
            eTarget.stats.spd += slow;
            eTarget.stats.iceResist += iceDeb;
        }
        base.Die();
    }
}
