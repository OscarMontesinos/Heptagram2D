using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class Orel : PjBase
{
    public GameObject desertFeather;
    public float a1Damage;
    public float a1Spd;
    public float a1Range;
    public float a1detour;
    public float a1Modifier;


    public float a2Damage;
    public float a2Spd;
    public float a2Range;
    public float a2Detour;
    [HideInInspector]
    public  bool a2Special;
    public float a2DashSpd;
    public float a2DashRange;

    public GameObject h1Particles;
    public float h1Area;
    public float h1CastTime;
    public float h1Spd;
    public float h1Range;
    public float h1Dmg;
    public float h1Debuff;
    public float h1Time;

    public float h2CastTime;
    public float h2Duration;
    public float h2Spd;
    public float h2AtSpd;
    public float h2Pot;
    public float h2DashSpd;
    public float h2DashRange;


    public float resistBuff;
    float actualResistBuff;

    public override void Awake()
    {
        base.Awake();

        foreach (ParticleSystem particle in h1Particles.GetComponentsInChildren<ParticleSystem>())
        {
            particle.Stop();
        }
    }

    public override void Start()
    {
        base.Start();
        if (CharacterManager.Instance.data[1].convergence >= 3)
        {
            actualResistBuff = CalculateControl(resistBuff);
            stats.desertResist += resistBuff;
        }
    }
    public override void Update()
    {
        h1Particles.transform.rotation = controller.pointer.transform.rotation;

        base.Update();
    }

    public override void Activate(bool active)
    {

        base.Activate(active);
        foreach (ParticleSystem particle in h1Particles.GetComponentsInChildren<ParticleSystem>())
        {
            particle.Stop();
        }
    }

    public override void MainAttack()
    {
        base.MainAttack();
        if (!IsCasting() && !IsStunned())
        {
            StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
            DesertFeather feather = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
            feather.SetUp(this, a1Spd, a1Range, CalculateSinergy(a1Damage));
            if(CharacterManager.Instance.data[1].convergence >= 2)
            {
                DesertFeather feather2 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
                feather2.SetUp(this, Random.Range(a1Spd-20,a1Spd), a1Range, CalculateSinergy(a1Damage / a1Modifier));
                feather2.transform.localEulerAngles = new Vector3(feather2.transform.localEulerAngles.x, feather2.transform.localEulerAngles.y, feather2.transform.localEulerAngles.z + Random.Range(1,a1detour));
                DesertFeather feather3 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
                feather3.SetUp(this, Random.Range(a1Spd - 20, a1Spd), a1Range, CalculateSinergy(a1Damage / a1Modifier));
                feather3.transform.localEulerAngles = new Vector3(feather3.transform.localEulerAngles.x, feather3.transform.localEulerAngles.y, feather3.transform.localEulerAngles.z - Random.Range(1, a1detour));
            }
        }
    }

    public override void StrongAttack()
    {
        base.StrongAttack();
        if (!IsCasting() && !IsStunned() && !dashing)
        {
            if (CharacterManager.Instance.data[1].convergence >= 1 && a2Special)
            {

                StartCoroutine(SpecialFeatherDischarge());

                StartCoroutine(FeatherDischarge());
            }
            else
            {
                StartCoroutine(FeatherDischarge());
            }
        }
    }

    public override void Hab1()
    {
        base.Hab1();
        if (currentHab1Cd <= 0 && !IsCasting() && !IsStunned())
        {
            StartCoroutine(Assault());
        }
    }
    public override void Hab2()
    {
        base.Hab2();
        if (currentHab2Cd <= 0 && !IsCasting() && !IsStunned() && !GetComponent<OrelPredatorBuff>())
        {
            StartCoroutine(PredatorFlight());
        }
    }

    IEnumerator PredatorFlight()
    {
        controller.LockPointer(true);


        yield return StartCoroutine(Dash(controller.pointer.transform.up, h2DashSpd, h2DashRange, false));


        OrelPredatorBuff buff = gameObject.AddComponent<OrelPredatorBuff>();
        buff.SetUp(this, h2Duration, h2Spd, h2AtSpd, actualResistBuff, CalculateControl(h2Pot));

        controller.LockPointer(false);
    }

    IEnumerator Assault()
    {
        a2Special = true;
        currentHab1Cd = CDR(hab1Cd);
        controller.LockPointer(true);
        yield return StartCoroutine(Cast(h1CastTime));

        Vector2 dir = controller.pointer.transform.up;
        foreach (ParticleSystem particle in h1Particles.GetComponentsInChildren<ParticleSystem>())
        {
            particle.Play();
        }
        StartCoroutine(Dash(dir, h1Spd, h1Range, false));

        yield return null;

        List<Enemy> enemiesHitted = new List<Enemy>();

        while (dashing)
        {
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, h1Area, GameManager.Instance.enemyLayer);
            Enemy enemy;
            foreach (Collider2D enemyColl in enemiesHit)
            {
                enemy = enemyColl.GetComponent<Enemy>();
                if (!enemiesHitted.Contains(enemy))
                {
                    enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(h1Dmg), HitData.Element.desert);
                    DamageDealed(this, enemy, CalculateSinergy(h1Dmg), HitData.Element.desert, HitData.AttackType.melee, HitData.HabType.hability);

                    if(CharacterManager.Instance.data[1].convergence >= 4)
                    {
                        OrelAssaultDebuff debuff = enemy.AddComponent<OrelAssaultDebuff>();
                        debuff.SetUp(this, CalculateControl(h1Debuff), h1Time);
                    }
                        enemiesHitted.Add(enemy);
                }
            }
            yield return null;
        }
        foreach (ParticleSystem particle in h1Particles.GetComponentsInChildren<ParticleSystem>())
        {
            particle.Stop();
        }
        controller.LockPointer(false);
    }

    IEnumerator SpecialFeatherDischarge()
    {
        a2Special = false;

        yield return StartCoroutine(Cast(CalculateAtSpd(stats.atSpd) * (strongAtSpdMultiplier/2)));
        yield return StartCoroutine(Dash(-controller.pointer.transform.up,a2DashSpd,a2DashRange,false));


    }
    IEnumerator FeatherDischarge()
    {
        controller.LockPointer(true);
        yield return StartCoroutine(Cast(CalculateAtSpd(stats.atSpd) * strongAtSpdMultiplier));

        DesertFeather feather = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
        feather.SetUp(this, Random.Range(a2Spd - 30, a2Spd), a2Range, CalculateSinergy(a2Damage));
        feather.transform.localEulerAngles = new Vector3(feather.transform.localEulerAngles.x, feather.transform.localEulerAngles.y, feather.transform.localEulerAngles.z - Random.Range(-a2Detour, a2Detour));

        DesertFeather feather2 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
        feather2.SetUp(this, Random.Range(a2Spd - 30, a2Spd), a2Range, CalculateSinergy(a2Damage));
        feather2.transform.localEulerAngles = new Vector3(feather2.transform.localEulerAngles.x, feather2.transform.localEulerAngles.y, feather2.transform.localEulerAngles.z - Random.Range(-a2Detour, a2Detour));

        DesertFeather feather3 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
        feather3.SetUp(this, Random.Range(a2Spd - 30, a2Spd), a2Range, CalculateSinergy(a2Damage));
        feather3.transform.localEulerAngles = new Vector3(feather3.transform.localEulerAngles.x, feather3.transform.localEulerAngles.y, feather3.transform.localEulerAngles.z - Random.Range(-a2Detour, a2Detour));

        DesertFeather feather4 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
        feather4.SetUp(this, Random.Range(a2Spd - 30, a2Spd), a2Range, CalculateSinergy(a2Damage));
        feather4.transform.localEulerAngles = new Vector3(feather4.transform.localEulerAngles.x, feather4.transform.localEulerAngles.y, feather4.transform.localEulerAngles.z - Random.Range(-a2Detour, a2Detour));

        DesertFeather feather5 = Instantiate(desertFeather, transform.position, controller.pointer.transform.rotation).GetComponent<DesertFeather>();
        feather5.SetUp(this, Random.Range(a2Spd - 30, a2Spd), a2Range, CalculateSinergy(a2Damage));
        feather5.transform.localEulerAngles = new Vector3(feather5.transform.localEulerAngles.x, feather5.transform.localEulerAngles.y, feather5.transform.localEulerAngles.z - Random.Range(-a2Detour, a2Detour));

        controller.LockPointer(false);
        yield return StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, h1Area);
    }
}
