using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValiMistArrow : Projectile
{
    PjBase user;
    public GameObject arrowSprite;
    public GameObject particleMist;
    public GameObject arrow;
    public float dmg;
    public float stunTime;
    public float arrowDetour;
    public float arrowBounceRange;
    public float arrowSpeed;
    public float arrowRange;
    public float arrowDmg;
    public float delay;
    public int times;
    

    public void SetUp(PjBase user, float speed, float range, float dmg, float stunTime, float arrowDetour, float arrowBounceRange, float arrowSpeed, float arrowRange, float arrowDmg,float delay, int times)
    {
        this.user = user;
        this.speed = speed;
        this.range = range;
        this.dmg = dmg;
        this.stunTime = stunTime;
        this.arrowDetour = arrowDetour;
        this.arrowBounceRange = arrowBounceRange;
        this.arrowSpeed = arrowSpeed;
        this.arrowRange = arrowRange;
        this.arrowDmg = arrowDmg;
        this.delay = delay;
        this.times = times;
    }

    override public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            arrowSprite.SetActive(false);
            speed = 0;
            RaycastHit2D ray2d = Physics2D.Raycast(transform.position, transform.up, 100, GameManager.Instance.wallLayer);
            
            Vector2 normal = collision.ClosestPoint(transform.position) - new Vector2(transform.position.x,transform.position.y);
            Vector2 reflect = Vector2.Reflect(transform.up, normal.normalized);
            StartCoroutine(Bounce(-normal));
        }
        else if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().GetComponent<TakeDamage>().TakeDamage(user, dmg, HitData.Element.ice);
            collision.GetComponent<Enemy>().GetComponent<TakeDamage>().Stunn(stunTime);
            user.DamageDealed(user, collision.GetComponent<Enemy>(), HitData.Element.ice, HitData.AttackType.range, HitData.HabType.hability);
            Die();
        }
    }

    IEnumerator Bounce(Vector2 reflect)
    {
        if (times > 0)
        {
            Instantiate(particleMist, transform.position, transform.rotation);
            yield return new WaitForSeconds(0.25f);
            List<GameObject> arrowList = new List<GameObject>();
            GameObject arrowRef = Instantiate(arrow, transform.position, transform.rotation);
            arrowRef.transform.up = reflect;
            arrowList.Add(arrowRef);
            arrowRef.GetComponent<ValiArrow>().SetUp(user, arrowSpeed, arrowBounceRange, arrowDmg, true, stunTime/10);
            GameObject arrowRef2 = Instantiate(arrow, transform.position, transform.rotation);
            arrowRef2.transform.up = reflect;
            arrowRef2.transform.eulerAngles = new Vector3(0, 0, arrowRef2.transform.eulerAngles.z + arrowDetour);
            arrowList.Add(arrowRef2);
            arrowRef2.GetComponent<ValiArrow>().SetUp(user, arrowSpeed, arrowBounceRange, arrowDmg, true, stunTime / 10);
            GameObject arrowRef3 = Instantiate(arrow, transform.position, transform.rotation);
            arrowRef3.transform.up = reflect;
            arrowRef3.transform.eulerAngles = new Vector3(0, 0, arrowRef3.transform.eulerAngles.z - arrowDetour);
            arrowList.Add(arrowRef3);
            arrowRef3.GetComponent<ValiArrow>().SetUp(user, arrowSpeed, arrowBounceRange, arrowDmg, true, stunTime / 10);
            List<Enemy> enemyList = new List<Enemy>();
            foreach (GameObject arrow in arrowList)
            {
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    Vector3 enemyDist = enemy.transform.position - transform.position;
                    if (Physics2D.Raycast(transform.position, enemyDist, arrowBounceRange, GameManager.Instance.enemyLayer) && !Physics2D.Raycast(transform.position, enemyDist, enemyDist.magnitude, GameManager.Instance.wallLayer) && !enemyList.Contains(enemy))
                    {
                        Vector2 enemyDir = enemy.transform.position - transform.position;
                        arrow.transform.up = enemyDir;
                        enemyList.Add(enemy);
                        break;
                    }
                }
            }
            times--;
            yield return new WaitForSeconds(delay); 
            StartCoroutine(Bounce(reflect));
        }
        else
        {
            Die();
        }
    }
}
