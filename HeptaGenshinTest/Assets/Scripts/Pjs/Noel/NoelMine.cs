using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoelMine : Spell
{
    Noel noel;
    public bool showGizmos;
    public ParticleSystem explosion;
    public GameObject particle;
    public GameObject lightGO;
    public float area;
    float dmg;
    float stunnTime;
    float delay;
    float currentDelay;
    bool active;

    public override void Update()
    {
        base.Update();
        if (!active) 
        {
            currentDelay -= Time.deltaTime;
            if (currentDelay <= 0)
            {
                active = true;
                lightGO.SetActive(true);
            }
        }

        if(noel.stats.hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetUp();
    }
    public void SetUp()
    {
        lightGO.SetActive(false);
        noel = FindObjectOfType<Noel>();
        user = noel;
        dmg = noel.h2Dmg;
        delay = noel.h2Delay; 
        currentDelay = delay;
        stunnTime = noel.h2StunnDuration;
        time = noel.h2Duration;
        particle = explosion.gameObject;
        if(transform.parent != null)
        {
            transform.eulerAngles = new Vector3(0, 0, -transform.parent.rotation.z);
        }

    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Enemy") || collision.CompareTag("Wall")) && active)
        {
            Explode();

        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.CompareTag("Enemy") || collision.CompareTag("Wall")) && active)
        {
            Explode();

        }
    }

    void Explode()
    {
        active = false;
        explosion.Play();
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, area, GameManager.Instance.enemyLayer);
        foreach (Collider2D enemyColl in enemiesHit)
        {
            enemyColl.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user, noel.CalculateSinergy(dmg), HitData.Element.lightning);
            noel.DamageDealed(noel, enemyColl.GetComponent<Enemy>(), noel.CalculateSinergy(dmg), HitData.Element.lightning, HitData.AttackType.range, HitData.HabType.hability);
            noel.Stunn(enemyColl.GetComponent<Enemy>(), stunnTime);
        }

        if (CharacterManager.Instance.data[4].convergence >= 7)
        {
            currentDelay = delay;
            active = false;
            lightGO.SetActive(false);
        }
        else
        {
            Die();
        }

    }


    public override void Die()
    {
        if (particle != null)
        {
            particle.transform.parent = null;
        }
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.DrawWireSphere(transform.position, area);
        }
    }
}
