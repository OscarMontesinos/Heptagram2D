using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Adrik : PjBase
{
    public GameObject a1Bullet;
    public float a1Speed;
    public float a1Range;
    public float a1Dmg;
    bool a2Casting;
    float a2CastingTime;
    public float a2MaxCastingTime;
    public GameObject a2HealObject;
    public ParticleSystem a2Particle;
    public float a2HealRange;
    public float a2Heal;
    public float a2HealSpd;
    public GameObject a2FireBlast;
    public float a2Speed;
    public float a2Range;
    public float a2Dmg;
    public GameObject h1Piroclast;
    public List<GameObject> piroclastsVisulas = new List<GameObject>();
    public float h1Speed;
    public float h1Range;
    public float h1Dmg;
    public int h1MaxCharges;
    int h1Charges;
    public float h1MarkDuration;
    public float h1MarkHeal;
    public float h1MarkDmg;
    public float c3CdReduc;
    public GameObject h2Bomb;
    public GameObject h2BombSpawner;
    public float h2CastTime;
    public float h2Speed;
    public float h2Dmg;
    public float h2Range;

    public override void Start()
    {
        base.Start();
        UpdatePiroclasts();
    }

    public override void MainAttack()
    {
        if (!IsCasting())
        {
            base.MainAttack();
            if (h1Charges <= 0)
            {
                StartCoroutine(AdrikNormalAttack());
                StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
            }
            else
            {

                Piroclast();
                if (CharacterManager.Instance.data[3].convergence >= 1)
                {
                    StartCoroutine(SoftCast(CalculateAtSpd((stats.atSpd * 1.5f)*1.5f)));
                }
                else
                {
                    StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd * 1.5f)));
                }
                h1Charges--;
                UpdatePiroclasts();
            }
        }
    }

    IEnumerator AdrikNormalAttack()
    {
        FireBullet bullet = Instantiate(a1Bullet, transform.position, controller.pointer.transform.rotation).GetComponent<FireBullet>();
        bullet.SetUp(this, a1Speed, a1Range, CalculateSinergy(a1Dmg));

        yield return new WaitForSeconds(CalculateAtSpd(stats.atSpd * 4.5f));

        FireBullet bullet2 = Instantiate(a1Bullet, transform.position, controller.pointer.transform.rotation).GetComponent<FireBullet>();
        bullet2.SetUp(this, a1Speed, a1Range, CalculateSinergy(a1Dmg));

        yield return new WaitForSeconds(CalculateAtSpd(stats.atSpd * 4.5f));

        FireBullet bullet3 = Instantiate(a1Bullet, transform.position, controller.pointer.transform.rotation).GetComponent<FireBullet>();
        bullet3.SetUp(this, a1Speed, a1Range, CalculateSinergy(a1Dmg));
    }
    void Piroclast()
    {
        Piroclast piroclast = Instantiate(h1Piroclast, transform.position, controller.pointer.transform.rotation).GetComponent<Piroclast>();
        piroclast.SetUp(this, h1Speed, h1Range, CalculateSinergy(h1Dmg));
    }
    void UpdatePiroclasts()
    {
        int count = h1Charges;
        foreach (GameObject piroclast in piroclastsVisulas)
        {
            if(count > 0)
            {
                piroclast.SetActive(true);
            }
            else
            {
                piroclast.SetActive(false);
            }
            count--;
        }
    }

    public override void StrongAttack()
    {
        base.StrongAttack();
        if (!IsCasting())
        {
            StartCoroutine(AdrikStrongAttack());
        }
    }

    IEnumerator AdrikStrongAttack()
    {
        a2Casting = true;
        a2CastingTime = 0;
        softCasting = true;
        a2Particle.Play();
        while(a2CastingTime < a2MaxCastingTime)
        {
            yield return null;
            if (Input.GetMouseButtonDown(1))
            {
                controller.LockPointer(true);
                a2Casting = false;
                softCasting = false;
                a2Particle.Stop();
                yield return StartCoroutine(Cast(CalculateAtSpd(stats.atSpd * strongAtSpdMultiplier)));
                FireBlast piroclast = Instantiate(a2FireBlast, transform.position, controller.pointer.transform.rotation).GetComponent<FireBlast>();
                piroclast.SetUp(this, a2Speed, a2Range, CalculateSinergy(a2Dmg)); 
                controller.LockPointer(false);
                yield break;
            }
            a2CastingTime+=Time.deltaTime;
        }

        a2Particle.Stop();
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, a2HealRange, GameManager.Instance.enemyLayer);
        foreach (Collider2D enemyColl in enemiesHit)
        {
            if (enemyColl.gameObject.GetComponent<AdrikMark>())
            {
                enemyColl.gameObject.GetComponent<AdrikMark>().Die();
                CreateHeal(enemyColl.GetComponent<Enemy>(), CalculateControl(a2Heal));
            }
        }

        a2Casting = false;
        softCasting = false;
    }


    public override void Hab1()
    {
        base.Hab1();
        if (!IsCasting() && currentHab1Cd <= 0)
        {
            StartCoroutine(Cast(0.75f));
            currentHab1Cd = CDR(hab1Cd);
            h1Charges = h1MaxCharges;
            UpdatePiroclasts();
        }
    }

    public override void Hab2()
    {
        base.Hab2();
        if (!IsCasting() && currentHab2Cd <= 0)
        {
            List<Enemy> enemyList = new List<Enemy>();
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                Vector2 dist = enemy.transform.position - transform.position;
                if (dist.magnitude < h2Range && !Physics2D.Raycast(transform.position, dist, dist.magnitude, GameManager.Instance.wallLayer))
                {
                    enemyList.Add(enemy);
                }
            }
            if (enemyList.Count > 0)
            {
                currentHab2Cd = CDR(hab2Cd);
                StartCoroutine(FireBombing(enemyList));
            }
        }

    }

    IEnumerator FireBombing(List<Enemy> enemyList)
    {
        float time = h2CastTime / 2;
        if(enemyList.Count <= 3)
        {
            time /= 3;
        }
        else if(enemyList.Count <= 6)
        {
            time /= 2;
        }
        GetComponent<Collider2D>().enabled = false;
        List<FireBomb> bombList = new List<FireBomb>();
        casting = true;
        foreach (Enemy enemy in enemyList)
        {
            Vector2 dist = enemy.transform.position - transform.position;
            h2BombSpawner.transform.parent.up = dist;
            FireBomb bomb = Instantiate(h2Bomb, h2BombSpawner.transform.position, h2BombSpawner.transform.rotation).GetComponent<FireBomb>();
            bomb.SetUp(this, 0, dist.magnitude, CalculateSinergy(h2Dmg));
            bombList.Add(bomb);
            yield return new WaitForSeconds(time/enemyList.Count);

            if (CharacterManager.Instance.data[3].convergence >= 7)
            {
                if (enemyList.Count <= 3)
                {
                    h2BombSpawner.transform.parent.localEulerAngles = new Vector3(h2BombSpawner.transform.parent.localEulerAngles.x, h2BombSpawner.transform.parent.localEulerAngles.y, h2BombSpawner.transform.parent.localEulerAngles.z + 10);

                    FireBomb bomb2 = Instantiate(h2Bomb, h2BombSpawner.transform.position, h2BombSpawner.transform.rotation).GetComponent<FireBomb>();
                    bomb2.SetUp(this, 0, dist.magnitude, CalculateSinergy(h2Dmg/2));
                    bombList.Add(bomb2);

                    yield return new WaitForSeconds(time / enemyList.Count);

                    h2BombSpawner.transform.parent.localEulerAngles = new Vector3(h2BombSpawner.transform.parent.localEulerAngles.x, h2BombSpawner.transform.parent.localEulerAngles.y, h2BombSpawner.transform.parent.localEulerAngles.z - 20);
                    FireBomb bomb3 = Instantiate(h2Bomb, h2BombSpawner.transform.position, h2BombSpawner.transform.rotation).GetComponent<FireBomb>();
                    bomb3.SetUp(this, 0, dist.magnitude, CalculateSinergy(h2Dmg/2));
                    bombList.Add(bomb3);

                    yield return new WaitForSeconds(time / enemyList.Count);
                }
                else if (enemyList.Count <= 6)
                {
                    int random = Random.Range(0, 2);
                    if (random == 1)
                    {

                        h2BombSpawner.transform.parent.localEulerAngles = new Vector3(h2BombSpawner.transform.parent.localEulerAngles.x, h2BombSpawner.transform.parent.localEulerAngles.y, h2BombSpawner.transform.parent.localEulerAngles.z + 10);

                        FireBomb bomb2 = Instantiate(h2Bomb, h2BombSpawner.transform.position, h2BombSpawner.transform.rotation).GetComponent<FireBomb>();
                        bomb2.SetUp(this, 0, dist.magnitude, CalculateSinergy(h2Dmg / 2));
                        bombList.Add(bomb2);

                        yield return new WaitForSeconds(time / enemyList.Count);
                    }
                    else
                    {
                        h2BombSpawner.transform.parent.localEulerAngles = new Vector3(h2BombSpawner.transform.parent.localEulerAngles.x, h2BombSpawner.transform.parent.localEulerAngles.y, h2BombSpawner.transform.parent.localEulerAngles.z - 10);

                        FireBomb bomb2 = Instantiate(h2Bomb, h2BombSpawner.transform.position, h2BombSpawner.transform.rotation).GetComponent<FireBomb>();
                        bomb2.SetUp(this, 0, dist.magnitude, CalculateSinergy(h2Dmg / 2));
                        bombList.Add(bomb2);

                        yield return new WaitForSeconds(time / enemyList.Count);
                    }
                }
            }
        }

        yield return StartCoroutine(Cast(h2CastTime/2));
        foreach(FireBomb bomb in bombList)
        {
            bomb.speed = h2Speed;
        }
        GetComponent<Collider2D>().enabled = true;
    }

    public void CreateHeal(Enemy target, float heal)
    {
        AdrikHeal aHeal = Instantiate(a2HealObject, target.transform.position, target.transform.rotation).GetComponent<AdrikHeal>();
        aHeal.SetUp(this, a2HealSpd, heal);
    }
    public override void OnKill(Enemy enemy)
    {
        base.OnKill(enemy); 
        if (CharacterManager.Instance.data[3].convergence >= 4 && stats.hp >0)
        {
            CreateHeal(enemy, CalculateControl(a2Heal));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, a2HealRange);
        Gizmos.DrawWireSphere(transform.position, h2Range);

    }

}
