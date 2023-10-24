using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ValiFog : Spell
{
    public List<Enemy> enemyList = new List<Enemy>();
    public List<PjBase> allyList = new List<PjBase>();
    float slow;
    float spd;
    float atSpd;
    float iceDeb;
    public void SetUp(PjBase user, float duration,float slow, float spd, float atSpd, float iceDeb)
    {
        this.user = user;
        time = duration;
        this.slow = slow;
        this.spd = spd;
        this.atSpd = atSpd;
        this.iceDeb = iceDeb;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PjBase>())
        {
            SpellEnter(collision.GetComponent<PjBase>());
        }
        else if (collision.GetComponent<Enemy>())
        {
            SpellEnter(collision.GetComponent<Enemy>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PjBase>())
        {
            SpellExit(collision.GetComponent<PjBase>());
        }
        else if (collision.GetComponent<Enemy>())
        {
            SpellExit(collision.GetComponent<Enemy>());
        }
    }
    public override void SpellEnter(PjBase target)
    {
        allyList.Add(target);
        base.SpellEnter(target);
        if(CharacterManager.Instance.data[0].convergence >= 4)
        {
            target.AddComponent<ValiFogBuff>().SetUp(user.GetComponent<Vali>(), time, 0, spd, atSpd, 0);
        }
    }

    public override void SpellEnter(Enemy target)
    {
        enemyList.Add(target);
        if (CharacterManager.Instance.data[0].convergence >= 5)
        {
            target.AddComponent<ValiFogBuff>().SetUp(user.GetComponent<Vali>(), time, slow, 0, 0, iceDeb);
        }
        else
        {
            target.AddComponent<ValiFogBuff>().SetUp(user.GetComponent<Vali>(), time, slow, 0, 0, 0);
        }
        base.SpellEnter(target);
    }

    public override void SpellExit(PjBase target)
    {
        allyList.Remove(target);
        target.GetComponent<ValiFogBuff>().Die();
        base.SpellExit(target);
    }

    public override void SpellExit(Enemy target)
    {
        enemyList.Remove(target);
        target.GetComponent<ValiFogBuff>().Die();
        base.SpellExit(target);
    }
    public override void Die()
    {
        base.Die();
    }
}
