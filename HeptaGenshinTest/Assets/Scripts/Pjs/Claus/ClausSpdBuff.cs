using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ClausSpdBuff : Buff 
{
    Claus claus;
    float extraSpdPerOneMana;
    float currentSpd;
    public void SetUp(Claus user, float extraSpdPerOneMana)
    {
        untimed = true;
        this.user = user;
        claus = user;
        target = GetComponent<PjBase>();
        this.extraSpdPerOneMana = extraSpdPerOneMana;
    }

    public override void Update()
    {
        base.Update();
        Vector2 dist = claus.transform.position - target.transform.position;
        if (dist.magnitude <= claus.c3Range)
        {
            if (currentSpd != extraSpdPerOneMana * claus.mana)
            {
                target.stats.spd -= currentSpd;
                currentSpd = extraSpdPerOneMana * claus.mana;
                target.stats.spd += currentSpd;
            }
        }
        else
        {
            target.stats.spd -= currentSpd;
            currentSpd = 0;
        }
    }
    public override void Die()
    {
        target.stats.spd -= currentSpd;
        base.Die();
    }
}
