using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Enemy : MonoBehaviour, TakeDamage
{
    public Stats stats;

    public float stunTime;
    public PjBase target;
    public float viewDist;
    public GameObject pointer;
    public float damageTextOffset;
    // Start is called before the first frame update

    private void Awake()
    {
        stats.hp = stats.mHp;
    }
    void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        stunTime -= Time.deltaTime;

        if (target == null)
        {
            foreach (PjBase pj in GameManager.Instance.pjList)
            {
                if (pj != null)
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
            if (dist.magnitude > viewDist || Physics2D.Raycast(transform.position, dist, dist.magnitude, GameManager.Instance.wallLayer))
            {
                
                target = null;
               
            }
            else
            {
                pointer.transform.up = dist;
            }
        }
    }

    void TakeDamage.TakeDamage(float value, HitData.Element element)
    {
        float calculo = 0;
        DamageText dText = null;
        switch (element)
        {
            case HitData.Element.ice:
                calculo = stats.iceResist; 
                dText = Instantiate(GameManager.Instance.damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(damageTextOffset - 0.5f, damageTextOffset+0.5f), 0), transform.rotation).GetComponent<DamageText>();
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

        dText.damageText.text = value.ToString("F0");

        stats.hp -= value;
        if (stats.hp <= 0)
        {
            GetComponent<TakeDamage>().Die();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, viewDist);
    }

    void TakeDamage.Stunn(float stunTime)
    {
        if (stunTime > this.stunTime)
        {
            this.stunTime = stunTime;
        }
    }

    void TakeDamage.Die()
    {
        Destroy(gameObject);
    }
}
