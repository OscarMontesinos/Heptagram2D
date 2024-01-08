using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PjBase : MonoBehaviour, TakeDamage
{
    public GameManager manager;
    public PlayerController controller;
    public GameObject sprite;
    public string chName;
    public int id;
    public HitData.Element element;
    public GameObject spinObjects;
    public Slider hpBar;
    public TextMeshProUGUI hpText;
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
    public float dashCd = 0.5f;
    [HideInInspector]
    public float currentDashCd;
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
    [HideInInspector]
    public bool ignoreSoftCastDebuff;
    public int level;
    public Stats stats;
    public Stats statsPerLevel;
    public float damageTextOffset;
    [HideInInspector]
    public float dmgDealed;

    float healCount;
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
        bDashSpeed -= stats.spd;

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

        if (currentDashCd > 0)
        {
            currentDashCd -= Time.deltaTime;
        }

        if (spinObjects != null)
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
        if (stunTime <= 0)
        {
            return;
        }
    }

    public virtual void StrongAttack()
    {
        if (stunTime <= 0)
        {
            return;
        }
    }

    public virtual void Hab1()
    {
        if (stunTime <= 0)
        {
            return;
        }
    }

    public virtual void Hab2()
    {
        if(stunTime <= 0)
        {
            return;
        }
    }


    public virtual void BasicDash()
    {
        if (!casting && !dashing && currentDashCd <= 0)
        {   
            StartCoroutine(Dash(controller.inputMov, bDashSpeed, bDashRange, true));
            UsedBasicDash();
            currentDashCd = dashCd;
        }
    }

    public virtual void UsedBasicDash()
    { 
    
    }
    public virtual void EndedBasicDash()
    { 
    
    }
    public virtual void UsedBasicDashGlobal()
    { 
    
    }
    public virtual void EndedBasicDashGlobal()
    { 
    
    }

    public IEnumerator Cast(float time)
    {
        casting = true;
        yield return new WaitForSeconds(time);
        casting = false;
    }
    public virtual IEnumerator SoftCast(float time)
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

    public void RegisterDamage(float dmg)
    {
        if (controller != null)
        {
            dmgDealed += dmg;
            UIManager.Instance.UpdateDamageText();
        }
    }

    public virtual void DamageDealed(PjBase user, PjBase target, float amount, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {
        foreach (PjBase pj in controller.team)
        {
            pj.Interact(this, target, amount, element, attackType, habType);
        }

        List<HitInteract> hitList = new List<HitInteract>( target.gameObject.GetComponents<HitInteract>());
        foreach (HitInteract hit in hitList)
        {
            hit.Interact(user,target,amount,element,attackType,habType);
        }
        
    }

    void TakeDamage.TakeDamage(PjBase user,float value, HitData.Element element)
    {
        TakeDmg(user, value, element);
    }

    public virtual void TakeDmg(PjBase user,float value, HitData.Element element)
    {
        if (isActive)
        {
            float calculo = 0;
            DamageText dText = null;
            switch (element)
            {
                case HitData.Element.ice:
                    calculo = stats.iceResist + stats.resist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.iceColor;
                    break;
                case HitData.Element.fire:
                    calculo = stats.fireResist + stats.resist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.fireColor;
                    break;
                case HitData.Element.water:
                    calculo = stats.waterResist + stats.resist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.waterColor;
                    break;
                case HitData.Element.blood:
                    calculo = stats.waterResist + stats.resist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.bloodColor;
                    break;
                case HitData.Element.desert:
                    calculo = stats.desertResist + stats.resist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.desertColor;
                    break;
                case HitData.Element.wind:
                    calculo = stats.windResist + stats.resist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.windColor;
                    break;
                case HitData.Element.nature:
                    calculo = stats.natureResist + stats.resist;
                    dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                    dText.textColor = GameManager.Instance.natureColor;
                    break;
                case HitData.Element.lightning:
                    calculo = stats.lightningResist + stats.resist;
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
            user.RegisterDamage(value);
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
                hpText.text = stats.hp.ToString("F0");
            }
        }
    }

    public virtual void Heal(PjBase user, float value, HitData.Element element)
    {
        if (stats.hp > 0)
        {
            stats.hp += value;
            if (stats.hp > stats.mHp)
            {
                stats.hp = stats.mHp;
            }

            if (value + healCount > 1)
            {
                value += healCount;
                healCount = 0;
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
                    case HitData.Element.blood:
                        dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset + 0.5f), 0), transform.rotation).GetComponent<DamageText>();
                        dText.textColor = GameManager.Instance.bloodColor;
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
            else
            {
                healCount += value;
            }
        }

    }

    public virtual void Stunn(PjBase target, float value)
    {
        target.GetComponent<TakeDamage>().Stunn(value);

        if (controller != null)
        {
            foreach (PjBase pj in controller.team)
            {
                pj.OnGlobalStunn(target, value);
            }
        }
    }

    public virtual void OnGlobalStunn(PjBase target, float value)
    {

    }

    public virtual void OnGlobalDamageTaken()
    {

    }
    public virtual void OnDamageTaken()
    {

    }

    public virtual void Moving(float magnitude)
    {

    }
    void TakeDamage.Stunn(float stunTime)
    {
        this.stunTime += stunTime;
    }
    void TakeDamage.Die()
    {
        if (manager.pjList.Contains(this))
        {
            stats.hp = 0;
            if (GameManager.Instance.gamemode == GameManager.GameModes.singleplayer)
            {
                GameManager.Instance.DisplayAliveCharacter();
            }
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
        yield return null;
        StartCoroutine(Dash(direction, speed, range, isBasicDash, false, false));
    }
    public virtual IEnumerator Dash(Vector2 direction, float speed, float range, bool isBasicDash, bool ignoreWalls, bool shutDownCollider)
    {
        if (isBasicDash)
        {
            speed += stats.spd;
        }
        if (!shutDownCollider)
        {
            GetComponent<Collider2D>().isTrigger = true;
        }
        else
        {
            GetComponent<Collider2D>().enabled = false;
        }
        dashing = true;
        Vector2 destinyPoint = Physics2D.Raycast(transform.position, direction, range, GameManager.Instance.wallLayer).point;
        yield return null;
        if (destinyPoint == new Vector2(0, 0))
        {
            destinyPoint = new Vector2(transform.position.x, transform.position.y) + direction.normalized * range;
        }
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
        if (!shutDownCollider)
        {
            GetComponent<Collider2D>().isTrigger = false;
        }
        if (isBasicDash)
        {
            EndedBasicDash();
            foreach (PjBase pj in controller.team)
            {
                pj.EndedBasicDashGlobal();
            }
        }
    }

    public virtual IEnumerator Dash(Vector2 direction, float speed, float range, bool isBasicDash, bool ignoreWalls)
    {
        if (isBasicDash)
        {
            speed += stats.spd;
        }
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
            foreach (PjBase pj in controller.team)
            {
                pj.EndedBasicDashGlobal();
            }
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

    public virtual void Interact(PjBase user, PjBase target, float amount, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {

    }
}
