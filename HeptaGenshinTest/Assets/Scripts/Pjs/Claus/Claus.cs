using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using ExitGames.Client.Photon;
using CodeMonkey.Utils;
using UnityEngine.UI;
using System;
using UnityEngine.Assertions.Must;
using UnityEngine.SocialPlatforms;
using UnityEngine.Rendering.Universal;
using static UnityEngine.GraphicsBuffer;

public class Claus : PjBase
{
    public Slider manaBar;
    public float maxMana;
    [HideInInspector]
    public float mana;
    public float manaGainedMultiplier;
    public float manaPerHit;
    List<ClausSpdBuff> spdBuffs = new List<ClausSpdBuff>();
    public float extraSpdPerOneMana;
    public GameObject snowBall;

    public GameObject a1Point1;
    public GameObject a1Point2;
    public float a1Spd;
    public float a1Range;
    public float a1Dmg;
    public int combo;

    bool a2Dashing;
    public float a2Spd;
    public float a2Range;

    public GameObject h1Enchant;
    public float h1RealCd;
    float h1CurrentRealCd;
    public float h1Cost;
    int h1HeadingQuantity = 1;
    int h1SelectedQuantity;
    float h1SelectedCost;
    public float h1Range;
    public float h1Heal;
    public float h1Delay;
    public float h1PulsesCd;

    [HideInInspector]
    public bool h2Active;
    public float h2Prot;
    public float h2Duration;
    [HideInInspector]
    public float h2CurrentDuration;
    public GameObject h2Particles;
    public List<GameObject> currentParticles = new List<GameObject>();

    public float c1AtSpdPerOneMana;

    public float c2Range;

    public float c3Range;

    public int c5ExtraPulses;

    public float c6ExtraMana;

    public float c7ShieldPerOneDmg;
    public override void Start()
    {
        base.Start();

        ignoreSoftCastDebuff = true;
        if (CharacterManager.Instance.data[id].convergence >= 6)
        {
            maxMana += c6ExtraMana;
        }
        manaBar.maxValue = maxMana;
        hab1Cd = maxMana / h1Cost;
        h1HeadingQuantity = (int)hab1Cd;

        StartCoroutine(PostStart());

    }

    IEnumerator PostStart()
    {
        yield return null;
        if (CharacterManager.Instance.data[id].convergence >= 3)
        {
            foreach (PjBase pj in GameManager.Instance.pjList)
            {
                ClausSpdBuff buff = pj.AddComponent<ClausSpdBuff>();
                if (pj == this)
                {
                    buff.SetUp(this, extraSpdPerOneMana);
                }
                else
                {
                    buff.SetUp(this, extraSpdPerOneMana / 2);
                }
                spdBuffs.Add(buff);
            }
        }
        else
        {
            ClausSpdBuff buff = gameObject.AddComponent<ClausSpdBuff>();
            buff.SetUp(this, extraSpdPerOneMana);
            spdBuffs.Add(buff);
        }
    }

