using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimatorScreen : UIBaseScreen
{
    public Animator animator;

    public string animationIn = "In", animationOut = "Out";

    protected IEnumerator mCouroutineIn = null, mCouroutineOut = null;

    public override void OnAnimationIn()
    {
        if (mCouroutineOut != null)
        {
            StopCoroutine(mCouroutineOut);
        }
        //coroutineIn = CoroutineIn();
        StartCoroutine(CoroutineIn());
    }

    public override void OnAnimationOut()
    {
        if (mCouroutineIn != null)
        {
            StopCoroutine(mCouroutineIn);
        }
        //couroutineOut = CoroutineOut();
        StartCoroutine(CoroutineOut());
    }

    IEnumerator CoroutineIn()
    {
        if (!string.IsNullOrEmpty(animationIn))
        {
            animator.Play(animationIn, -1, 0f);
            yield return null;
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            yield return null;
        }
        
        OnAnimationInEnd();
    }

    IEnumerator CoroutineOut()
    {
        if (!string.IsNullOrEmpty(animationIn))
        {
            animator.Play(animationOut, -1, 0f);
            yield return null;
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            yield return null;
        }
        OnAnimationOutEnd();
    }
}