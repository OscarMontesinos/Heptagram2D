using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Enemy : PjBase
{

    public float stunTime;
    public PjBase target;
    public float viewDist;
    public GameObject pointer;
    // Start is called before the first frame update

    public override void Awake()
    {
        isActive = true;
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
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        stunTime -= Time.deltaTime;

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
            else
            {
                pointer.transform.up = dist;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, viewDist);
    }

}
