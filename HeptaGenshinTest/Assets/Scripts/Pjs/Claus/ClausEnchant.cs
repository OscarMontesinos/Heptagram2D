using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClausEnchant : Spell
{
    List<PjBase> targets = new List<PjBase>();
    public ParticleSystem particles;
    int pulses;
    float heal;
    float pulseCd;
    float initialCd;
    float cd =1;

    public void SetUp(Claus user, int pulses, float heal, float initialCd, float pulseCd)
    {
        this.user = user;
        this.pulses = pulses;
        this.heal = heal;
        this.pulseCd = pulseCd;
        this.initialCd = initialCd;
        cd = initialCd;
        untimed = true;
    }

    public override void Update()
    {
        if (cd > 0)
        {
            cd -= Time.deltaTime;
        }
        else
        {
            if (pulses <= 0)
            {
                Die();
            }
            else
            {
                pulses--;
            }
            foreach (PjBase pj in targets)
            {
                if (pj.isActive)
                {
                    pj.Heal(user, heal, HitData.Element.ice);
                }
            }

            particles.Play();

            if (pulses <= 0)
            {
                cd = initialCd/1.5f;
            }
            else
            {
                cd = pulseCd;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SpellEnter(collision.GetComponent<PjBase>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SpellExit(collision.GetComponent<PjBase>());
        }
    }

    public override void SpellEnter(PjBase target)
    {
        targets.Add(target);
        base.SpellEnter(target);
    }
    public override void SpellExit(PjBase target)
    {
        targets.Remove(target);
        base.SpellEnter(target);
    }
}
