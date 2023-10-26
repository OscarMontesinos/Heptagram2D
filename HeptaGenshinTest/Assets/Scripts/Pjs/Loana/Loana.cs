using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
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
    public GameObject c6Particle;
    public float c6Area;
    public float c6Dmg;
    public override void Awake()
    {
        base.Awake(); 
        _animator = GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();

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
        base.MainAttack();
    }

    public override void StrongAttack()
    {
        if (!IsCasting())
        {
            StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd / strongAtSpdMultiplier)));
            _animator.Play("LoanaAttack3");
            base.StrongAttack();
        }
    }

    public void LoanaAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(a1Point.transform.position, a1Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().TakeDamage(CalculateSinergy(a1Dmg), HitData.Element.water);
            DamageDealed(this, enemy, HitData.Element.water, HitData.AttackType.melee, HitData.HabType.basic);

        }
    }
    public void LoanaStrongAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(a2Point.transform.position, new Vector2(a2Weight,a2Length),controller.pointer.transform.localEulerAngles.z, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().Stunn(a2Stunn);
            enemy.GetComponent<TakeDamage>().TakeDamage(CalculateSinergy(a2Dmg), HitData.Element.water);
            DamageDealed(this, enemy, HitData.Element.water, HitData.AttackType.melee, HitData.HabType.basic);

        }
    }

    public void AnimationCursorLock(int value)
    {
        if (value == 1)
        {
            controller.LockPointer(true);
        }
        else
        {
            controller.LockPointer(false);
        }
    }

    public override void OnGlobalDamageTaken()
    {
        if (CharacterManager.Instance.data[2].convergence >= 6)
        {
            int random = Random.Range(1, 4);
            if (random == 3)
            {
                Instantiate(c6Particle, transform);
                Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, c6Area, GameManager.Instance.enemyLayer);
                Enemy enemy;
                foreach (Collider2D enemyColl in enemiesHit)
                {
                    enemy = enemyColl.GetComponent<Enemy>();
                    enemy.GetComponent<TakeDamage>().TakeDamage(CalculateSinergy(a1Dmg), HitData.Element.water);
                    DamageDealed(this, enemy, HitData.Element.water, HitData.AttackType.aoe, HitData.HabType.hability);

                }
            }
        }

            base.OnGlobalDamageTaken();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(a1Point.transform.position, a1Area);
        Gizmos.DrawWireSphere(transform.position, c6Area);
        Gizmos.DrawWireCube(a2Point.transform.position, new Vector3(a2Weight, a2Length, 1));

    }
}
