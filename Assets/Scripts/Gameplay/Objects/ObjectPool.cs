using UnityEngine;
using System.Collections;

public class ObjectPool : ObjectBound
{
    protected Pooler mPooler;

    public void AttachPooler(Pooler pooler)
    {
        mPooler = pooler;
    }

    public virtual void Destroy()
    {
        gameObject.SetActive(false);
        if (mPooler) gameObject.transform.parent = mPooler.gameObject.transform;
    }
}
