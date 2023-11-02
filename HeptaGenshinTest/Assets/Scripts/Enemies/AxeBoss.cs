using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AxeBoss : Projectile
{
    PjBase user;
    public GameObject sprites;
    float dmg; 
    float stunnTime;
    float knockSpd;
    float knockRange;
    public void SetUp(PjBase user, float speed, float range, float dmg, float stunnTime, float knockSpd, float knockRange)
    {
        this.user = user;
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
        this.stunnTime = stunnTime;
        this.knockRange = knockRange;
        this.knockSpd = knockSpd;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerBarrier"))
        { 
            collision.GetComponent<TakeDamage>().TakeDamage(dmg, HitData.Element.ice);
            if (collision.CompareTag("Player"))
            {
                speed = 0;
                sprites.SetActive(false);
                collision.GetComponent<PjBase>().stunTime = stunnTime;
                Die();
            }
            else
            {
                Die();
            }
        }
        base.OnTriggerEnter2D(collision);
    }

}
