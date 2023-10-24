using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngineInternal;

public class Vali : PjBase
{
    public float a1Damage;
    public float a1Spd;
    public float a1Range;
    public GameObject arrow;
    public float a2Damage;
    public float a2Spd;
    public float a2Range;
    public GameObject iceArrow;
    public GameObject hugeIceArrow;
    public float h1Spd;
    public float h1Range;
    public float h1Dmg;
    public float h1StunTime;
    public float h1ArrowDetour;
    public float h1ArrowBounceRange;
    public float h1ArrowSpeed;
    public float h1ArrowRange;
    public float h1ArrowDmg;
    public float h1MarkDmg;
    public float h1MarkTime;
    public GameObject mistArrow;
    public GameObject fog;
    public GameObject preFog;
    public float fogDuration;
    public float fogSpd;
    public float fogAtSpd;
    public float fogSlow;
    public float fogIceDeb;
    public GameObject homingIceArrow;
    public float homingSpd;
    public float homingTorque;
    public float homingDetour;
    public float homingDmg;
    bool homingDir;
    public GameObject dashArrow;
    [HideInInspector]
    public float currentArrowDashCd;
    public float arrowDashCd;
    public int arrowDashCount;
    public float arrowDashRange;
    public float arrowDashDmg;
    public float valiConvAtSpd;
    int combo;

    public override void Start()
    {
        base.Start();
        if(CharacterManager.Instance.data[0].convergence >= 6)
        {
            foreach(PjBase pj in GameManager.Instance.pjList)
            {
                ValiBuff buff = pj.AddComponent<ValiBuff>();
                buff.SetUp(this, CalculateControl(valiConvAtSpd));
            }
        }
    }

    public override void Update()
    {
        base.Update();

        if (currentArrowDashCd > 0)
        {
            currentArrowDashCd -= Time.deltaTime;
        }
    }

    public override void MainAttack()
    {
        if(combo!=0 && currentComboReset <= 0)
        {
            combo = 0;  
        }
        if (!IsCasting())
        {
            StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
            if (combo == 0)
            {
                ValiArrow arrow = Instantiate(this.arrow, transform.position, controller.pointer.transform.rotation).GetComponent<ValiArrow>();
                arrow.SetUp(this, a1Spd, a1Range, CalculateSinergy(a1Damage),false);
                combo++;
                currentComboReset = CalculateAtSpd(stats.atSpd) + 0.5f;
            }
            else
            {
                StartCoroutine(Cast(CalculateAtSpd(stats.atSpd) / 4));
                StartCoroutine(Basic2());
                combo=0;
            }
        }
        base.MainAttack();
    }

    public override void StrongAttack()
    {
        if (!IsCasting())
        {
            if (CharacterManager.Instance.data[0].convergence >= 7)
            {
                StartCoroutine(HugeIceArrow());
            }
            else
            {
                StartCoroutine(IceArrow());
            }
        }
        base.StrongAttack();
    }

    public override void Hab1()
    {
        if (currentHab1Cd <= 0 && !IsCasting())
        {
            MistArrow arrow = Instantiate(mistArrow, transform.position, controller.pointer.transform.rotation).GetComponent<MistArrow>();
            currentHab1Cd = CDR(hab1Cd);
            arrow.SetUp(this, h1ArrowSpeed, h1Range, CalculateSinergy(h1Dmg), h1StunTime, h1ArrowDetour, h1ArrowBounceRange, h1ArrowSpeed, h1ArrowRange, CalculateSinergy(h1ArrowDmg));
        }
        base.Hab2();
    }
    public override void Hab2()
    {
        if (currentHab2Cd <= 0 && !IsCasting())
        {
            StartCoroutine(Fog());
        }
        base.Hab2();
    }

