using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretSlow : Buff
{
    float slow;
    public void SetUp(PjBase user, float slow, float time)
    {
        this.time = time;
        this.user = user;
        target = GetComponent<PjBase>();
        this.slow = slow;
        target.stats.spd -= this.slow;
        if (target.stats.spd < 1)
        {
            Die();
        }
    }

    public override void Die()
    {
        target.stats.spd += slow;
        base.Die();
    }
}
