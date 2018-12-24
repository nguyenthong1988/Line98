using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CancelTrigger : MonoBehaviour, ICancelHandler
{
    [Header("UI Screen Manager Settings")]
    public bool DisableCancelHandler;

    private Action<BaseEventData> mCancel;

    public void SetCancelAction(Action<BaseEventData> _cancel)
    {
        mCancel = _cancel;
    }

    public void OnCancel(BaseEventData eventData)
    {
        if (!DisableCancelHandler && mCancel != null) mCancel.Invoke(eventData);
    }
}