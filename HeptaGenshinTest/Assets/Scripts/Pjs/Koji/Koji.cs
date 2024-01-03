using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst.Intrinsics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Koji : PjBase
{
    public Animator _animator;
    public GameObject a1Point;
    public float a1Area;
    public float a1Dmg;
    int combo;

    public GameObject a2Point;
    public float a2Area;
    public float a2Dmg;
    public float a2DashSpd;
    public float a2DashRange;

    public GameObject h1Knife;
    public KojiKnife currentKnife;
    public float h1Dmg;
    public float h1DmgAttack;
    public float h1Spd;
    public float h1DashSpd;
    public float h1Range;
    public float h1ExtraRange;
    public float h1Detour;

    public Enemy h2Target;
    public ParticleSystem h2Particle;
    public float h2Dmg;
    public float h2DashSpd;
    public float h2Range;

    public float c1Area;

    public float c2Stun;

    public float c4AtSpdMod;
    float c4CurentAtSpdMod;
    public float c4CdReduc;

    public ParticleSystem c7Particle33;
    public ParticleSystem c7Particle66;
    public ParticleSystem c7Particle;
    public int c7MaxPremeditation;
    int c7Premeditation;
    public float c7MaxDmg;
    int c7PremeditationParticleCounter;


    public override void Awake()
    {
        base.Awake();

        c4CurentAtSpdMod = stats.atSpd * c4AtSpdMod / 100;
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
                _animator.Play("KojiAttack1");
                combo++;
                currentComboReset = CalculateAtSpd(stats.atSpd) + 0.5f;
            }
            else
            {

                _animator.Play("KojiAttack2");
                combo = 0;
            }
        }
        base.MainAttack();
    }

    public void KojiAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(a1Point.transform.position, a1Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a1Dmg), HitData.Element.wind);
            DamageDealed(this, enemy, HitData.Element.wind, HitData.AttackType.melee, HitData.HabType.basic);
            if (CharacterManager.Instance.data[4].convergence >= 4 && currentKnife == null)
            {
                currentHab2Cd -= c4CdReduc;
            }
            if (currentKnife != null)
            {
                currentKnife.targetList.Add(enemy);
                if (CharacterManager.Instance.data[5].convergence >= 5)
                {
                    currentKnife.targetList.Add(enemy);
                    currentKnife.targetList.Add(enemy);
                }
            }
        }
    }
    public override void StrongAttack()
    {
        if (!IsCasting())
        {
            float range = a2DashRange;
            float speed = a2DashSpd;
            if (Physics2D.CircleCast(transform.position, 2, controller.pointer.transform.up, a2DashRange, GameManager.Instance.wallLayer + GameManager.Instance.enemyLayer))
            {
                Vector2 dist = Physics2D.CircleCast(a2Point.transform.position, 1, controller.pointer.transform.up, a2DashRange, GameManager.Instance.wallLayer + GameManager.Instance.enemyLayer).point - new Vector2(transform.position.x, transform.position.y);
                range = a2DashRange - ((a2DashRange - dist.magnitude) + 1.5f);
                speed = a2DashSpd / (a2DashRange / range);
                if (speed > a2DashSpd)
                {
                    speed = a2DashSpd;
                }
                if (range > a2DashRange)
                {
                    range = a2DashRange;
                }
            }

            StartCoroutine(Dash(controller.pointer.transform.up, speed, range, false));
            StartCoroutine(SoftCast(CalculateAtSpd(strongAtSpdMultiplier)));
            _animator.Play("KojiStrongAttack");
        }
        base.StrongAttack();
    }
    public void KojiStrongAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(a2Point.transform.position, a2Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a2Dmg), HitData.Element.wind);
            DamageDealed(this, enemy, HitData.Element.wind, HitData.AttackType.melee, HitData.HabType.basic);
            if (CharacterManager.Instance.data[4].convergence >= 4 && currentKnife == null)
            {
                currentHab2Cd -= c4CdReduc;
            }
            if (currentKnife != null)
            {
                currentKnife.targetList.Add(enemy);
                if (CharacterManager.Instance.data[5].convergence >= 5)
                {
                    currentKnife.targetList.Add(enemy);
                    currentKnife.targetList.Add(enemy);
                }
            }
        }
        combo = 1;
        currentComboReset = CalculateAtSpd(stats.atSpd) + 0.5f;
    }

    public override void Hab1()
    {
        base.Hab1();
        if (!IsCasting() && !dashing && currentHab1Cd <= 0)
        {
            if (currentKnife == null)
            {
                KojiKnife knife = Instantiate(h1Knife, transform.position, controller.pointer.transform.rotation).GetComponent<KojiKnife>();
                knife.SetUp(this, CalculateSinergy(h1Dmg), h1Spd, h1Detour, h1ExtraRange, h1Range);
                currentKnife = knife;
                if (CharacterManager.Instance.data[5].convergence >= 4)
                {
                    stats.atSpd += c4CurentAtSpdMod;
                }
                StartCoroutine(SoftCast(0.5f));

                currentHab1Cd = CDR(hab1Cd);

            }
            else
            {
                StartCoroutine(DashToKnife());
                if (CharacterManager.Instance.data[4].convergence >= 4)
                {
                    stats.atSpd -= c4CurentAtSpdMod;
                }
                if (CharacterManager.Instance.data[4].convergence < 6)
                {
                    currentHab1Cd = CDR(hab1Cd);
                }
            }
        }
    }


    IEnumerator DashToKnife()
    {
        currentKnife.targetList.Clear();
        currentKnife.dashing = false;
        Vector2 dir = currentKnife.transform.position - transform.position;
        controller.LockPointer(true);
        controller.pointer.transform.up = dir;
        _animator.Play("KojiHabilityDash");
        if (CharacterManager.Instance.data[4].convergence >= 1)
        {
            StartCoroutine(Dash(dir, h1DashSpd, dir.magnitude, false, true));
            List<PjBase> enemiesHitted = new List<PjBase>();

            while (dashing)
            {
                Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, c1Area, GameManager.Instance.enemyLayer);
                Enemy enemy;
                foreach (Collider2D enemyColl in enemiesHit)
                {
                    enemy = enemyColl.GetComponent<Enemy>();
                    if (!enemiesHitted.Contains(enemy))
                    {
                        enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(h1DmgAttack), HitData.Element.wind);
                        DamageDealed(this, enemy, HitData.Element.wind, HitData.AttackType.melee, HitData.HabType.hability);

                        enemiesHitted.Add(enemy);
                    }
                }
                yield return null;
            }
        }
        else
        {
            yield return Dash(dir, h1DashSpd, dir.magnitude, false, true);
        }
        Destroy(currentKnife.gameObject);
        _animator.Play("KojiHabilityAttack");
        GetComponent<Collider2D>().enabled = false;
        yield return StartCoroutine(Cast(1.1f));
        GetComponent<Collider2D>().enabled = true;
    }

    public void KojiHabilityAttack()
    {

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(a2Point.transform.position, a2Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(h1DmgAttack), HitData.Element.wind);
            DamageDealed(this, enemy, HitData.Element.wind, HitData.AttackType.melee, HitData.HabType.basic);
            if (CharacterManager.Instance.data[4].convergence >= 4 && currentKnife == null)
            {
                currentHab2Cd -= 1;
            }
            
        }
        combo = 1;
        currentComboReset = CalculateAtSpd(stats.atSpd) + 0.5f;

    }

    public override void Hab2()
    {
        base.Hab2();
        if (!IsCasting())
        {
            h2Particle.Play();
            _animator.Play("KojiUltPreparation");
            StartCoroutine(Cast(1));
            if (Physics2D.CircleCast(transform.position, 1, controller.pointer.transform.up, h2DashSpd, GameManager.Instance.enemyLayer))
            {
                GetComponent<Collider2D>().enabled = false;
                RaycastHit2D ray = Physics2D.CircleCast(transform.position, 1, controller.pointer.transform.up, h2DashSpd, GameManager.Instance.enemyLayer);
                StartCoroutine(KojiUlt1(ray.collider.gameObject.GetComponent<Enemy>()));
            }

            
        }
    }

    IEnumerator KojiUlt1(Enemy target)
    {
        yield return new WaitForSeconds(1);


        float range = h2Range;
        Vector2 dist = Physics2D.CircleCast(a2Point.transform.position, 1, controller.pointer.transform.up, h2DashSpd, GameManager.Instance.enemyLayer).point - new Vector2(transform.position.x, transform.position.y);
        range = h2Range - ((h2Range - dist.magnitude) + 1.5f);

        currentHab2Cd = CDR(hab2Cd);
        h2Target = target;
        controller.pointer.transform.up = h2Target.transform.position - transform.position;
        controller.LockPointer(true);


        yield return StartCoroutine(Dash(controller.pointer.transform.up, h2DashSpd, range, false));

        controller.pointer.transform.up = h2Target.transform.position - transform.position;
        _animator.Play("KojiUlt1");
        GetComponent<Collider2D>().enabled = false;
        yield return StartCoroutine(Cast(1));
        if (CharacterManager.Instance.data[5].convergence >= 7 && c7Premeditation > 0 && h2Target != null)
        {

            yield return StartCoroutine(Dash(-controller.pointer.transform.up, h2DashSpd, h2Range, false));

            GetComponent<Collider2D>().enabled = false;

            h2Particle.Play();

            yield return StartCoroutine(Cast(0.4f));

            range = h2Range;
            dist = h2Target.transform.position - transform.position;
            controller.pointer.transform.up = dist;
            range = h2Range - ((h2Range - dist.magnitude) + 1.5f);

            currentHab2Cd = CDR(hab2Cd);
            controller.LockPointer(true);


            _animator.Play("KojiUlt2");

            yield return StartCoroutine(Dash(controller.pointer.transform.up, h2DashSpd, range, false));

            GetComponent<Collider2D>().enabled = false;
            dist = h2Target.transform.position - transform.position;
            controller.pointer.transform.up = dist;


            yield return StartCoroutine(Cast(1));

        }

        GetComponent<Collider2D>().enabled = true;
    }

    public void Ult1Dmg()
    {
        if (h2Target != null) 
        {
            h2Target.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(h2Dmg/2), HitData.Element.wind);
            DamageDealed(this, h2Target, HitData.Element.wind, HitData.AttackType.melee, HitData.HabType.hability);

            if (CharacterManager.Instance.data[5].convergence >= 2)
            {
                Stunn(h2Target, c2Stun);
            }

                if (currentKnife != null)
            {
                currentKnife.targetList.Add(h2Target);
                if (CharacterManager.Instance.data[5].convergence >= 5)
                {
                    currentKnife.targetList.Add(h2Target);
                    currentKnife.targetList.Add(h2Target);
                }
            }
        } 
    }
    public void Ult2Dmg()
    {
        if (h2Target != null) 
        {
            float dmg = c7MaxDmg * Mathf.InverseLerp(0,c7MaxPremeditation,c7Premeditation);
            c7Premeditation = 0;
            h2Target.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(dmg), HitData.Element.wind);
            DamageDealed(this, h2Target, HitData.Element.wind, HitData.AttackType.melee, HitData.HabType.hability);

            c7Particle.Play(); 
            
            if (currentKnife != null)
            {
                currentKnife.targetList.Add(h2Target);
                if (CharacterManager.Instance.data[5].convergence >= 5)
                {
                    currentKnife.targetList.Add(h2Target);
                    currentKnife.targetList.Add(h2Target);
                }
            }
        } 
    }


    public override void TakeDmg(PjBase user, float value, HitData.Element element)
    {
        if (CharacterManager.Instance.data[5].convergence >= 4 && currentKnife == null)
        {
            value /= 2;
        }
            base.TakeDmg(user, value, element);
        
    }

    public override void Interact(PjBase user, PjBase target, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {
        if(user == this && CharacterManager.Instance.data[5].convergence >= 7)
        {
            c7Premeditation++;
            if(c7Premeditation > c7MaxPremeditation)
            {
                c7Premeditation = c7MaxPremeditation;
                c7PremeditationParticleCounter++;
                if(c7PremeditationParticleCounter >= 10)
                {
                    c7PremeditationParticleCounter = 0;
                    h2Particle.Play();
                }
            }
            else if(c7Premeditation == c7MaxPremeditation / 3)
            {
                c7Particle33.Play();
            }
            else if(c7Premeditation == (c7MaxPremeditation / 3)*2)
            {

                c7Particle66.Play();
            }
            else if(c7Premeditation == c7MaxPremeditation)
            {
                h2Particle.Play();
            }

        }
        base.Interact(user, target, element, attackType, habType);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(a1Point.transform.position, a1Area);
        Gizmos.DrawWireSphere(a2Point.transform.position, a2Area);
        Gizmos.DrawWireSphere(transform.position, c1Area);

    }
}
