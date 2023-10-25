using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrelAssaultDebuff : Buff
{
    float def;
    public void SetUp(Orel user, float def, float time)
    {
        this.time = time;
        this.user = user;
        target = GetComponent<PjBase>();
        this.def = def;
        target.stats.desertResist -= this.def;
    }

    public override void Die()
    {
        target.stats.desertResist += def;
        base.Die();
    }
}
