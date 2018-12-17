using System;
using System.Collections.Generic;
using UnityEngine;


public class FollowPath : MonoBehaviour
{
    public float Speed;

    [HideInInspector]
    public bool Loop;

    [HideInInspector]
    public bool IsMoving = false;
    public List<Vector3> PathNodes { get; set; }
    public readonly float ApproximateFactor = 0.1f;

    protected int mNodeIndex = 0;
    protected Action mOnMoveDone;

    void Awake() => Initialize();

    protected virtual void Initialize()
    {
         mNodeIndex = 0;
    }

    public virtual void Move(List<Vector3> path, Action onMoveDone = null)
    {
        IsMoving = true;
        PathNodes = path;
        mNodeIndex = 0;
        mOnMoveDone = onMoveDone;
    }

    public virtual void Pause()
    {
        IsMoving = false;
    }

    public virtual void Update()
    {
        if (IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, PathNodes[mNodeIndex], Time.deltaTime * Speed);
            if (Vector3.Distance(PathNodes[mNodeIndex], transform.position) <= ApproximateFactor)
            {
                mNodeIndex++;
            }

            if (mNodeIndex >= PathNodes.Count)
            {
                if (Loop)
                {
                    mNodeIndex = 0;
                    transform.position = PathNodes[0];
                }
                else
                {
                    IsMoving = false;
                    mOnMoveDone?.Invoke();
                    transform.position = PathNodes[PathNodes.Count - 1];
                }
            }
        }

    }
}
