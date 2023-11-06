using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : Barrier
{
    public BubbleDebuffer debuffer;

    public override void SetUp(PjBase user, float hp, float duration)
    {
        base.SetUp(user, hp, duration);
        Loana loana = user.GetComponent<Loana>();
        debuffer.SetUp(loana.c7Slow, user.CalculateControl(loana.c7DebuffAmount), loana.c7StunnTime);
    }
}
