using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using TMPro;

public class Noel : PjBase
{
    public Animator _animator;
    public LineRenderer a1RayLine;
    public GameObject a1EndPoint;
    public float a1Weight;
    public float a1Range;
    public float a1Dmg;
    public GameObject h1Cuckoo;
    public List<CuckooTurret> h1CuckooList = new List<CuckooTurret>();
    public int h1MaxCount;
    int h1Count;
    public float h1Spd;
    public float h1BulletSpd;
    public float h1Range;
    public float h1TurretRange;
    public int h1BulletCount;
    public float h1Dmg;
    public float h1AtSpdMod;
    public float c4Slow;
    public float c4Duration;
    public int c2ExtraBullets;
    public GameObject h2Display1;
    public GameObject h2Display2;
    public GameObject h2Display3;
    public float h2Duration;
    public float h2StunnDuration;
    public float h2Dmg;
    public float h2Delay;
    public float h2CastTime;
    List<NoelPot> c3ActualPot = new List<NoelPot>();
    public GameObject c3Lights;
    public GameObject c3Light1;
    public GameObject c3Light2;
    public GameObject c3Light3;
    public float c3Pot;
    public override void Start()
    {
        base.Start();
        if (CharacterManager.Instance.data[4].convergence >= 1)
        {
            h1MaxCount++;
        }
        if (CharacterManager.Instance.data[4].convergence >= 2)
        {
            h1BulletCount += c2ExtraBullets;
        }
        StartCoroutine(PostStart());
    }

    IEnumerator PostStart()
    {
        yield return null;
        if (CharacterManager.Instance.data[4].convergence >= 3)
        {
            c3Lights.SetActive(true);
            c3Light1.SetActive(false);
            c3Light2.SetActive(false);
            c3Light3.SetActive(false);
            foreach (PjBase pj in GameManager.Instance.pjList)
            {
                NoelPot buff = pj.AddComponent<NoelPot>();
                buff.SetUp(this, CalculateControl(0));
                c3ActualPot.Add(buff);
            }
        }
        else
        {
            c3Lights.SetActive(false);
        }
    }
    public override void MainAttack()
    {
        base.MainAttack();
        if (!IsCasting())
        {
            StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
            Vector2 dist = UtilsClass.GetMouseWorldPositionWithZ() - transform.position;

            if (Physics2D.CircleCast(transform.position, a1Weight, dist, a1Range, GameManager.Instance.enemyLayer + GameManager.Instance.wallLayer))
            {
                RaycastHit2D ray2D = Physics2D.CircleCast(transform.position, a1Weight, dist, a1Range, GameManager.Instance.enemyLayer + GameManager.Instance.wallLayer);
                Vector3[] array = new Vector3[2];
                array[0] = new Vector3(transform.position.x, transform.position.y,0);
                array[1] = new Vector3(ray2D.point.x, ray2D.point.y,0);
                a1RayLine.SetPositions(array);

                if (ray2D.collider.GetComponent<Enemy>())
                {
                    ray2D.collider.GetComponent<TakeDamage>().TakeDamage(CalculateSinergy(a1Dmg), HitData.Element.lightning);
                    DamageDealed(this, ray2D.collider.GetComponent<Enemy>(), HitData.Element.lightning, HitData.AttackType.melee, HitData.HabType.basic);
                }

                StartCoroutine(MainRay());
            }
            else
            {
                Vector3[] array = new Vector3[2];
                array[0] = new Vector3(transform.position.x, transform.position.y, 0);
                array[1] = new Vector3(a1EndPoint.transform.position.x, a1EndPoint.transform.position.y,0);
                a1RayLine.SetPositions(array);
                StartCoroutine(MainRay());
            }
        }
    }

    public override void StrongAttack()
    {
        base.StrongAttack(); 
        if (!IsCasting())
        {
            StartCoroutine(Cast(CalculateAtSpd(stats.atSpd * strongAtSpdMultiplier)));
            _animator.Play("NoelStrongAttack");
        }
    }

    public void ReplaceCuckoos()
    {
        if (h1Count > 0)
        {
            h1CuckooList[0].position = c3Light1.transform.position;
        }
        if (h1Count > 1)
        {
            h1CuckooList[1].position = c3Light2.transform.position;
        }
        if (h1Count > 2)
        {
            h1CuckooList[2].position = c3Light3.transform.position;
        }
    }

