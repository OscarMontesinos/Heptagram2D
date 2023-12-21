using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class OrionDebuff : Buff
{
    float def;
    public void SetUp(Orion user, float def, float time)
    {
        this.time = time;
        this.user = user;
        target = GetComponent<PjBase>();
        this.def = def;
        target.stats.desertResist -= this.def;
        target.stats.lightningResist -= this.def;
        target.stats.waterResist -= this.def;
        target.stats.iceResist -= this.def;
        target.stats.windResist -= this.def;
        target.stats.natureResist -= this.def;
        target.stats.fireResist -= this.def;
    }

    public override void Die()
    {
        target.stats.desertResist += this.def;
        target.stats.lightningResist += this.def;
        target.stats.waterResist += this.def;
        target.stats.iceResist += this.def;
        target.stats.windResist += this.def;
        target.stats.natureResist += this.def;
        target.stats.fireResist += this.def;
        base.Die();
    }
}
