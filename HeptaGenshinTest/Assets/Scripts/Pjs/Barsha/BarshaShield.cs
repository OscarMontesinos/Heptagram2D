using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BarshaShield : Shield
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
        if (CharacterManager.Instance.data[6].convergence >= 6)
        {
            user.GetComponent<Barsha>().h1Particle.SetActive(false);
        }
        shieldAmount -= singularShieldAmount;
        base.Die();
    }
}
