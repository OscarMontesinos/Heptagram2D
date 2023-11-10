using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HunterBoss : Enemy
{
    Animator animator;
    public GameObject attackPoint;
    public float attackDmg;
    public float attackRange;
    public float maxDashRange;
    public float dashRangeMod;
    public float dashSpd;
    bool attacking;

    public GameObject axe;
    public float axeDmg;
    public float axeRange;
    public float axeSpd;
    public float axeKnockRange;
    public float axeKnockSpd;
    public float axeStunn;

    float reachTime;

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }
    public override void Start()
    {
        base.Start();
        PlayerController.Instance.targetBoss = gameObject;
        AI();
    }
    public override void Update()
    {
        if (stunTime > 0)
        {
            stunTime -= Time.deltaTime; 
            if (stunnBar.maxValue < stunTime)
            {
                stunnBar.maxValue = stunTime;
            }

            stunnBar.value = stunTime;
        }
        else
        {
            stunnBar.maxValue = 0.3f;
            stunnBar.value = 0;
        }
    

        reachTime += Time.deltaTime;

        if (target == null)
        {
            foreach (PjBase pj in GameManager.Instance.pjList)
            {
                if (pj != null && pj.isActive)
                {
                    Vector2 dist = pj.transform.position - transform.position;
                    if (dist.magnitude < viewDist && !Physics2D.Raycast(transform.position, dist, dist.magnitude, GameManager.Instance.wallLayer))
                    {
                        target = pj;
                    }
                }
            }
        }

        if (target != null)
        {
            Vector2 dist = target.transform.position - transform.position;


            if (point)
            {
                pointer.transform.up = dist;
            }
        }
    }
    public override void AI()
    {
        animator.Play("IdleBoss");
        if (target != null && stunTime <= 0)
        {
            Vector2 dir = target.transform.position - transform.position;
            if (Physics2D.Raycast(transform.position, dir, dir.magnitude, GameManager.Instance.wallLayer))
            {
                StartCoroutine(ComboReach());
            }
            else if (Random.Range(0, 2) == 1)
            {
                StartCoroutine(ComboAxe());
            }
            else
            {
                if (dir.magnitude > maxDashRange - dashRangeMod)
                {
                    StartCoroutine(Combo());
                }
                else
                {
                    transform.Translate(dir.normalized * stats.spd * Time.deltaTime);
                    StartCoroutine(RestartAi());
                }
            }
        }
        else
        {
            StartCoroutine(RestartAi());
        }
        base.AI();
    }

    public void DashToPlayer()
    {
        Vector2 dir = target.transform.position - transform.position;

        if(dir.magnitude > maxDashRange-dashRangeMod)
        {
            StartCoroutine(Dash(dir, dashSpd, maxDashRange, true));

        }
        else
        {
            StartCoroutine(Dash(dir, dashSpd, dir.magnitude-dashRangeMod, true));
        }
    }
    public void DashToPlayerWithoutRange()
    {
        Vector2 dir = target.transform.position - transform.position;


        StartCoroutine(Dash(dir, dashSpd, dir.magnitude - dashRangeMod, true));
    }
    public void DashRandomDirection()
    {
        if (Random.Range(0, 2) == 1)
        {
            StartCoroutine(Dash(pointer.transform.right, dashSpd, maxDashRange, true));
        }
        else
        {
            StartCoroutine(Dash(-pointer.transform.right, dashSpd, maxDashRange/2, true));
        }
    }

    public void AttackDmg()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, GameManager.Instance.playerLayer);
        PjBase enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<PjBase>();
            enemy.GetComponent<TakeDamage>().TakeDamage(CalculateSinergy(attackDmg), HitData.Element.ice);
        }
    }

    public void ThrowAxe()
    {
        AxeBoss axe = Instantiate(this.axe,attackPoint.transform.position,pointer.transform.rotation).GetComponent<AxeBoss>();
        axe.SetUp(this, axeSpd, axeRange, CalculateSinergy(axeDmg), axeStunn, axeKnockSpd, axeKnockRange);
    }
    public void ThrowSlowAxe()
    {
        AxeBoss axe = Instantiate(this.axe,attackPoint.transform.position,pointer.transform.rotation).GetComponent<AxeBoss>();
        axe.SetUp(this, axeSpd*0.5f, axeRange, CalculateSinergy(axeDmg), axeStunn*1.5f, axeKnockSpd, axeKnockRange);
    }

    IEnumerator Combo()
    {
        DashToPlayer();
        attacking = true;
        animator.Play("BossHit1");
        while (attacking || stunTime > 0)
        {
            yield return null;
        }
        DashToPlayer();
        attacking = true;
        animator.Play("BossHit2");
        while (attacking || stunTime > 0)
        {
            yield return null;
        }
        DashToPlayer();
        attacking = true;
        animator.Play("BossHit3");
        while (attacking || stunTime > 0)
        {
            yield return null;
        }
        DashToPlayer();
        attacking = true;
        animator.Play("BossHit4");
        while (attacking || stunTime > 0)
        {
            yield return null;
        }
        attacking = true;
        animator.Play("BossAxeThrow");
        while (attacking || stunTime > 0)
        {
            yield return null;
        }
        animator.Play("IdleBoss");
        yield return StartCoroutine(Cast(1.5f));
        AI();

    }

    IEnumerator ComboAxe()
    {
        Vector2 dir = target.transform.position - transform.position;

        if (dir.magnitude > maxDashRange*1.5f)
        {
            DashToPlayer();
        }
        else
        {
            DashRandomDirection();
        }
        yield return StartCoroutine(Cast(0.3f));
        attacking = true;
        animator.Play("BossQuickAxeThrow");
        while (attacking || stunTime > 0)
        {
            yield return null;
        }
        DashToPlayerWithoutRange();
        attacking = true;
        animator.Play("BossHit2");
        while (attacking || stunTime > 0)
        {
            yield return null;
        }
        animator.Play("IdleBoss");
        yield return StartCoroutine(Cast(1f));
        AI();
    }
    IEnumerator ComboReach()
    {
        Vector2 dir = target.transform.position - transform.position;
        reachTime = 0;
        if (Random.Range(0, 2) == 1)
        {
            while (Physics2D.Raycast(transform.position, dir, dir.magnitude, GameManager.Instance.wallLayer))
            {
                if (((int)reachTime) % 2 == 0)
                {
                    yield return StartCoroutine(Dash(pointer.transform.right, dashSpd, maxDashRange / 2, true));
                }
                else
                {
                    yield return StartCoroutine(Dash(-pointer.transform.right, dashSpd, maxDashRange / 2, true));
                }
                dir = target.transform.position - transform.position;

            }
            if (((int)reachTime) % 2 == 0)
            {
                yield return StartCoroutine(Dash(pointer.transform.right, dashSpd, maxDashRange / 2, true));
            }
            else
            {
                yield return StartCoroutine(Dash(-pointer.transform.right, dashSpd, maxDashRange / 2, true));
            }
        }
        else
        {
            while (Physics2D.Raycast(transform.position, dir, dir.magnitude, GameManager.Instance.wallLayer))
            {
                if (((int)reachTime) % 2 == 0)
                {
                    yield return StartCoroutine(Dash(-pointer.transform.right, dashSpd, maxDashRange / 2, true));
                }
                else
                {
                    yield return StartCoroutine(Dash(pointer.transform.right, dashSpd, maxDashRange / 2, true));
                }

                dir = target.transform.position - transform.position;


            }
            if (((int)reachTime) % 2 == 0)
            {
                yield return StartCoroutine(Dash(-pointer.transform.right, dashSpd, maxDashRange / 2, true));
            }
            else
            {
                yield return StartCoroutine(Dash(pointer.transform.right, dashSpd, maxDashRange / 2, true));
            }
        }

        yield return StartCoroutine(Cast(0.3f));
        attacking = true;
        animator.Play("BossQuickAxeThrow");
        while (attacking || stunTime > 0)
        {
            yield return null;
        }
        DashToPlayerWithoutRange();
        attacking = true;
        animator.Play("BossHit2");
        while (attacking || stunTime > 0)
        {
            yield return null;
        }
        animator.Play("IdleBoss");
        yield return StartCoroutine(Cast(1f));
        AI();
    }

    public void AttackingOff()
    {
        attacking = false;
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }
}
