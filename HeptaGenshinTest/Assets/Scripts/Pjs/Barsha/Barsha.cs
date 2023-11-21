using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Barsha : PjBase
{
    public Animator _animator; 
    public GameObject a1Point;
    public float a1Area;
    public float a1Dmg;
    int combo;

    public GameObject a2Point;
    bool a2Activated;
    bool a2Ready;
    public float a2Length;
    public float a2Weight;
    public float a2Dmg1;
    public float a2Dmg2;
    public float a2Dmg3;
    public float a2StunTime;

    public GameObject h1Particle;
    public Sprite h1Sprite1;
    public Sprite h1Sprite2;
    public float h1Area;
    public float h1Dmg;
    public float h1DashRange;
    public float h1DashSpd;
    float h1FervourDmg;
    public float h1FervourMaxDmg;
    public float h1FervourPerSeconds;
    public float h1FervourDuration;
    float h1FervourCurrentDuration;
    public float h1ShieldConversor;
    public float h1ShieldDuration;
    float h1FervourCount;
    bool h1Activated = true;


    public float h2DashRange;
    public float h2DashSpd;
    public float h2Area;
    public float h2Dmg;
    public float h2StunTime;
    bool h2Activated;

    public float c2AtSpdMod;
    float c2CurentAtSpdMod;
    public float c2StunnDuration;

    public float c4Dmg;
    public List<PjBase> c4EnemyList;

    public float c7DmgConversor;

    public override void Awake()
    {
        base.Awake();

        c2CurentAtSpdMod = stats.atSpd * c2AtSpdMod / 100;
    }

    public override void Update()
    {
        base.Update();
        if (h1FervourCurrentDuration > 0)
        {
            h1Activated = false;
            h1FervourCurrentDuration -= Time.deltaTime;
        }
        else if (!h1Activated)
        {
            h1Activated = true; 
            if (CharacterManager.Instance.data[6].convergence < 6)
            {
                h1Particle.SetActive(false);
            }
            if (CharacterManager.Instance.data[6].convergence >= 2)
                {
                    stats.atSpd -= c2CurentAtSpdMod;
                }
                if (controller.GetComponent<BarshaShield>())
                {
                    controller.GetComponent<BarshaShield>().Die();
                }
                BarshaShield shield = controller.AddComponent<BarshaShield>();
                shield.SetUp(this, CalculateControl(h1FervourCount * h1ShieldConversor), h1ShieldDuration + h1FervourCurrentDuration);
        }
        if(h1FervourDmg < h1FervourMaxDmg)
        {
            h1FervourDmg += Time.deltaTime * h1FervourPerSeconds;
        }
    }
    public override void MainAttack()
    {
        if (combo != 0 && currentComboReset <= 0)
        {
            combo = 0;
        }
        if (!IsCasting())
        {
            StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
            if (combo == 0)
            {
                _animator.Play("BarshaAttack1");
                combo++;
                currentComboReset = CalculateAtSpd(stats.atSpd) + 0.5f;
            }
            else if (combo == 1)
            {
                _animator.Play("BarshaAttack2");
                combo++;
                currentComboReset = CalculateAtSpd(stats.atSpd) + 0.5f;
            }
            else
            {
                _animator.Play("BarshaAttack3");
                combo = 0;
            }
        }
        base.MainAttack();
    }


    public void BarshaAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(a1Point.transform.position, a1Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a1Dmg), HitData.Element.nature);
            DamageDealed(this, enemy, HitData.Element.nature, HitData.AttackType.melee, HitData.HabType.basic);
        }
    }

    public override void StrongAttack()
    {
        if (!IsCasting())
        {
            if (!a2Ready)
            {
                StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd * strongAtSpdMultiplier)));
                _animator.Play("BarshaStrongAttack");
                combo = 1;
                currentComboReset = CalculateAtSpd(stats.atSpd) + 0.5f;
            }
            else
            {
                StartCoroutine(SpecialStrong());
                a2Activated = true;
                a2Ready = false;
            }
        }
            base.StrongAttack();
    }

    IEnumerator SpecialStrong()
    {
        yield return StartCoroutine(Cast(0.4f));
        _animator.Play("BarshaStrongSpecialAttack");
        StartCoroutine(Cast(0.4f));
        List<Enemy> enemiesHitted = new List<Enemy>();
        while (a2Activated)
        {
            yield return null;
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, h2Area, GameManager.Instance.enemyLayer);
            Enemy enemy;
            foreach (Collider2D enemyColl in enemiesHit)
            {
                enemy = enemyColl.GetComponent<Enemy>();
                if (!enemiesHitted.Contains(enemy))
                {
                    enemiesHitted.Add(enemy);
                    enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a2Dmg3), HitData.Element.nature);
                    DamageDealed(this, enemy, HitData.Element.nature, HitData.AttackType.melee, HitData.HabType.basic);
                    Stunn(enemy, a2StunTime);
                }
            }
        }
    }
    public void BarshaStrongAttack1()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(a1Point.transform.position, a1Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a2Dmg1), HitData.Element.nature);
            DamageDealed(this, enemy, HitData.Element.nature, HitData.AttackType.melee, HitData.HabType.basic);
        }
    }
    public void BarshaStrongAttack2()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(a2Point.transform.position, new Vector2(a2Weight, a2Length), controller.pointer.transform.localEulerAngles.z, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a2Dmg2), HitData.Element.nature);
            DamageDealed(this, enemy, HitData.Element.nature, HitData.AttackType.melee, HitData.HabType.basic);
        }
    }

    public void EndStrong2()
    {
        a2Activated = false;
    }

    public override void Hab1()
    {
        if (!IsCasting() && currentHab1Cd <= 0)
        {
            if (currentHab1Cd <= 0)
            {
                a2Ready = true;
                float range = h1DashRange;
                float speed = h1DashSpd;
                if (Physics2D.CircleCast(transform.position, 2, controller.pointer.transform.up, h1DashRange, GameManager.Instance.wallLayer + GameManager.Instance.enemyLayer))
                {
                    Vector2 dist = Physics2D.CircleCast(a1Point.transform.position, 1, controller.pointer.transform.up, h1DashRange, GameManager.Instance.wallLayer + GameManager.Instance.enemyLayer).point - new Vector2(transform.position.x, transform.position.y);
                    range = h1DashRange - ((h1DashRange - dist.magnitude) + 1.5f);
                    speed = h1DashSpd / (h1DashRange / range);
                    if (speed > h1DashSpd)
                    {
                        speed = h1DashSpd;
                    }
                    if (range > h1DashRange)
                    {
                        range = h1DashRange;
                    }
                }

                StartCoroutine(Dash(controller.pointer.transform.up, speed, range, false));
                StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
                _animator.Play("BarshaHabilityAttack");
                h1Particle.SetActive(true);
                currentHab1Cd = hab1Cd;
                h1FervourCurrentDuration = h1FervourDuration;
                h1FervourCount = 0;
                if (CharacterManager.Instance.data[6].convergence >= 2)
                {
                    stats.atSpd += c2CurentAtSpdMod;
                }
            }
        }
        base.Hab1();
    }

    public override void Hab2()
    {
        base.Hab2();
        if (!IsCasting() && currentHab2Cd <= 0)
        {
            h2Activated = true;
            currentHab2Cd = hab2Cd;
            if (CharacterManager.Instance.data[6].convergence >= 7)
            {
                if(h1FervourCurrentDuration > 0)
                {
                    h1FervourCurrentDuration = h1FervourDuration;
                    if (controller.GetComponent<BarshaShield>())
                    {
                        controller.GetComponent<BarshaShield>().Die();
                    }
                    BarshaShield shield = controller.AddComponent<BarshaShield>();
                    shield.SetUp(this, CalculateControl(h1FervourCount * h1ShieldConversor), h1ShieldDuration + h1FervourCurrentDuration);
                    h1FervourCount = 0;
                }
            }
                StartCoroutine(Hab2Corroutine());
        }
    }

    public void EndHab2()
    {
        h2Activated = false;
    }
    IEnumerator Hab2Corroutine()
    {
        Vector2 dist = UtilsClass.GetMouseWorldPosition() - transform.position;
        if (dist.magnitude < h2DashRange)
        {
            yield return StartCoroutine(Dash(controller.pointer.transform.up, h2DashSpd, dist.magnitude, false));
        }
        else
        {
            yield return StartCoroutine(Dash(controller.pointer.transform.up, h2DashSpd, h2DashRange, false));
        }
        GetComponent<Collider2D>().enabled = false;
        yield return StartCoroutine(Cast(0.4f));
        _animator.Play("BarshaUlt");
        StartCoroutine(Cast(0.4f));
        List<Enemy> enemiesHitted = new List<Enemy>();
        while (h2Activated)
        {
            yield return null;
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, h2Area, GameManager.Instance.enemyLayer);
            Enemy enemy;
            foreach (Collider2D enemyColl in enemiesHit)
            {
                enemy = enemyColl.GetComponent<Enemy>();
                if (!enemiesHitted.Contains(enemy))
                {
                    enemiesHitted.Add(enemy);
                    float dmg = h2Dmg;
                    if (CharacterManager.Instance.data[6].convergence >= 7)
                    {
                        dmg += Shield.shieldAmount * c7DmgConversor;
                    }
                        enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(dmg), HitData.Element.nature);
                    DamageDealed(this, enemy, HitData.Element.nature, HitData.AttackType.aoe, HitData.HabType.hability);
                    Stunn(enemy, h2StunTime);
                }
            }
        }
        yield return StartCoroutine(Cast(0.4f));
        GetComponent<Collider2D>().enabled = true;
    }

    public void BarshaHability()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(a1Point.transform.position, a1Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(h1Dmg), HitData.Element.nature);
            DamageDealed(this, enemy, HitData.Element.nature, HitData.AttackType.melee, HitData.HabType.basic);
            if (CharacterManager.Instance.data[6].convergence >= 2)
            {
                Stunn( enemy,c2StunnDuration);
            }
        }
        combo = 2;
        currentComboReset = CalculateAtSpd(stats.atSpd) + 0.5f;
    }

    public override void Interact(PjBase user, PjBase target, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {
        if (attackType == HitData.AttackType.melee && (h1FervourCurrentDuration > 0 || (CharacterManager.Instance.data[6].convergence >= 6 && controller.GetComponent<BarshaShield>())))
        {
            target.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(h1FervourDmg), HitData.Element.nature);
            h1FervourCount += CalculateSinergy(h1FervourDmg);
            DamageDealed(this, target, HitData.Element.nature, HitData.AttackType.passive, HitData.HabType.hability);
            h1FervourDmg = 0;
        }
        if(!c4EnemyList.Contains(target) && target.stunTime > 0)
        {
            target.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(c4Dmg), HitData.Element.nature);
            c4EnemyList.Add(target);
            DamageDealed(this, target, HitData.Element.nature, HitData.AttackType.passive, HitData.HabType.hability);
        }
        base.Interact(user, target, element, attackType, habType);
    }

    public override void OnGlobalStunn(PjBase target, float value)
    {
        c4EnemyList.Remove(target);
        base.OnGlobalStunn(target, value);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(a1Point.transform.position, a1Area);
        Gizmos.DrawWireCube(a2Point.transform.position, new Vector3(a2Weight, a2Length, 1));

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, h2Area);
    }
}