    IEnumerator Basic2()
    {
        controller.LockPointer(true);
        ValiArrow arrow = Instantiate(this.arrow, transform.position, controller.pointer.transform.rotation).GetComponent<ValiArrow>();
        arrow.SetUp(this, a1Spd, a1Range, CalculateSinergy(a1Damage), false);
        yield return new WaitForSeconds(CalculateAtSpd(stats.atSpd) / 3.5f);
        ValiArrow arrow2 = Instantiate(this.arrow, transform.position, controller.pointer.transform.rotation).GetComponent<ValiArrow>();
        arrow2.SetUp(this, a1Spd, a1Range, CalculateSinergy(a1Damage),false);
        controller.LockPointer(false);
    }
    IEnumerator IceArrow()
    {
        controller.LockPointer(true);
        yield return StartCoroutine(Cast(CalculateAtSpd(stats.atSpd) * strongAtSpdMultiplier));
        ValiIceArrow arrow = Instantiate(iceArrow, transform.position, controller.pointer.transform.rotation).GetComponent<ValiIceArrow>();
        arrow.SetUp(this, a2Spd, a2Range, CalculateSinergy(a2Damage), false);
        controller.LockPointer(false);
        yield return StartCoroutine(SoftCast(1f*stats.atSpd));
    }
    IEnumerator HugeIceArrow()
    {
        controller.LockPointer(true);
        yield return StartCoroutine(Cast(CalculateAtSpd(stats.atSpd) * strongAtSpdMultiplier));
        ValiIceArrow arrow = Instantiate(hugeIceArrow, transform.position, controller.pointer.transform.rotation).GetComponent<ValiIceArrow>();
        arrow.SetUp(this, a2Spd, a2Range, CalculateSinergy(a2Damage), true);
        controller.LockPointer(false);
        yield return StartCoroutine(SoftCast(1f*stats.atSpd));
    }
    IEnumerator Fog()
    {
        Instantiate(preFog, transform.position, controller.pointer.transform.rotation);
        yield return StartCoroutine(Cast(0.7f));
        ValiFog fog = Instantiate(this.fog, transform.position, controller.pointer.transform.rotation).GetComponent<ValiFog>();
        fog.SetUp(this, fogDuration, fogSlow, fogSpd, CalculateControl(fogAtSpd), fogIceDeb);
        currentHab2Cd = CDR(hab2Cd);
        yield return StartCoroutine(SoftCast(0.25f));
    }

    void ShootArrowsOnDash()
    {
        currentArrowDashCd = arrowDashCd;
        List<Enemy> enemyList = new List<Enemy>();
        Enemy chosenEnemy = null;
        int rnds=0;
        while (rnds < arrowDashCount)
        {
            chosenEnemy = null;
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                Vector3 enemyDist = enemy.transform.position - transform.position;
                if (Physics2D.Raycast(transform.position, enemyDist, arrowDashRange, GameManager.Instance.enemyLayer) && !Physics2D.Raycast(transform.position, enemyDist, enemyDist.magnitude, GameManager.Instance.wallLayer) && !enemyList.Contains(enemy))
                {
                    if (chosenEnemy == null)
                    {
                        chosenEnemy = enemy;
                    }
                    else
                    {
                        Vector3 chosenEnemyDist = chosenEnemy.transform.position - transform.position;
                        if (enemyDist.magnitude < chosenEnemyDist.magnitude)
                        {
                            chosenEnemy = enemy;
                        }
                    }
                }
            }
            if (chosenEnemy != null)
            {
                Vector2 enemyDir = chosenEnemy.transform.position - transform.position;
                ValiArrow arrow = Instantiate(dashArrow, transform.position, transform.rotation).GetComponent<ValiArrow>();
                arrow.SetUp(this, a1Spd, arrowDashRange, CalculateSinergy(arrowDashDmg),false);
                arrow.transform.up = enemyDir;
                enemyList.Add(chosenEnemy);
            }
            rnds++;
        }
    }
    void HomingArrow(GameObject target)
    {
        ValiHomingArrow arrow = Instantiate(homingIceArrow, transform.position, transform.rotation).GetComponent<ValiHomingArrow>();
        arrow.SetUp(this, target, homingSpd, CalculateSinergy(homingDmg),homingTorque);
        arrow.transform.up = target.transform.position - arrow.transform.position;
        if (homingDir)
        {
            arrow.transform.localEulerAngles = new Vector3(arrow.transform.localEulerAngles.x, arrow.transform.localEulerAngles.y, arrow.transform.localEulerAngles.z + homingDetour);
        }
        else
        {
            arrow.transform.localEulerAngles = new Vector3(arrow.transform.localEulerAngles.x, arrow.transform.localEulerAngles.y, arrow.transform.localEulerAngles.z - homingDetour);
        }

        homingDir = !homingDir;
    }

    public override void Interact(PjBase user, Enemy target,  HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {
        if (CharacterManager.Instance.data[0].convergence >= 1 && habType == HitData.HabType.basic && attackType == HitData.AttackType.range)
        {
            HomingArrow(target.gameObject);
        }

        base.Interact(user, target, element, attackType, habType);
    }

    public override void EndedBasicDash()
    {
        base.BasicDash();
        if (CharacterManager.Instance.data[0].convergence >= 2 && currentArrowDashCd <= 0)
        {
            ShootArrowsOnDash();
        }
    }
}
