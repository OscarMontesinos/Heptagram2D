using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    public float mHp;
    [HideInInspector]
    public float hp;
    [HideInInspector]
    public float shield;

    public float sinergy;
    public float atSpd;
    public float control;
    public float cdr;
    public float spd;

    public float iceResist;
    public float fireResist;
    public float waterResist;
    public float desertResist;
    public float windResist;
    public float natureResist;
    public float lightningResist;
}
