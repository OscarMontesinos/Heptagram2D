using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TakeDamage
{
    void TakeDamage(PjBase user,float value, HitData.Element element);
    void Stunn(float stunnTime);
    void Die();
}
