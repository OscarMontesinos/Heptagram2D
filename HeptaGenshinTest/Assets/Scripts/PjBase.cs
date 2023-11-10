using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PjBase : MonoBehaviour, TakeDamage
{
    public PlayerController controller;
    public GameObject sprite;
    public string chName;
    public HitData.Element element;
    public GameObject spinObjects;
    public Slider hpBar;
    public Slider stunnBar;
    public Sprite hab1Image;
    public Sprite hab2Image;
    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public float currentComboReset;
    public float strongAtSpdMultiplier;
    [HideInInspector]
    public float currentHab1Cd;
    public float hab1Cd;
    [HideInInspector]
    public float currentHab2Cd;
    public float hab2Cd;
    public float bDashSpeed;
    public float bDashRange;
    [HideInInspector]
    public bool casting;
    [HideInInspector]
    public bool softCasting;
    [HideInInspector]
    public bool dashing;
    [HideInInspector]
    public float stunTime;
    public int level;
    public Stats stats;
    public Stats statsPerLevel;
    public float damageTextOffset;
    public virtual void Awake()
    {
        controller = PlayerController.Instance;
    }
    public virtual void Start()
    {
        level = PlayerPrefs.GetInt(chName + "Level");
        stats.mHp += statsPerLevel.mHp * (level-1);
        stats.sinergy += statsPerLevel.sinergy * (level - 1);
        stats.control += statsPerLevel.control * (level - 1);
        stats.iceResist += statsPerLevel.iceResist * (level - 1);
        stats.fireResist += statsPerLevel.fireResist * (level - 1);
        stats.waterResist += statsPerLevel.waterResist * (level - 1);
        stats.desertResist += statsPerLevel.desertResist * (level - 1);
        stats.windResist += statsPerLevel.windResist * (level - 1);
        stats.natureResist += statsPerLevel.natureResist * (level - 1);
        stats.lightningResist += statsPerLevel.lightningResist * (level - 1);
        stats.hp = stats.mHp;


        GameManager.Instance.pjList.Add(this);
    }
    public virtual void Update()
    {
        if (isActive)
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
        }

        if (currentComboReset > 0)
        {
            currentComboReset -= Time.deltaTime;
        }
        if (currentHab1Cd > 0)
        {
            currentHab1Cd -= Time.deltaTime;
        }
        if (currentHab2Cd > 0)
        {
            currentHab2Cd -= Time.deltaTime;
        }
        if(spinObjects != null)
        {
            spinObjects.transform.rotation = controller.pointer.transform.rotation;
        }
    }

    public virtual void Activate(bool active)
    {
        GetComponent<Collider2D>().enabled = active;
        isActive = active;
        sprite.SetActive(active);
    }
    public virtual void MainAttack()
    {
    }

    public virtual void StrongAttack()
    {

    }

    public virtual void Hab1()
    {

    }

    public virtual void Hab2()
    {

    }

    public void BasicDash()
    {
        if (!casting && !softCasting)
        {
            StartCoroutine(Dash(controller.inputMov, bDashSpeed, bDashRange, true));
            StartCoroutine(Cast(0.5f));
            UsedBasicDash();
        }
    }
    public virtual void UsedBasicDash()
    { 
    
    }
    public virtual void EndedBasicDash()
    { 
    
    }

    public IEnumerator Cast(float time)
    {
        casting = true;
        yield return new WaitForSeconds(time);
        casting = false;
    }
    public IEnumerator SoftCast(float time)
    {
        softCasting = true;
        yield return new WaitForSeconds(time);
        softCasting = false;
    }

    public bool IsCasting()
    {
        if(!casting && !softCasting)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public virtual void DamageDealed(PjBase user, PjBase target, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {
        foreach (PjBase pj in controller.team)
        {
            pj.Interact(this, target, element, attackType, habType);
        }

        List<HitInteract> hitList = new List<HitInteract>( target.gameObject.GetComponents<HitInteract>());
        foreach (HitInteract hit in hitList)
        {
            hit.Interact(user,target,element,attackType,habType);
        }
        
    }

    void TakeDamage.TakeDamage(float value, HitData.Element element)
    {
        TakeDmg(value, element);
    }

    public virtual void TakeDmg(float value, HitData.Element element)
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
            float originalValue = value;
            if (controller != null)
            {
                while (Shield.shieldAmount > 0 && value > 0)
                {
                    Shield chosenShield = null;
                    foreach (Shield shield in controller.GetComponents<Shield>())
                    {
                        if (chosenShield == null || shield.time < chosenShield.time && shield.singularShieldAmount > 0)
                        {
                            chosenShield = shield;
                        }
                    }
                    value = chosenShield.ChangeShieldAmount(-value);

                }

                /*if(value != originalValue)
                {
                    originalValue -= value;
                    DamageText sText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    sText.textColor = Color.white;
                    sText.damageText.text = originalValue.ToString("F0");
                }*/
            }


            dText.damageText.text = value.ToString("F0");

            stats.hp -= value;
            if (stats.hp <= 0)
            {
                GetComponent<TakeDamage>().Die();
            }
            else if (!CompareTag("Enemy"))
            {
                OnDamageTaken();
                foreach (PjBase pj in controller.team)
                {
                    pj.OnGlobalDamageTaken();
                }
            }
            if (hpBar != null)
            {
                hpBar.maxValue = stats.mHp;
                hpBar.value = stats.hp;
            }
        }
    }

    public void Heal(float value, HitData.Element element)
    {
        if (stats.hp > 0)
        {
            stats.hp += value;
            if (stats.hp > stats.mHp)
            {
                stats.hp = stats.mHp;
            }

            DamageText dText = null;
            switch (element)
            {
                case HitData.Element.ice:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.iceColor;
                    break;
                case HitData.Element.fire:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.fireColor;
                    break;
                case HitData.Element.water:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.waterColor;
                    break;
                case HitData.Element.desert:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.desertColor;
                    break;
                case HitData.Element.wind:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.windColor;
                    break;
                case HitData.Element.nature:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.natureColor;
                    break;
                case HitData.Element.lightning:
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.lightningColor;
                    break;
            }

            dText.damageText.text = "+" + value.ToString("F0");
        }

    }

    public virtual void OnGlobalDamageTaken()
    {

    }
    public virtual void OnDamageTaken()
    {

    }
    void TakeDamage.Stunn(float stunTime)
    {
        this.stunTime = stunTime;
    }
    void TakeDamage.Die()
    {
        if (PlayerController.Instance.team.Contains(this))
        {
            stats.hp = 0;
            GameManager.Instance.DisplayAliveCharacter();
        }
        else
        {
            foreach(PjBase pj in PlayerController.Instance.team)
            {
                pj.OnKill(this.GetComponent<Enemy>());
            }
            Destroy(gameObject);
        }
    }

    public virtual float CalculateSinergy(float calculo)
    {
        float value = stats.sinergy;
        value *= calculo / 100;
        //valor.text = value.ToString();
        return value;

    }

    public virtual float CalculateControl(float calculo)
    {
        float value = stats.control;
        value *= calculo / 100;
        //valor.text = value.ToString();
        return value;
    }
    public float CDR(float value)
    {
        value -= ((value * ((stats.cdr / (100 + stats.cdr)))));
        return value;
    }
    public float CalculateAtSpd(float value)
    {
        value = 1 / value;
        return value;
    }

    public virtual IEnumerator Dash(Vector2 direction, float speed, float range, bool isBasicDash)
    {
        GetComponent<Collider2D>().enabled = false;
        dashing = true;
        Vector2 destinyPoint = Physics2D.Raycast(transform.position, direction, range, GameManager.Instance.wallLayer).point;
        yield return null;
        if (destinyPoint == new Vector2(0, 0))
        {
            destinyPoint = new Vector2(transform.position.x, transform.position.y) + direction.normalized * range;
        }
        Vector2 distance = destinyPoint - new Vector2(transform.position.x, transform.position.y);
        yield return null;
        while (distance.magnitude > 1 && dashing && stunTime <=0)
        {
            if (distance.magnitude > 0.7)
            {
                controller.rb.velocity = distance.normalized * speed;
            }
            else
            {
                controller.rb.velocity = distance * speed;
            }
            distance = destinyPoint - new Vector2(transform.position.x, transform.position.y);
            yield return null;
        }
        dashing = false;
        GetComponent<Collider2D>().enabled = true;
        controller.rb.velocity = new Vector2(0, 0);
        if (isBasicDash)
        {
            EndedBasicDash();
        }
    }

    public virtual IEnumerator Dash(Vector2 direction, float speed, float range, bool isBasicDash, bool ignoreWalls)
    {
        GetComponent<Collider2D>().enabled = false;
        dashing = true;

        Vector2 destinyPoint = new Vector2(transform.position.x, transform.position.y) + direction.normalized * range;

        Vector2 distance = destinyPoint - new Vector2(transform.position.x, transform.position.y);
        yield return null;
        while (distance.magnitude > 1 && dashing && stunTime <= 0)
        {
            if (distance.magnitude > 0.7)
            {
                controller.rb.velocity = distance.normalized * speed;
            }
            else
            {
                controller.rb.velocity = distance * speed;
            }
            distance = destinyPoint - new Vector2(transform.position.x, transform.position.y);
            yield return null;
        }
        dashing = false;
        GetComponent<Collider2D>().enabled = true;
        controller.rb.velocity = new Vector2(0, 0);
        if (isBasicDash)
        {
            EndedBasicDash();
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

    public virtual void OnKill(Enemy enemy)
    {

    }

    public virtual void Interact(PjBase user, Enemy target, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {

    }
    public virtual void Interact(PjBase user, PjBase target, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {

    }
}
