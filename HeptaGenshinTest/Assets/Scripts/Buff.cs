using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    [HideInInspector]
    public Vali user;
    [HideInInspector]
    public PjBase target;
    [HideInInspector]
    public Enemy eTarget;
    public float time;
    [HideInInspector]
    public bool untimed;

    // Update is called once per frame
    public virtual void Update()
    {
        if (!untimed)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                Die();
            }
        }
    }

    public virtual void Die()
    {
        Destroy(this);
    }
}
