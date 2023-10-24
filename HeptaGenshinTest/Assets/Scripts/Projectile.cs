using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float range;
    Rigidbody2D _rigidbody;
    Vector2 startPos;
    public bool collideWalls;
    public GameObject particle;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }
    private void FixedUpdate()
    {
        Vector2 dir = transform.up;
        _rigidbody.velocity = dir.normalized * speed;
        Vector2 dist = startPos - new Vector2(transform.position.x, transform.position.y);
        if (dist.magnitude >range)
        {
            Die();
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") && collideWalls)
        {
            Die();
        }
    }

    public void Die()
    {
        if(particle != null)
        {
            particle.transform.parent = null;
        }
        Destroy(gameObject);
    }
}
