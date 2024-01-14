using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ClausProtBuff : Buff
{

    Claus claus;
    public float def;
    float dmg;
    public void SetUp(Claus user, float time, float def)
    {
        this.time = time;
        this.user = user;
        claus = user;
        target = GetComponent<PjBase>();
        this.def = def;
        target.stats.resist += def;
        if (CharacterManager.Instance.data[claus.id].convergence >= 3)
        {
            target.stunTime = 0;
        }
    }

    public void AddDamage(float value)
    {
        dmg += value;
    }
    public override void Update()
    {
        base.Update();
        claus.h2CurrentDuration = time;
    }

    public override void Die()
    {
        if (dmg > 0)
        {
            ClausShield shield = claus.controller.gameObject.AddComponent<ClausShield>();
            shield.SetUp(claus, dmg * claus.CalculateControl(claus.c7ShieldPerOneDmg), claus.hab2Cd / 4);
        }
        claus.h2Active = false;
        claus.currentHab2Cd = claus.CDR(claus.hab2Cd);
        target.stats.resist -= def;
        base.Die();
    }
}
