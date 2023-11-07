using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Mine : Spell
{
    new Noel user;
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
    }

    private void Start()
    {
        SetUp();
    }
    public void SetUp()
    {
        lightGO.SetActive(false);
        user = FindObjectOfType<Noel>();
        dmg = user.h2Dmg;
        delay = user.h2Delay; 
        currentDelay = delay;
        stunnTime = user.h2StunnDuration;
        time = user.h2Duration;
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
            enemyColl.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user.CalculateSinergy(dmg), HitData.Element.lightning);
            user.DamageDealed(user, enemyColl.GetComponent<Enemy>(), HitData.Element.lightning, HitData.AttackType.range, HitData.HabType.hability);
            enemyColl.GetComponent<Enemy>().stunTime += stunnTime;
        }

        if (CharacterManager.Instance.data[3].convergence >= 7)
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
