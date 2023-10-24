using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TakeDamage
{
    void TakeDamage(float value, HitData.Element element);
    void Stunn(float stunnTime);
    void Die();
}
