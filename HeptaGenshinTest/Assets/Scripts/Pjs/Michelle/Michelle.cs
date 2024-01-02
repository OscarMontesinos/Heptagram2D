using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting;
using UnityEngine;
using ExitGames.Client.Photon;

public class Michelle : PjBase
{
    public Color32 habilitiesColor;

    Animator _animator;
    int combo;
    public GameObject a1Point;
    public float a1Area;
    public float a1Dmg;

    public float a2Area;
    public float a2Dmg;

    public float h1PotMultiplier;
    public float h1MaxPool;
    public float h1PoolLosedPerSecond;
    public float h1PoolExchangedPerSecond;
    [HideInInspector]
    public float h1pool;

    bool h2Active;
    public float h2Duration;
    float h2CurrentDuration;
    public GameObject h2UIIndicator;
    GameObject h2CurrentUIIndicator;
    public GameObject h2ParticlePlace;
    public GameObject h2BloodParticle;
    public float h2SpinSpeed;
    public float h2CrimsonDmgPerBlood;
    public float h2HealBlood;

    public float c1ExtraBlood;

    public float c2MaxExtraDmg;

    public float c7MaxPool;
    public override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }

    public override void Start()
    {
        base.Start();
        if (CharacterManager.Instance.data[8].convergence >= 7)
        {
            h1MaxPool = c7MaxPool;
        }
        hab1Cd = h1MaxPool;
    }



    public override void Update()
    {
        base.Update();

        currentHab1Cd = h1pool;

        h2ParticlePlace.transform.localEulerAngles = new Vector3(h2ParticlePlace.transform.localEulerAngles.x, h2ParticlePlace.transform.localEulerAngles.y, h2ParticlePlace.transform.localEulerAngles.z + (h2SpinSpeed * Time.deltaTime));

        if (h2CurrentDuration > 0)
        {
            h2CurrentDuration -= Time.deltaTime;
        }
        else if (h2Active)
        {
            h2Active = false;
            Destroy(h2CurrentUIIndicator);
            if (CharacterManager.Instance.data[8].convergence < 4)
            {
                foreach (Transform blood in h2ParticlePlace.transform)
                {
                    Destroy(blood.gameObject);
                }
            }
        }

        if (h1pool > 0)
        {
            if (CharacterManager.Instance.data[8].convergence >= 7)
            {
                if ((h1pool / h1MaxPool) * 100 > 40)
                {
                    h1pool -= h1PoolLosedPerSecond * Time.deltaTime;
                    if (CharacterManager.Instance.data[8].convergence >= 5 && stats.hp < stats.mHp)
                    {
                        Heal(this, h1PoolLosedPerSecond * Time.deltaTime, HitData.Element.blood);
                    }
                }
            }
            else
            {
                h1pool -= h1PoolLosedPerSecond * Time.deltaTime;
                if (CharacterManager.Instance.data[8].convergence >= 5 && stats.hp < stats.mHp)
                {
                    Heal(this, h1PoolLosedPerSecond * Time.deltaTime, HitData.Element.blood);
                }
            }
        }
    }

    public override float CalculateSinergy(float calculo)
    {
        float value = stats.sinergy + (h1pool * h1PotMultiplier);
        value *= calculo / 100;
        //valor.text = value.ToString();
        return value;
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
            if (h2Active)
            {
                if (CharacterManager.Instance.data[8].convergence >= 5)
                {
                    _animator.Play("StrongAttack2");
                }
                else
                {
                    _animator.Play("StrongAttack");
                }
            }
            else
            {
                if (combo == 0)
                {
                    _animator.Play("Attack1");
                    combo++;
                    currentComboReset = CalculateAtSpd(stats.atSpd) + 0.5f;
                }
                else
                {

                    _animator.Play("Attack2");
                    combo = 0;
                }
            }
        }
        base.MainAttack();
    }

    public override void StrongAttack()
    {
        if (!IsCasting())
        {
            StartCoroutine(Cast(CalculateAtSpd(stats.atSpd / strongAtSpdMultiplier)));
            if (h2Active)
            {
                GetComponent<Collider2D>().enabled = false;
                _animator.Play("CrimsonConfetti");
            }
            else
            {
                if (CharacterManager.Instance.data[8].convergence >= 5)
                {
                    _animator.Play("StrongAttack2");
                }
                else
                {
                    _animator.Play("StrongAttack");
                }
            }
            base.StrongAttack();
        }
    }

    public void MichelleAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(a1Point.transform.position, a1Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            float extraDmg = 0;
            if (CharacterManager.Instance.data[8].convergence >= 2)
            {
                extraDmg = c2MaxExtraDmg - (c2MaxExtraDmg * (enemy.stats.hp / enemy.stats.mHp));
            }
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a1Dmg + extraDmg), HitData.Element.blood);
            DamageDealed(this, enemy, HitData.Element.water, HitData.AttackType.melee, HitData.HabType.basic);
            if (CharacterManager.Instance.data[2].convergence >= 1)
            {
                currentHab1Cd -= 1;
            }
        }
    }

    public void MichelleStrongAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, a2Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            float extraDmg = 0;
            if (CharacterManager.Instance.data[8].convergence >= 2)
            {
                extraDmg = c2MaxExtraDmg - (c2MaxExtraDmg * (enemy.stats.hp / enemy.stats.mHp));
            }
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(a2Dmg + extraDmg), HitData.Element.blood);
            DamageDealed(this, enemy, HitData.Element.water, HitData.AttackType.melee, HitData.HabType.basic);

            if (h2Active)
            {
                GameObject bloodParticle = Instantiate(h2BloodParticle, enemy.transform.position, transform.rotation);
                bloodParticle.transform.parent = h2ParticlePlace.transform;

            }
        }
    }

    public void CrimsonConfetti()
    {
        int count = 0;

        foreach (Transform blood in h2ParticlePlace.transform)
        {
            count++;
            Destroy(blood.gameObject);
        }

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, a2Area, GameManager.Instance.enemyLayer);
        Enemy enemy;
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemy = enemyColl.GetComponent<Enemy>();
            enemy.GetComponent<TakeDamage>().TakeDamage(this, CalculateSinergy(h2CrimsonDmgPerBlood * count), HitData.Element.blood);
            DamageDealed(this, enemy, HitData.Element.water, HitData.AttackType.melee, HitData.HabType.basic);

        }

        GetComponent<Collider2D>().enabled = true;
    }

    public override void Hab1()
    {
        if (!IsCasting())
        {
            StartCoroutine(PoolHeal());
        }
        base.Hab1();
    }

    IEnumerator PoolHeal()
    {
        casting = true;
        while (Input.GetKey(KeyCode.E) && (stats.hp < stats.mHp || (CharacterManager.Instance.data[8].convergence >= 6 && stats.hp >= stats.mHp)))
        {
            yield return null;
            if (h1pool > 0)
            {
                if (stats.hp < stats.mHp)
                {
                    h1pool -= h1PoolExchangedPerSecond * Time.deltaTime;
                    Heal(this, h1PoolExchangedPerSecond * Time.deltaTime, HitData.Element.blood);
                }
                else
                {
                    h1pool -= h1PoolExchangedPerSecond * Time.deltaTime;
                    currentHab2Cd -= h1PoolExchangedPerSecond * Time.deltaTime;
                }
            }
        }
        casting = false;
    }

    public void UpdatePool(float value)
    {
        h1pool += value;

        DamageText dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
        dText.textColor = GameManager.Instance.bloodColor;

        dText.damageText.text = "+" + value.ToString("F0");

        if (h1pool < 0)
        {
            h1pool = 0;
        }
        else if (h1pool > h1MaxPool)
        {
            h1pool = h1MaxPool;
        }

    }

    public override void Hab2()
    {
        if (!IsCasting() && currentHab2Cd <= 0)
        {
            h2CurrentDuration = h2Duration;
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(Cast(1.5f));
            StartCoroutine(RedCarnival());
        }
    }

    IEnumerator RedCarnival()
    {
        h2CurrentUIIndicator = Instantiate(h2UIIndicator, UIManager.Instance.transform);
        yield return new WaitForSeconds(1.5f);
        h2Active = true;
        currentHab2Cd = hab2Cd;
        if (CharacterManager.Instance.data[8].convergence >= 1)
        {
            UpdatePool(c1ExtraBlood);
        }
        GetComponent<Collider2D>().enabled = true;
    }

    public override void Heal(PjBase user, float value, HitData.Element element)
    {
        if (user == this)
        {
            base.Heal(user, value, element);
        }
        else
        {
            UpdatePool(value);
        }
    }

    public override void TakeDmg(PjBase user, float value, HitData.Element element)
    {
        if (isActive)
        {
            float calculo = 0;
            DamageText dText = null;
            switch (element)
            {
                case HitData.Element.ice:
                    calculo = stats.iceResist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.iceColor;
                    break;
                case HitData.Element.fire:
                    calculo = stats.fireResist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.fireColor;
                    break;
                case HitData.Element.water:
                    calculo = stats.waterResist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.waterColor;
                    break;
                case HitData.Element.blood:
                    calculo = stats.waterResist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.bloodColor;
                    break;
                case HitData.Element.desert:
                    calculo = stats.desertResist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.desertColor;
                    break;
                case HitData.Element.wind:
                    calculo = stats.windResist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.windColor;
                    break;
                case HitData.Element.nature:
                    calculo = stats.natureResist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.natureColor;
                    break;
                case HitData.Element.lightning:
                    calculo = stats.lightningResist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.lightningColor;
                    break;
            }

            if (calculo < 0)
            {
                calculo = 0;
            }
            value -= ((value * ((calculo / (100 + calculo) * 100))) / 100);

            dText.damageText.text = value.ToString("F0");

            stats.hp -= value;
            user.RegisterDamage(value);
            if (stats.hp <= 0)
            {
                GetComponent<TakeDamage>().Die();
            }
             
            if (hpBar != null)
            {
                hpBar.maxValue = stats.mHp;
                hpBar.value = stats.hp;
                hpText.text = stats.hp.ToString("F0");
            }


            float missingHp = stats.mHp - stats.hp;

            foreach (Transform blood in h2ParticlePlace.transform)
            {
                missingHp -= CalculateControl(h2HealBlood);
                blood.gameObject.GetComponent<BloodParticle>().Activate();

                if (missingHp <= 0)
                {
                    break;
                }
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(a1Point.transform.position, a1Area);
        Gizmos.DrawWireSphere(transform.position, a2Area);

    }
}
