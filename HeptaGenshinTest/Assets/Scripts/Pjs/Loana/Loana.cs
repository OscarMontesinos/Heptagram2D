using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Loana : PjBase
{
    Animator _animator;
    int combo;
    public GameObject a1Point;
    public float a1Area;
    public float a1Dmg;

    public GameObject a2Point;
    public float a2Length;
    public float a2Weight;
    public float a2Dmg;
    public float a2Stunn;


    public GameObject h1Wave;
    public float h1Dmg;
    public float h1Range;
    public float h1Spd;
    public float h1SpdOverTime;
    public float h1Duration;
    public float h1ShldAmount;

    public GameObject h2Bubble;
    public float h2Amount;
    public float h2Duration;

    public GameObject c6Particle;
    public float c3Area;
    public float c3Dmg;

    public float c4Heal;
    public float c4CurrentCd;
    public float c4Cd;

    public float c5BuffAmount;
    public float c6BuffAmount;

    public float c7DebuffAmount;
    public float c7Slow;
    public float c7StunnTime;
    public override void Awake()
    {
        base.Awake(); 
        _animator = GetComponent<Animator>();
    }

    public override void Start()
    {
        base.Start();
        StartCoroutine(PostStart());
    }

    IEnumerator PostStart()
    {
        yield return null;

        if (CharacterManager.Instance.data[id].convergence >= 4)
        {
            foreach (PjBase pj in GameManager.Instance.pjList)
            {
                if (pj.element == HitData.Element.water && pj != this )
                {
                    if (CharacterManager.Instance.data[id].convergence >= 6)
                    {
                        LoanaBuff buff = pj.AddComponent<LoanaBuff>();
                        buff.SetUp(this, CalculateControl(c5BuffAmount), CalculateControl(c6BuffAmount));
                    }
                    else
                    {
                        LoanaBuff buff = pj.AddComponent<LoanaBuff>();
                        buff.SetUp(this, CalculateControl(c5BuffAmount), CalculateControl(0));
                    }
                }
                else if (CharacterManager.Instance.data[id].convergence >= 6)
                {
                    LoanaBuff buff = pj.AddComponent<LoanaBuff>();
                    buff.SetUp(this, CalculateControl(0), CalculateControl(c6BuffAmount));
                }
            }
        }
    }


    public override void Update()
    {
        base.Update();

        if (c4CurrentCd > 0)
        {
            c4CurrentCd -= Time.deltaTime;
        }
    }

    public override void MainAttack()
    {
        base.MainAttack();
        if (combo != 0 && currentComboReset <= 0)
        {
            combo = 0;
        }
        if (!IsCasting())
        {
            StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
            if (combo == 0)
            {
                _animator.Play("LoanaAttack1");
                combo++;
                currentComboReset = CalculateAtSpd(stats.atSpd) + 0.5f;
            }
            else
            {

                _animator.Play("LoanaAttack2");
                combo = 0;
            }
        }
    }

    public override void StrongAttack()
    {
            base.StrongAttack();
        if (!IsCasting())
        {
            StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd / strongAtSpdMultiplier)));
            _animator.Play("LoanaAttack3");
        }
    }

    public void LoanaAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(a1Point.transform.position, a1Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a1Dmg), HitData.Element.water);
            DamageDealed(this, enemy, CalculateSinergy(a1Dmg), HitData.Element.water, HitData.AttackType.melee, HitData.HabType.basic);
            if (CharacterManager.Instance.data[id].convergence >= 1)
            {
                currentHab1Cd -= 1;
            }
        }
    }

    public void LoanaStrongAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(a2Point.transform.position, new Vector2(a2Weight, a2Length), controller.pointer.transform.localEulerAngles.z, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            Stunn(enemy,a2Stunn);
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a2Dmg), HitData.Element.water);
            DamageDealed(this, enemy, CalculateSinergy(a2Dmg), HitData.Element.water, HitData.AttackType.melee, HitData.HabType.basic);
            if (CharacterManager.Instance.data[id].convergence >= 1)
            {
                currentHab1Cd -= 1;
            }
            if (CharacterManager.Instance.data[id].convergence >= 4 && c4CurrentCd <=0)
            {
                foreach(PjBase pj in controller.team)
                {
                    pj.Heal(this,CalculateControl(c4Heal), element);
                }
                c4CurrentCd = CDR(c4Cd);
            }
        }
    }

    public override void Hab1()
    {
        base.Hab1();
        if (!IsCasting() && currentHab1Cd <= 0)
        {
            StartCoroutine(SoftCast(2));
            _animator.Play("LoanaWave");
        }
    }

    public override void Hab2()
    {
        if (!IsCasting() && currentHab2Cd <= 0)
        {
            base.Hab2();
            StartCoroutine(Cast(1));
            Bubble bubble = Instantiate(h2Bubble, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Bubble>();
            bubble.SetUp(this, CalculateControl(h2Amount), h2Duration);
            if (CharacterManager.Instance.data[id].convergence >= 2)
            {
                bubble.hp += Shield.shieldAmount;
            }
            currentHab2Cd = CDR(hab2Cd);
        }
    }
    public void CreateWave()
    {
        if (controller.GetComponent<LoanaShield>())
        {
            controller.GetComponent<LoanaShield>().Die();
        }

        LoanaShield shield = controller.AddComponent<LoanaShield>();
        shield.SetUp(this, 0, h1Duration);
        LoanaWave wave = Instantiate(h1Wave, transform.position, controller.pointer.transform.rotation).GetComponent<LoanaWave>();
        wave.SetUp(this,shield, h1Spd, h1SpdOverTime, h1Range, CalculateSinergy(h1Dmg), CalculateControl(h1ShldAmount));
        currentHab1Cd = CDR(hab1Cd);
    }


    

    public override void OnGlobalDamageTaken()
    {
        if (CharacterManager.Instance.data[id].convergence >= 2)
        {
            int random = Random.Range(1, 4);
            if (random == 3 ||  CharacterManager.Instance.data[id].convergence >= 6 && controller.GetComponent<LoanaShield>() && controller.GetComponent<LoanaShield>().singularShieldAmount > 0)
            {
                Instantiate(c6Particle, transform);
                Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, c3Area, GameManager.Instance.enemyLayer);
                Enemy enemy;
                foreach (Collider2D enemyColl in enemiesHit)
                {
                    enemy = enemyColl.GetComponent<Enemy>();
                    enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a1Dmg), HitData.Element.water);
                    DamageDealed(this, enemy, CalculateSinergy(a1Dmg), HitData.Element.water, HitData.AttackType.aoe, HitData.HabType.hability);
                }
            }
        }

        base.OnGlobalDamageTaken();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(a1Point.transform.position, a1Area);
        Gizmos.DrawWireSphere(transform.position, c3Area);
        Gizmos.DrawWireCube(a2Point.transform.position, new Vector3(a2Weight, a2Length, 1));

    }
}
