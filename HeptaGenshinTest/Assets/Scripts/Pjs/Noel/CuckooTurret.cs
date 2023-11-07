using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuckooTurret : MonoBehaviour
{
    public Noel user;
    public GameObject bullet;
    public GameObject rotator;
    public GameObject shootingPoint;
    public List<Enemy> targetList = new List<Enemy>();
    public Vector3 position;
    public float range;
    public float speed;
    public int attackCount;
    float attackTime;

    private void Awake()
    {

    }

    private void Update()
    {
        if (attackTime > 0)
        {
            attackTime -= Time.deltaTime;
        }
        else if(targetList.Count > 0)
        {
            StartCoroutine(Shoot());
        }

        Vector2 dir = position - transform.position;
        if (dir.magnitude > 0.3f)
        {
            transform.Translate(dir.normalized * speed * Time.deltaTime);
            rotator.transform.up = dir;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            targetList.Add(collision.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            targetList.Remove(collision.GetComponent<Enemy>());
        }
    }

    public void SetUp(Noel user, Vector2 position, int attackCount, float range, float speed)
    {
        this.user = user;
        this.position = position;
        this.range = range;
        this.attackCount = attackCount;
        this.speed = speed;
    }

    public IEnumerator Shoot()
    {
        Vector2 dir = position - transform.position;
        if (dir.magnitude <= 0.3f)
        {

            Enemy enemy = targetList[Random.Range(0, targetList.Count)];
            if(enemy == null)
            {
                targetList.Remove(enemy);
                yield break;
            }

            Vector2 dist = enemy.transform.position - transform.position;
            if (Physics2D.Raycast(transform.position, dist, range, GameManager.Instance.wallLayer))
            {
                yield return null;
                yield break;
            }

                attackTime = user.CalculateAtSpd(user.stats.atSpd *0.75f);


            rotator.transform.up = enemy.transform.position - transform.position;

            int shootCount = attackCount;

            while (shootCount > 0 && enemy != null && dir.magnitude <= 0.3f)
            {
                CuckooBullet bullet = Instantiate(this.bullet, shootingPoint.transform.position, rotator.transform.rotation).GetComponent<CuckooBullet>();
                if (shootCount == attackCount)
                {
                    bullet.SetUp(user, user.h1BulletSpd, user.h1Range, user.CalculateSinergy(user.h1Dmg), this, true);
                }
                else
                {
                    bullet.SetUp(user, user.h1BulletSpd, user.h1Range, user.CalculateSinergy(user.h1Dmg), this, false);
                }
                rotator.transform.up = enemy.transform.position - transform.position;
                yield return new WaitForSeconds(user.CalculateAtSpd(user.stats.atSpd) / (attackCount*4));
                rotator.transform.eulerAngles = new Vector3(rotator.transform.eulerAngles.x, rotator.transform.eulerAngles.y, rotator.transform.eulerAngles.z + Random.Range(-5f, 5f));
                shootCount--;
            }
        }

    }

}
