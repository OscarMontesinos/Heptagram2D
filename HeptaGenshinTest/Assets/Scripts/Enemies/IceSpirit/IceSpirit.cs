using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IceSpirit : Enemy
{
    public GameObject spell;
    public float spellSpeed;
    public float spellRange;
    public float spellDamage;
    public float atqSpd;
    public float atqSpdDetour;
    public float getCloserThreshold;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        AI();
    }


    public override void Update()
    {
        base.Update();

        if (target != null)
        {
            Vector2 dist = target.transform.position - transform.position;
            if (stunTime <= 0 && (dist.magnitude > getCloserThreshold || Physics2D.Raycast(transform.position, dist, dist.magnitude, GameManager.Instance.wallLayer)))
            {
                agent.SetDestination(target.transform.position);
            }
            else
            {
                agent.SetDestination(transform.position);
            }
        }
    }

    public override void AI()
    {
        if (target != null && stunTime <= 0)
        {
            StartCoroutine(ShootSpell(true));

        }
        else
        {
            StartCoroutine(RestartAi());
        }
        base.AI();
    }

    IEnumerator ShootSpell(bool loopAI)
    {
        if (stunTime <= 0)
        {
            PhantomSpell spell = Instantiate(this.spell, transform.position, pointer.transform.rotation).GetComponent<PhantomSpell>();
            spell.SetUp(this, spellSpeed, spellRange, CalculateSinergy(spellDamage));
        }
        yield return new WaitForSeconds(atqSpd + Random.Range(-atqSpdDetour, atqSpdDetour));
        if (loopAI)
        {
            AI();
        }
    }

}
