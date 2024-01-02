using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Enemy : PjBase
{
    public Rigidbody2D rb;
    public PjBase target;
    public float viewDist;
    public GameObject pointer;
    public bool point = true;
    // Start is called before the first frame update

    public override void Awake()
    {
        isActive = true;
        rb = GetComponent<Rigidbody2D>();
            manager = GameManager.Instance;
    }
    public override void Start()
    {
        stats.mHp += statsPerLevel.mHp * (level - 1);
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


        if (hpBar != null)
        {
            hpBar.maxValue = stats.mHp;
            hpBar.value = stats.hp;
                hpText.text = stats.hp.ToString("F0");
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

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
            if (dist.magnitude > viewDist || Physics2D.Raycast(transform.position, dist, dist.magnitude, GameManager.Instance.wallLayer))
            {

                target = null;

            }
            else if (point)
            {
                pointer.transform.up = dist;
            }
        }
    }
    public virtual void AI()
    {

    }
    public virtual IEnumerator RestartAi()
    {
        yield return null;
        AI();
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, viewDist);
    }


    public override IEnumerator Dash(Vector2 direction, float speed, float range, bool isBasicDash)
    {
        GetComponent<Collider2D>().isTrigger = true;
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
                rb.velocity = distance.normalized * speed;
            }
            else
            {
                rb.velocity = distance * speed;
            }
            distance = destinyPoint - new Vector2(transform.position.x, transform.position.y);
            yield return null;
        }
        dashing = false;
        GetComponent<Collider2D>().isTrigger = false;
        rb.velocity = new Vector2(0, 0);
        if (isBasicDash)
        {
            EndedBasicDash();
        }

    }

    public void PointerLock(int value)
    {
        if (value == 1)
        {
            point = true;
        }
        else
        {
            point = false;
        }
    }

    
}
