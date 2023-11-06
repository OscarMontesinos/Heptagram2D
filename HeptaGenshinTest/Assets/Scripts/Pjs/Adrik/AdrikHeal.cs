using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrikHeal : Projectile
{
    Adrik user;
    float heal;
    public void SetUp(PjBase user, float speed, float heal)
    {
        this.user = user.GetComponent<Adrik>();
        this.speed = speed;
        this.heal = heal;
        withoutRange = true;
        
    }

    public override void Update()
    {
        base.Update();
        Vector2 dir = user.transform.position - transform.position;
        transform.up = dir;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (PjBase pj in user.controller.team)
            {
                pj.Heal(heal, user.element);
            }
            if (CharacterManager.Instance.data[3].convergence >= 3)
            {
                user.currentHab2Cd -= user.c3CdReduc;
            }

            Die();
        }
        base.OnTriggerEnter2D(collision);
    }
}
