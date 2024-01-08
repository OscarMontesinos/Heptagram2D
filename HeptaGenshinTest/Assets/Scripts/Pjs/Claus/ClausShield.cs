using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClausShield : Shield
{
    public void SetUp(PjBase user, float amount, float time)
    {
        this.user = user;
        shieldAmount += amount;
        singularShieldAmount = shieldAmount;
        this.time = time;
    }


    public override void Die()
    {
        shieldAmount -= singularShieldAmount;
        base.Die();
    }
}
