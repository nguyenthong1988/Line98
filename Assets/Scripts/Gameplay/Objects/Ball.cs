using System;
using System.Collections.Generic;
using UnityEngine;

public class Ball : ObjectPool
{
    public enum Color { None = -1, Red, Green, Blue, Cyan, Magenta, Yellow, Brown, Ghost }
    public enum Size { Dot, Ball };

    public Color BallColor;
    public Size BallSize;

    public GameObject ExplosiveEffect;

    protected Animator mAnimator;
    protected FollowPath mFollowPath;

    protected override void Initialize()
    {
        base.Initialize();
        mAnimator = GetComponent<Animator>();
        mFollowPath = GetComponent<FollowPath>();
    }

    public void SetSize(Size size)
    {
        BallSize = size;
        float scaleRatio = size == Size.Dot ? 0.75f : 1.5f;
        transform.localScale = new Vector3(scaleRatio, scaleRatio);
    }

    public void Selected()
    { 
        SetTrigger("Selected");
    }

    public void Idle()
    {
        SetTrigger("Idle");
    }

    protected void SetTrigger(string trigger)
    {
        if (mAnimator && !string.IsNullOrEmpty(trigger))
        {
            mAnimator.SetTrigger(trigger);
        }
    }

    public void Move(List<Vector3> path, Action onMoveDone = null)
    { 
        if(mFollowPath && path != null && path.Count > 0)
        {
            mFollowPath.Move(path, onMoveDone);
        }
    }

    public void Explode()
    {
        PlayExplosiveEffect();
        Destroy();
    }

    public void PlayExplosiveEffect()
    {
        if (ExplosiveEffect)
        {
            GameObject instantiatedEffect = (GameObject)Instantiate(ExplosiveEffect, transform.position, transform.rotation);
            instantiatedEffect.transform.localScale = transform.localScale;
            Destroy(instantiatedEffect, 0.3f);
        }
    }
}

