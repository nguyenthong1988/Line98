using System;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

public class UIPopup : UIBaseScreen
{
    [Header("Popup")]
    public Text MessageText;
    public Button PositiveButton;
    public Button NegativeButton;

    public string Message;
    public Action<object> OnPositive;
    public Action<object> OnNegative;

    protected object mData;

    public override void OnShow()
    {

    }

    public override void OnHide()
    {

    }

    public virtual void SetCallback(Action<object> positive = null, Action<object> negative = null)
    {
        OnPositive = positive;
        OnNegative = negative;
    }

    public virtual void SetMessage(string message)
    {
        Message = message;
        if (MessageText != null) MessageText.text = message;
    }

    public virtual void SetData(object data)
    {
        mData = data;
    }

    public override void OnAnimationIn()
    {
        //this.transform.GetChild(1).localScale = new Vector3(0f, 0f, 0f);
        //this.transform.GetChild(1).transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() => OnAnimationInEnd());
    }

    public override void OnAnimationOut()
    {
        //this.transform.GetChild(1).transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutCubic).OnComplete(() => OnAnimationOutEnd());
    }
}
