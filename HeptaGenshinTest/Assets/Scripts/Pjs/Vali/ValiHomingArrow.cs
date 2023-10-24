using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ValiHomingArrow : Projectile
{
    PjBase user;
    public GameObject target;
    public float torque;
    float dmg;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetUp(PjBase user, GameObject target, float speed, float dmg, float torque)
    {
        this.target = target;
        this.user = user;
        this.speed = speed;
        this.range = 400;
        this.dmg = dmg;
        this.torque = torque;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector2 dir = target.transform.position - transform.position;
            dir.Normalize();
            float rotateAmount = Vector3.Cross(dir, transform.up).z;
            transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + -rotateAmount * torque * Time.deltaTime);
        }
        else
        {
            Die();
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && collision.gameObject == target)
        {
            collision.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(dmg, HitData.Element.ice);
            user.DamageDealed(user, collision.GetComponent<Enemy>(), HitData.Element.ice, HitData.AttackType.range, HitData.HabType.hability);
            Die();
        }
        base.OnTriggerEnter2D(collision);
    }
}
