using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClausShield : Shield
{
    float amountLosedOverTime;
    public void SetUp(PjBase user, float amount, float time)
    {
        this.user = user;
        shieldAmount += amount;
        singularShieldAmount = shieldAmount;
        this.time = time;
        amountLosedOverTime = amount / time;
    }

    public override void Update()
    {
        base.Update();
        ChangeShieldAmount(-amountLosedOverTime * Time.deltaTime);
    }


    public override void Die()
    {
        shieldAmount -= singularShieldAmount;
        base.Die();
    }
}
