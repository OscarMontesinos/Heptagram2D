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
    [HideInInspector]
    public float charge;
    public float chargePerSecond;
    [HideInInspector]
    public OrionShield actualShield;
    public float maxShield;
    public float shieldPerSecond;
    public GameObject lighntingBullet;
    bool attacking;
    public float a1Detour;
    float a1DetourMultiplier;
    public float a1Spd;
    public float a1Range;
    public float a1Dmg;

    public GameObject a2Missile;
    public float a2Cost;
    public float a2Spd;
    public float a2Range;
    public float a2Dmg;
    public float a2StunTime;

    public float a1AtSpdConvertion;
    public float h1Charge;

    public GameObject h2Wave;
    public float h2CastTime;
    public float h2Dmg;
    public float h2Spd;
    public float h2Range;
    public float h2Debuff;
    public float h2DebuffTime;

    public float c1Amount;

    public GameObject c3Shield;
    public OrionWall c3Wall;
    public float c3Cd;
    [HideInInspector]
    public float c3CurrentCd;
    public float c3Regen;

    public float c4ExtraCharge;
    public float c4ExtraShield;

    public GameObject missileIndicator;
    public float c6Cd;
    [HideInInspector]
    public float c6CurrentCd;
    public override void Start()
    {
        base.Start();
        OrionShield shield = controller.AddComponent<OrionShield>();
        shield.SetUp(this, CalculateControl(0), CalculateControl(maxShield));
        actualShield = shield;
        c3Shield.SetActive(false);
        c6CurrentCd = CDR(c6Cd);
    }
    public override void Update()
    {
        base.Update();

        if (c3CurrentCd > 0)
        {
            c3CurrentCd -= Time.deltaTime;
            c3Wall.hpBar.maxValue = c3Cd;
            c3Wall.hpBar.value = c3Cd - c3CurrentCd;
        }
        else if(c3Wall.hp <= 0)
        {
            c3Wall.RechargeShield();
        }
        if (c6CurrentCd > 0)
        {
            missileIndicator.SetActive(false);
            c6CurrentCd -= Time.deltaTime;
        }
        else
        {
            missileIndicator.SetActive(true);
        }
        if (attacking)
        {
            if (charge < maxCharge)
            {
                charge += Time.deltaTime * chargePerSecond;
            }

            if (CharacterManager.Instance.data[7].convergence >= 3 && c3CurrentCd <=0)
            {
                c3Shield.gameObject.SetActive(true);
            }
        }
        else
        {
            if (charge > 0 && !casting && !softCasting)
            {
                charge -= Time.deltaTime;
            }
            if (CharacterManager.Instance.data[7].convergence >= 3)
            {
                c3Shield.gameObject.SetActive(false);
            }
            if(c3Wall.hp < c3Wall.mHp && c3Wall.hp != 0)
            {
                c3Wall.hp += Time.deltaTime * c3Regen;
                c3Wall.hpBar.value = c3Wall.hp;
            }
        }

        if (charge >= maxCharge)
        {
            actualShield.ChangeShieldAmount(Time.deltaTime * (CalculateSinergy(shieldPerSecond)));
        }
        else if (actualShield.singularShieldAmount > 0)
        {
            actualShield.ChangeShieldAmount(-Time.deltaTime * 3);
        }

        a1DetourMultiplier = Mathf.InverseLerp(maxCharge, 0, charge);
        if (a1DetourMultiplier < 0.5f)
        {
            a1DetourMultiplier = 0.5f;
        }
        chargeBar.value = charge;
        chargeBar.maxValue = maxCharge;

        if (CharacterManager.Instance.data[5].convergence >= 1 && charge >= maxCharge)
        {
            stunTime = 0;
        }
        if (CharacterManager.Instance.data[7].convergence >= 1 && charge >= maxCharge)
        {
            if (currentHab1Cd > 0)
            {
                currentHab1Cd -= Time.deltaTime;
            }
            if (currentHab2Cd > 0)
            {
                currentHab2Cd -= Time.deltaTime;
            }
            if (c3CurrentCd > 0)
            {
                c3CurrentCd -= Time.deltaTime;
            }
            if (c6CurrentCd > 0)
            {
                c6CurrentCd -= Time.deltaTime;
            }
        }
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
    public override void StrongAttack()
    {

        if (!IsCasting() && (charge >= a2Cost || (CharacterManager.Instance.data[7].convergence >= 6 && c6CurrentCd <= 0 && c6CurrentCd <= 0)))
        {
            StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd * strongAtSpdMultiplier)));


            if (CharacterManager.Instance.data[7].convergence >= 6 && c6CurrentCd <= 0)
            {
                LaunchMissile(controller.pointer.transform.rotation, a2StunTime, a2Range);
                c6CurrentCd = CDR(c6Cd);
            }
            else
            {
                LaunchMissile(controller.pointer.transform.rotation, 0, a2Range);
                charge -= a2Cost;
            }
        }
        base.StrongAttack();
    }

    public void LaunchMissile(Quaternion rotation, float stunnTime, float range)
    {
        OrionMissile bullet = Instantiate(a2Missile, transform.position, rotation).GetComponent<OrionMissile>();
        bullet.SetUp(this, a2Spd, range, CalculateSinergy(a2Dmg), stunnTime);
    }

    public override void Hab1()
    {
        if (!IsCasting() && currentHab1Cd <= 0)
        {
            charge = h1Charge;
            currentHab1Cd = CDR(hab1Cd);
            if (CharacterManager.Instance.data[7].convergence >= 1)
            {
                foreach (PjBase pj in GameManager.Instance.pjList)
                {
                    OrionBuff buff = pj.AddComponent<OrionBuff>();
                    buff.SetUp(this, CalculateControl(c1Amount));
                }
            }
            base.Hab1();
        }
    }

    public override void Hab2()
    {
        if (!IsCasting() && currentHab2Cd <= 0)
        {
            StartCoroutine(OrionPulse());
            currentHab2Cd = CDR(hab2Cd);
        }
        base.Hab2();
    }

    IEnumerator OrionPulse()
    {
        yield return StartCoroutine(Cast(h2CastTime));
        OrionWave wave = Instantiate(h2Wave, transform.position, controller.pointer.transform.rotation).GetComponent<OrionWave>();
        wave.SetUp(this, h2Spd, h2Range, CalculateSinergy(h2Dmg),CalculateControl(h2Debuff), h2DebuffTime);

    }
    public override IEnumerator SoftCast(float time)
    {
        softCasting = true;
        yield return new WaitForSeconds(time);
        attacking = false;
        softCasting = false;
    }

    public override void TakeDmg(PjBase user, float value, HitData.Element element)
    {

        if (CharacterManager.Instance.data[7].convergence >= 1 && c3Wall.hp > 0 && attacking)
        {
            c3Wall.GetComponent<TakeDamage>().TakeDamage(user, value, element);
        }
        else
        {
            base.TakeDmg(user, value, element);
        }
    }

}
