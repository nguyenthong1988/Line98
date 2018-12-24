using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class UITweenScreen : UIBaseScreen
{
    public override void OnAnimationIn()
    {
        mCanvasGroup.alpha = 1f;

        Transform[] allChildren = GetComponentsInChildren<RectTransform>();

        //foreach (var item in allChildren)
        //{
        //    if (item.transform == this.transform) continue;
        //    item.localScale = new Vector3(0f, 0f, 0f);
        //    item.gameObject.transform.DOScale(Vector3.one, Random.Range(0.2f, 0.7f))
        //        .SetDelay(Random.Range(0f, 0.6f))
        //        .SetEase(Ease.OutBack);
        //}

        //DOVirtual.DelayedCall(0.5f, () => OnAnimationInEnd());
    }

    public override void OnAnimationOut()
    {        
        //canvasGroup.alpha = 1f;
        //DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, 0.5f).SetEase(Ease.OutCubic).OnComplete(() => OnAnimationOutEnd());
    }
}