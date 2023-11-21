using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class KojiKnife : MonoBehaviour
{
    Koji user;
    Rigidbody2D rb;
    public List<Enemy> targetList = new List<Enemy>();
    public GameObject point;
    float dmg;
    float spd;
    float detour;
    float extraRange;
    public float weight;
    public float height;
    public bool dashing;

    private void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (targetList.Count > 0 && !dashing)
        {
            if (targetList[0] == null)
            {
                targetList.RemoveAt(0);
            }
            else
            {
                Vector2 dir = targetList[0].transform.position - transform.position;
                transform.up = dir;
                transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + Random.Range(-detour, detour));
                StartCoroutine(Dash(transform.up, spd, dir.magnitude + extraRange));
                targetList.RemoveAt(0);
            }
        }
    }

    public void SetUp(Koji user, float dmg, float spd, float detour, float extraRange, float initialRange)
    {
        this.user = user;
        this.dmg = dmg;
        this.spd = spd;
        this.detour = detour;
        this.extraRange = extraRange;

        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + Random.Range(-detour, detour));
        StartCoroutine(Dash(transform.up, spd, initialRange));
    }


    public virtual IEnumerator Dash(Vector2 direction, float speed, float range)
    {
        dashing = true;
        yield return new WaitForSeconds(user.CalculateAtSpd(user.stats.atSpd * 3));
        Vector2 destinyPoint = new Vector2(transform.position.x, transform.position.y) + direction.normalized * range;
        Vector2 distance = destinyPoint - new Vector2(transform.position.x, transform.position.y);
        yield return null;

        List<Enemy> enemiesHitted = new List<Enemy>();

        while (distance.magnitude > 1 && dashing)
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


            Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(point.transform.position, new Vector2(weight, height), transform.localEulerAngles.z, GameManager.Instance.enemyLayer);
            Enemy enemy;
            foreach (Collider2D enemyColl in enemiesHit)
            {
                enemy = enemyColl.GetComponent<Enemy>();
                if (!enemiesHitted.Contains(enemy))
                {
                    enemiesHitted.Add(enemy);

                    enemy.GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.wind);
                    user.DamageDealed(user, enemy, HitData.Element.wind, HitData.AttackType.melee, HitData.HabType.basic);
                }
            }
        }
        dashing = false;
        rb.velocity = new Vector2(0, 0);

        
    }




    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(point.transform.position, new Vector3(weight, height, 1));
    }
}
