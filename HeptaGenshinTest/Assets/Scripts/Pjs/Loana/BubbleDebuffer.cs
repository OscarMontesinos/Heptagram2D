using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleDebuffer : Spell
{
    float slow;
    float exh;
    float stunn;
    public void SetUp(float slow, float exh, float stunn)
    {
        this.slow = slow;
        this.exh = exh;
        this.stunn = stunn;
        untimed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>())
        {
            SpellEnter(collision.GetComponent<Enemy>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>())
        {
            SpellExit(collision.GetComponent<Enemy>());
        }
    }

    public override void SpellEnter(Enemy target)
    {
        user.Stunn(target, stunn);
        target.stats.spd -= slow;
        target.stats.sinergy -= exh;
        base.SpellEnter(target);
    }

    public override void SpellExit(Enemy target)
    {
        target.stats.spd += slow;
        target.stats.sinergy += exh;
        base.SpellExit(target);
    }
}
