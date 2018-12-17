using UnityEngine;
using System.Collections;

public class ObjectBound : MonoBehaviour
{
    public enum BoundBasedOn { Collider2D, Renderer, Undefined }

    public BoundBasedOn BasedOnBound;

    public bool IsEnable { get; set; }
    public bool IsActive { get; set; }

    void Awake() => Initialize();
    void Start() { }
    void Update() { }

    protected virtual void Initialize() 
    {
        DefineBounds();
    }

    protected virtual void OnEnable() => IsEnable = true;
    protected virtual void OnDisable() => IsEnable = false;

    protected virtual void DefineBounds()
    {
        BasedOnBound = BoundBasedOn.Undefined;
        if (GetComponent<Renderer>() != null)
        {
            BasedOnBound = BoundBasedOn.Renderer;
        }
        if (GetComponent<Collider2D>() != null)
        {
            BasedOnBound = BoundBasedOn.Collider2D;
        }
    }

    public virtual Bounds Bounds
    {
        get 
        {
            if (BasedOnBound == BoundBasedOn.Renderer)
            {
                if (GetComponent<Renderer>() != null)
                    return GetComponent<Renderer>().bounds;
            }

            if (BasedOnBound == BoundBasedOn.Collider2D)
            {
                if (GetComponent<Collider2D>() != null)
                    return GetComponent<Collider2D>().bounds;
            }

            return new Bounds(Vector3.zero, Vector3.zero);
        }

    }
}
