using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Orion : PjBase
{
    public Slider chargeBar;
    public float maxCharge;
    float charge;
    public float chargePerSecond;
    OrionShield actualShield;
    public float maxShield;
    public float shieldPerSecond;
    public GameObject lighntingBullet;
    bool attacking;
    public float a1Detour;
    float a1DetourMultiplier;
    public float a1Spd;
    public float a1Range;
    public float a1Dmg;
    public float a1AtSpdConvertion;
    public float h1Charge;
    public override void Start()
    {
        base.Start();
        OrionShield shield = controller.AddComponent<OrionShield>();
        shield.SetUp(this, CalculateControl(0), CalculateControl(maxShield));
        actualShield = shield;
    }
    public override void Update()
    {
        base.Update();
        if (attacking)
        {
            if (charge < maxCharge)
            {
                charge += Time.deltaTime * chargePerSecond;
            }
        }
        else
        {
            if (charge > 0)
            {
                charge -= Time.deltaTime;
            }
        }

        if (charge >= maxCharge)
        {
            actualShield.ChangeShieldAmount(Time.deltaTime * CalculateSinergy( shieldPerSecond));
        }
        else if (actualShield.singularShieldAmount >0)
        {
            actualShield.ChangeShieldAmount(-Time.deltaTime * 3);
        }

        a1DetourMultiplier = Mathf.InverseLerp(maxCharge, 0, charge);
        if(a1DetourMultiplier < 0.5f)
        {
            a1DetourMultiplier = 0.5f;
        }
        chargeBar.value = charge;
        chargeBar.maxValue = maxCharge;
    }
    public override void MainAttack()
    {
       
        if (!IsCasting())
        {
            attacking = true;
            StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd * a1AtSpdConvertion)));
            LightningBullet bullet = Instantiate(lighntingBullet, transform.position, controller.pointer.transform.rotation).GetComponent<LightningBullet>();
            bullet.SetUp(this, a1Spd, a1Range, CalculateSinergy(a1Dmg));
            bullet.transform.localEulerAngles = new Vector3(bullet.transform.localEulerAngles.x, bullet.transform.localEulerAngles.y, bullet.transform.localEulerAngles.z + Random.Range(-a1Detour * a1DetourMultiplier, a1Detour* a1DetourMultiplier));
        }
        base.MainAttack();
    }

    public override void Hab1()
    {
        if (!IsCasting() && currentHab1Cd <= 0)
        {
            charge = h1Charge;
            currentHab1Cd = CDR(hab1Cd);
        }
            base.Hab1();
    }
    public override IEnumerator SoftCast(float time)
    {
        softCasting = true;
        yield return new WaitForSeconds(time);
        attacking = false;
        softCasting = false;
    }
}