    IEnumerator MainRay()
    {
        yield return new WaitForSeconds(0.1f);
        Vector3[] array = new Vector3[2];
        a1RayLine.SetPositions(array);
    }

    public override void Hab1()
    {
        base.Hab1();
        if (currentHab1Cd <= 0 && !IsCasting())
        {
            Vector2 dist = UtilsClass.GetMouseWorldPosition() - transform.position;
            if (dist.magnitude > h1TurretRange)
            {
                if (Physics2D.Raycast(transform.position, dist, h1TurretRange, GameManager.Instance.wallLayer))
                {
                    RaycastHit2D ray2D = Physics2D.Raycast(transform.position, dist, h1TurretRange, GameManager.Instance.wallLayer);
                    dist = ray2D.point;
                }
                else if (dist.magnitude > h1TurretRange)
                {
                    float range = dist.magnitude - h1TurretRange;
                    dist = UtilsClass.GetMouseWorldPosition();
                    dist = new Vector2(dist.x - range, dist.y - range);
                }
                else
                {
                    dist = UtilsClass.GetMouseWorldPosition();
                }
            }
            else 
            {
                if (Physics2D.Raycast(transform.position, dist, dist.magnitude, GameManager.Instance.wallLayer))
                {
                    RaycastHit2D ray2D = Physics2D.Raycast(transform.position, dist, h1TurretRange, GameManager.Instance.wallLayer);
                    dist = ray2D.point;
                }
                else if (dist.magnitude > h1TurretRange)
                {
                    float range = dist.magnitude - h1TurretRange;
                    dist = UtilsClass.GetMouseWorldPosition();
                    dist = new Vector2(dist.x - range, dist.y - range);
                }
                else
                {
                    dist = UtilsClass.GetMouseWorldPosition();
                }
            }

            if (h1Count < 3)
            {
                currentHab1Cd = CDR(hab1Cd);
            }
            else
            {
                currentHab1Cd = CDR(hab1Cd*0.5f);
            }

            if (h1Count < h1MaxCount)
            {
                CuckooTurret cuckoo = Instantiate(h1Cuckoo, transform.position, transform.rotation).GetComponent<CuckooTurret>();
                cuckoo.SetUp(this, dist, h1BulletCount, h1Range, h1Spd);
                h1CuckooList.Add(cuckoo);
                h1Count++;
                if (CharacterManager.Instance.data[4].convergence >= 3)
                {
                    switch (h1Count)
                    {
                        case 1:
                            c3Light1.SetActive(true);
                            break;

                        case 2:
                            c3Light2.SetActive(true);
                            break;

                        case 3:
                            c3Light3.SetActive(true);
                            break;

                    }

                    foreach (NoelPot pot in c3ActualPot)
                    {
                        pot.BuffUpdate(CalculateControl(c3Pot));
                    }
                }
            }
            else
            {
                h1CuckooList[0].position = dist;
                h1CuckooList.Add(h1CuckooList[0]);
                h1CuckooList.RemoveAt(0);
            }   

            
        }
    }

    public override void Hab2()
    {
        base.Hab2();
        if (!IsCasting() && currentHab2Cd <= 0)
        {
            StartCoroutine(MineDisplace());
            currentHab2Cd = CDR( hab2Cd);
        }
    }

    IEnumerator MineDisplace()
    {
        GetComponent<Collider2D>().enabled = false;

        yield return StartCoroutine(Cast(h2CastTime / 3));

        Instantiate(h2Display1, transform.position, h2Display1.transform.rotation);
        yield return StartCoroutine(Cast(h2CastTime / 3));

        Instantiate(h2Display2, transform.position, h2Display2.transform.rotation);
        yield return StartCoroutine(Cast(h2CastTime / 3));

        Instantiate(h2Display3, transform.position, h2Display3.transform.rotation);

        if (CharacterManager.Instance.data[4].convergence >= 7)
        {
            foreach(CuckooTurret turret in h1CuckooList)
            {
                Instantiate(h2Display1, turret.transform.position, h2Display1.transform.rotation);
            }
        }

        GetComponent<Collider2D>().enabled = true;
        
    }

     

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, a1Range);
        Vector2 point = new Vector2(transform.position.x, transform.position.y + a1Range/2);
        Gizmos.DrawWireCube(point, new Vector3(a1Weight, a1Range, 1));
    }
}
