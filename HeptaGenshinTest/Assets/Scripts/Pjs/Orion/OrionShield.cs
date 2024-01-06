using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrionShield : Shield
{
    public float maxShield;
    public void SetUp(PjBase user, float amount, float maxShield)
    {
        this.user = user;
        shieldAmount += amount;
        singularShieldAmount = shieldAmount;
        this.maxShield = maxShield;
    }


    public override void Die()
    {
       
    }
    public override float ChangeShieldAmount(float value)
    {
        if (value > -singularShieldAmount)
        {
            singularShieldAmount += value;
            shieldAmount += value;
            if(singularShieldAmount > maxShield)
            {
                float value2 = singularShieldAmount - maxShield;
                singularShieldAmount -= value2;
                shieldAmount -= value2;
            }
            value = 0;
        }
        else
        {
            value += singularShieldAmount;
            singularShieldAmount = 0;
            shieldAmount += value;
        }

        if (shieldAmount < 0)
        {
            shieldAmount = 0;
        }

        return -value;
    }
}
