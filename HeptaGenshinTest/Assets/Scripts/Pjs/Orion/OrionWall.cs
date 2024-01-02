using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class OrionWall : MonoBehaviour, TakeDamage
{
    public Orion user;
    public Slider hpBar;
    public float mHp;
    public float hp;

    private void Start()
    {
        RechargeShield();
    }
    private void Update()
    {
    }
    public void RechargeShield()
    {
        hpBar.maxValue = mHp;
        hp = mHp;
        hpBar.value = hp;
    }
    void TakeDamage.TakeDamage(PjBase user, float value, HitData.Element element)
    {
        hp -= value;
        hpBar.value = hp;
        if(hp <= 0)
        {
            hp = 0;
            GetComponent<TakeDamage>().Die();
        }
    }
    void TakeDamage.Stunn(float stunTime)
    {
        user.stunTime += stunTime;
    }
    void TakeDamage.Die()
    {
        user.c3CurrentCd = user.c3Cd;
        gameObject.SetActive(false);
    }
}
