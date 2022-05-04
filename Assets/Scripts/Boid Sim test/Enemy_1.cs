using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Member
{
    protected override Vector3 Combine()
    {
        return cfg.wanderPriority * Wander();
    }
}
