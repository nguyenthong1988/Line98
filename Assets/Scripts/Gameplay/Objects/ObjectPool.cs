using UnityEngine;
using System.Collections;

public class ObjectPool : ObjectBound
{
    public virtual void Destroy()
    {
        gameObject.SetActive(false);
    }
}
