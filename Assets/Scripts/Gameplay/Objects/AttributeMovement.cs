using System;
using System.Collections.Generic;
using UnityEngine;


public class AttributeMovement : MonoBehaviour
{
    public enum MoveType { None = -1, Direct, Path, Curve }
    public float Speed;

    [HideInInspector]
    public bool Loop;

    [HideInInspector]
    public bool IsMoving = false;
    public List<Vector3> PathNodes { get; set; }
    public readonly float ApproximateFactor = 0.1f;

    protected int mNodeIndex = 0;
    protected Action mOnMoveDone;

    protected Vector3 mStartPoint, mMidPoint, mEndPoint;
    protected float mBazierPath;

    protected MoveType mMoveType = MoveType.None;

    void Awake() => Initialize();

    protected virtual void Initialize()
    {
        mNodeIndex = 0;
    }

    public virtual void MoveOnPath(List<Vector3> path, Action onMoveDone = null)
    {
        mMoveType = MoveType.Path;
        IsMoving = true;
        PathNodes = path;
        mNodeIndex = 0;
        mOnMoveDone = onMoveDone;
    }

    public virtual void MoveCurve(Vector3 from, Vector3 end, Action onMoveDone = null)
    {
        mMoveType = MoveType.Curve;
        mStartPoint = from;
        mEndPoint = end;
        mMidPoint = (from + end) * 2;
        mBazierPath = 0f;
        IsMoving = true;
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
            if (mMoveType == MoveType.Path) MovePath();
            else if (mMoveType == MoveType.Curve) MoveBezier();
        }

    }

    public virtual void MovePath()
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
                transform.position = PathNodes[PathNodes.Count - 1];
                OnMoveDone();
            }
        }
    }

    public virtual void MoveBezier()
    {
        float dist = (BezierPoint(mBazierPath + 0.01f, mStartPoint, mMidPoint, mEndPoint) - transform.position).magnitude;
        float f = 0.01f / dist;

        mBazierPath += Speed * f * Time.deltaTime;
        mBazierPath = Mathf.Clamp01(mBazierPath);
        transform.position = BezierPoint(mBazierPath, mStartPoint, mMidPoint, mEndPoint);
        if (Vector3.Distance(mEndPoint, transform.position) <= ApproximateFactor)
        {
            transform.position = mEndPoint;
            OnMoveDone();
        }
    }

    protected void OnMoveDone()
    {
        IsMoving = false;
        mMoveType = MoveType.None;
        mOnMoveDone?.Invoke();
    }

    Vector3 BezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1f - t;
        float uu = u * u;
        float tt = t * t;

        Vector3 p = uu * p0;
        p += 2f * u * t * p1;
        p += tt * p2;

        return p;
    }
}