    public override void Update()
    {
        base.Update();

        if (!dashing)
        {
            a2Dashing = false;
        }

        if (h1CurrentRealCd > 0)
        {
            h1CurrentRealCd -= Time.deltaTime;
        }

        if (h2Active)
        {
            currentHab2Cd = h2CurrentDuration;
        }
        else if (currentParticles[0] != null)
        {
            foreach (GameObject item in currentParticles)
            {
                Destroy(item);
            }
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            h1HeadingQuantity++;
            if (h1HeadingQuantity > maxMana / h1Cost)
            {
                h1HeadingQuantity--;
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            h1HeadingQuantity--;
            if (h1HeadingQuantity < 1)
            {
                h1HeadingQuantity++;
            }
        }

        h1SelectedQuantity = h1HeadingQuantity;
        if (mana - (h1SelectedQuantity * h1Cost) < 0)
        {
            h1SelectedQuantity = (int)(mana / h1Cost);
        }

        if (h1CurrentRealCd <= 0)
        {
            currentHab1Cd = h1SelectedQuantity;
            if (currentHab1Cd == 0)
            {
                currentHab1Cd = 0.2f;
            }
        }
        else
        {
            currentHab1Cd = h1CurrentRealCd;
        }

        h1SelectedCost = h1Cost * h1SelectedQuantity;



        Vector2 dir = UtilsClass.GetMouseWorldPosition() - a1Point1.transform.position;
        a1Point1.transform.up = dir;
        dir = UtilsClass.GetMouseWorldPosition() - a1Point2.transform.position;
        a1Point2.transform.up = dir;
        manaBar.value = mana;


    }


    public override void BasicDash()
    {
        if (a2Dashing)
        {
            StartCoroutine(DashCancel());
        }
        else
        {
            base.BasicDash();
        }
    }

    IEnumerator DashCancel()
    {
        dashing = false;
        yield return null;
        BasicDash();
    }

    public void AddMana(float value)
    {
        mana += value;
        if (mana < 0)
        {
            mana = 0;
        }
        else if (mana > maxMana)
        {
            mana = maxMana;
        }
    }

    public override void MainAttack()
    {
        base.MainAttack();

        if (!IsCasting())
        {
            if (CharacterManager.Instance.data[id].convergence >= 1)
            {
                StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd + (c1AtSpdPerOneMana * mana))));
            }
            else
            {
                StartCoroutine(SoftCast(CalculateAtSpd(stats.atSpd)));
            }
            Snowball bullet;
            if (combo == 0)
            {
                bullet = Instantiate(snowBall, a1Point1.transform.position, a1Point1.transform.rotation).GetComponent<Snowball>();
                combo++;
            }
            else
            {
                bullet = Instantiate(snowBall, a1Point2.transform.position, a1Point2.transform.rotation).GetComponent<Snowball>();
                combo = 0;
            }
            bullet.SetUp(this, a1Spd, a1Range, CalculateSinergy(a1Dmg));
        }
    }
    public override void StrongAttack()
    {
        base.StrongAttack();

        if (!dashing)
        {
            Vector3 destiny = UtilsClass.GetMouseWorldPosition();
            Vector3 dist = destiny - transform.position;

            a2Dashing = true;
            StartCoroutine(DawnSkating());
            StartCoroutine(Dash(dist, a2Spd + stats.spd, a2Range, false, false, false));

        }
    }

    IEnumerator DawnSkating()
    {
        List<PjBase> alliesHitted = new List<PjBase>();
        while (a2Dashing)
        {
            yield return null;
            Collider2D[] allyHit = Physics2D.OverlapCircleAll(transform.position, c2Range, GameManager.Instance.playerLayer);
            PjBase ally;
            foreach (Collider2D allyColl in allyHit)
            {
                ally = allyColl.GetComponent<PjBase>();
                if (!alliesHitted.Contains(ally))
                {
                    alliesHitted.Add(ally);
                    if (ally.isActive && ally != this)
                    {
                        mana -= h1Cost * 0.5f;
                        Enchant(ally.transform.position, 1, h1Delay);
                    }
                }
            }
        }
    }



    public override void Hab1()
    {
        base.Hab1();
        if (!IsCasting() && h1SelectedQuantity > 0 && h1CurrentRealCd <= 0)
        {
            Vector3 destiny = UtilsClass.GetMouseWorldPosition();
            Vector3 dist = destiny - transform.position;

            if (dist.magnitude > h1Range)
            {
                float range = dist.magnitude - h1Range;
                destiny -= dist.normalized * range;
            }
            else
            {
                destiny = UtilsClass.GetMouseWorldPosition();
            }

            Enchant(destiny, h1SelectedQuantity, h1Delay);

            h1CurrentRealCd = h1RealCd + h1Delay + (h1PulsesCd * h1SelectedQuantity);
            mana -= h1Cost * h1SelectedQuantity;


        }
    }

    void Enchant(Vector3 destiny, int quantity, float delay)
    {
        int extraCharge = 0;
        if (CharacterManager.Instance.data[id].convergence >= 5)
        {
            extraCharge += c5ExtraPulses;
        }
        ClausEnchant enchant = Instantiate(h1Enchant, destiny, transform.rotation).GetComponent<ClausEnchant>();
        enchant.SetUp(this, quantity + extraCharge, CalculateControl(h1Heal), delay, h1PulsesCd);
    }


    public override void Hab2()
    {
        if (CharacterManager.Instance.data[id].convergence >= 4)
        {
            stunTime = 0;
        }
        else
        {
            base.Hab2();
        }
        if (!IsCasting() && currentHab2Cd <= 0)
        {
            if (CharacterManager.Instance.data[id].convergence >= 3)
            {
                stunTime = 0;
            }
            h2Active = true;
            currentHab2Cd = CDR(hab2Cd);
            currentParticles = new List<GameObject>();
            foreach (PjBase pj in GameManager.Instance.pjList)
            {
                if (pj.isActive)
                {
                    currentParticles.Add(Instantiate(h2Particles, pj.transform));
                }

                ClausProtBuff buff = pj.AddComponent<ClausProtBuff>();
                buff.SetUp(this, h2Duration, CalculateControl(h2Prot));
            }
        }
    }

    public override void Moving(float magnitude)
    {
        if (!a2Dashing)
        {
            AddMana(magnitude * manaGainedMultiplier);
        }
        else
        {

            AddMana(magnitude * manaGainedMultiplier * 3);
        }
        base.Moving(magnitude);
    }

    public override void Interact(PjBase user, PjBase target, float amount, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {
        base.Interact(user, target, amount, element, attackType, habType);
        if (CharacterManager.Instance.data[id].convergence >= 7 && h2Active)
        {
            FindObjectOfType<ClausProtBuff>().AddDamage(amount);
        }

    }
}
