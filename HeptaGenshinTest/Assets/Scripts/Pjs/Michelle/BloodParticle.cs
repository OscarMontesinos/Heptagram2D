using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BloodParticle : MonoBehaviour
{
    Michelle user;
    bool triggered;
    float speed;
    public float speedPerSecond;
    private void Awake()
    {
        user = FindObjectOfType<Michelle>();
    }

    public void Activate()
    {
        transform.parent = null;
        transform.localEulerAngles = Vector3.zero;
        triggered = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == user.gameObject)
        {
            user.Heal(user, user.CalculateControl(user.h2HealBlood), HitData.Element.blood);
            if (CharacterManager.Instance.data[8].convergence >= 7)
            {
                user.UpdatePool(user.CalculateControl(user.h2HealBlood));
            }
                Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (triggered)
        {
            Vector2 dir = user.transform.position - transform.position;
            transform.Translate(dir.normalized * speed);
            speed += speedPerSecond * Time.deltaTime;
        }
    }
}
