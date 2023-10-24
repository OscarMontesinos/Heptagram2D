using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HitInteract
{
    void Interact(PjBase user, Enemy enemy, HitData.Element element, HitData.AttackType attackType, HitData.HabType habType)
    {

    }
}
