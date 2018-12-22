using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pooler : MonoBehaviour
{
    public static Pooler Instance = null;

    public ObjectPool Template;
    public int PoolSize = 0;
    public bool UseFixedPoolSize = false;

    protected List<GameObject> mPooledObjects;
    protected GameObject mObjectPooler;

    void Awake() => Initialize();

    protected virtual void Initialize()
    {
        Instance = this;
    }

    public Pooler() { }
    public Pooler(string name) { }

    public void FillPool()
    {
        if (mPooledObjects == null) mPooledObjects = new List<GameObject>();

        for (int i = 0; i < PoolSize; i++)
        {
            AddObjectToPool();
        }
    }

    protected virtual GameObject AddObjectToPool()
    {
        GameObject obj = Instantiate(Template).gameObject;
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(mObjectPooler != null ? mObjectPooler.transform : transform);
        obj.name = Template.name + "-" + mPooledObjects.Count;
        mPooledObjects.Add(obj);

        return obj;
    }

    public virtual GameObject GetObject()
    {
        for (int i = 0; i < mPooledObjects.Count; i++)
        {
            if (!mPooledObjects[i].gameObject.activeInHierarchy)
            {
                return mPooledObjects[i];
            }
        }

        if(!UseFixedPoolSize) return AddObjectToPool();

        return null;
    }

    public virtual GameObject Spawn(Vector3 position, Transform parent = null)
    {
        GameObject obj = GetObject();

        if (obj)
        {
            obj.SetActive(true);
            obj.transform.SetParent(parent);
            obj.transform.position = position;
        }

        return obj;
    }

    public virtual GameObject Spawn(Vector3 position, Quaternion rotation , Vector3 scale, Transform parent = null)
    {
        GameObject obj = GetObject();

        if(obj)
        {
            obj.SetActive(true);
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.localScale = scale;
        }

        return obj;
    }
}
