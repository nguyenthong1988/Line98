using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : Pooler
{
    protected override void Initialize()
    {
        base.Initialize();
        mObjectPooler = new GameObject("[ObjectPooler] " + this.name + " " + gameObject.name);
        FillPool();
    }
}
