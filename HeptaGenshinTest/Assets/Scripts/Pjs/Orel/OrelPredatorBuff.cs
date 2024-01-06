using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class OrelPredatorBuff : Buff
{
    Orel orel;
    float spd;
    float atSpd;
    float def;
    float pot;
    public void SetUp(Orel user, float time, float spd, float atSpd, float def, float pot)
    {
        this.time = time;
        this.user = user;
        orel = user;
        target = GetComponent<PjBase>();
        this.spd = spd;
        this.atSpd = target.stats.atSpd * atSpd / 100;
        target.stats.atSpd += this.atSpd;
        target.stats.spd += this.spd;

        if (CharacterManager.Instance.data[1].convergence >= 3)
        {
            this.def = def;
            this.user.stats.desertResist -= def;
            this.user.stats.waterResist -= def;
            this.user.stats.iceResist -= def;
            this.user.stats.natureResist -= def;
            this.user.stats.fireResist -= def;
            this.user.stats.windResist -= def;
            this.user.stats.lightningResist -= def;
        }
        if (CharacterManager.Instance.data[1].convergence >= 5)
        {
            user.currentHab1Cd = 0;
        }
        if (CharacterManager.Instance.data[1].convergence >= 7)
        {
            this.pot = pot;
            target.stats.sinergy += pot;
            user.hab1Cd /= 2;

        }
    }

    public override void Update()
    {
        base.Update();

        if (CharacterManager.Instance.data[1].convergence >= 7)
        {
            orel.a2Special = true;
        }
        if (time > 0)
        {
            orel.currentHab2Cd = time;
        }
    }
    public override void Die()
    {
        orel.currentHab2Cd = orel.CDR(orel.hab2Cd);
        user.stats.spd -= spd;
        user.stats.atSpd -= atSpd;
        if (CharacterManager.Instance.data[1].convergence >= 3)
        {
            user.stats.desertResist += def;
            user.stats.waterResist += def;
            user.stats.iceResist += def;
            user.stats.natureResist += def;
            user.stats.fireResist += def;
            user.stats.windResist += def;
            user.stats.lightningResist += def;
        }
        if (CharacterManager.Instance.data[1].convergence >= 7)
        {
            target.stats.sinergy -= pot;
            user.hab1Cd *= 2;
            orel.a2Special = false;

        }
        base.Die();
    }
}
