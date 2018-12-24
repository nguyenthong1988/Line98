using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIBaseScreen : MonoBehaviour, IComparable<UIBaseScreen>
{
    [HideInInspector]
    public readonly int ID = UniqueID<UIBaseScreen>.NextUID;

    [Header("Generation Settings")]
    public bool GenerateNavigation = true;

    public Selectable DefaultSelection;
    public Selectable CancelSelection;

    [Header("Settings")]
    public bool hideCurrent = true;
    public bool keepOnTopWhenHiding = true;

    public UIScreenManager.ScreenPriority Priority = UIScreenManager.ScreenPriority.Normal;

    [HideInInspector]
    public bool IsInit = false;

    [HideInInspector]
    public Action<UIBaseScreen> onShow, onHide;

    [HideInInspector]
    public GameObject SelectedObject;

    public bool Transition
    {
        get
        {
            return gameObject.activeSelf && isTransitioning;
        }
    }

    private bool mIsduplicate, canBeDestroyed, isTransitioning, mIsVisible;
    private UIScreenManager mUIManager;

    protected CanvasGroup mCanvasGroup;

    public void Initialize(UIScreenManager scrnMgr, bool isCopy = false)
    {
        mUIManager = scrnMgr;
        mIsduplicate = isCopy;
        mCanvasGroup = gameObject.GetComponentInChildren<CanvasGroup>();
        InteractionsEnabled(false);
        gameObject.SetActive(false);
        mIsVisible = false;

        var list = this.GetComponentsInChildren<CancelTrigger>(true);
        foreach (var subcancelTrigger in list)
        {
            //Debug.Log("subcancelTrigger " + name + " + " + subcancelTrigger + " / " + list.Length + "  -- " + submitSelection + " / " + cancelSelection, this.gameObject);
            if (CancelSelection != null) subcancelTrigger.SetCancelAction((e) => SelectOrInvokeButton(CancelSelection.gameObject, e));
        }

        OnInitialize();
    }

    public virtual void OnInitialize() { }

    public virtual void OnShow() { }

    public virtual void OnHide() { }

    public virtual void OnAnimationIn()
    {
        OnAnimationInEnd();
    }

    public virtual void OnAnimationOut()
    {
        OnAnimationOutEnd();
    }

    public void OnAnimationInEnd()
    {
        InteractionsEnabled(true);
        isTransitioning = false;
        Debug.Log("OnAnimation  In  End : " + name);
    }

    public void OnAnimationOutEnd()
    {
        gameObject.SetActive(false);
        isTransitioning = false;
        if (mIsduplicate && canBeDestroyed) Destroy(gameObject);
        if (onHide != null) onHide.Invoke(this);
        Debug.Log("OnAnimation  Out  End : " + name);
    }

    public void OnDeactivated(bool hide, bool destroy = false)
    {
        Debug.Log("BLUR ( " + hide + " ) : " + name);
        if (!IsInit) return;

        if (hide)
        {
            if (destroy) canBeDestroyed = destroy;

            if (mIsVisible)
            {
                isTransitioning = true;
                InteractionsEnabled(false);
                OnAnimationOut();
                OnHide();
            }

            mIsVisible = false;
        }
        else if (mIsVisible)
        {
            mUIManager.StartCoroutine(CoroutineInteractionsEnabled(false));
        }
    }

    public void OnActivated()
    {
        Debug.Log("FOCUS : " + name);

        gameObject.SetActive(true);

        if (!mIsVisible)
        {
            if (!IsInit) IsInit = true;

            mIsVisible = true;
            isTransitioning = true;
            OnAnimationIn();
            OnShow();
            if (onShow != null) onShow.Invoke(this);
        }
        else
        {
            mUIManager.StartCoroutine(CoroutineInteractionsEnabled(true));
        }
        transform.SetAsLastSibling();
    }

    private IEnumerator CoroutineInteractionsEnabled(bool enabled)
    {

        while (isTransitioning)
        {
            yield return new WaitForEndOfFrame();
        }

        mCanvasGroup.blocksRaycasts = enabled;
        //canvasGroup.interactable = enabled;

        if (enabled) yield return mUIManager.StartCoroutine(CoroutineInternalSelect());
    }

    private void InteractionsEnabled(bool enabled)
    {
        mCanvasGroup.blocksRaycasts = enabled;
        //canvasGroup.interactable = enabled;

        if (enabled) mUIManager.StartCoroutine(CoroutineInternalSelect());
    }

    private IEnumerator CoroutineInternalSelect()
    {
        yield return new WaitForEndOfFrame();
        if (!mIsVisible) yield break;

        GameObject go = SelectedObject ?? (DefaultSelection != null ? DefaultSelection.gameObject : FindFirstEnabledSelectable(gameObject));
        SetSelected(go);
    }

    private static GameObject FindFirstEnabledSelectable(GameObject gameObject)
    {
        GameObject go = null;
        var selectables = gameObject.GetComponentsInChildren<Selectable>(true);
        foreach (var selectable in selectables)
        {
            if (selectable.IsActive() && selectable.IsInteractable())
            {
                go = selectable.gameObject;
                break;
            }
        }
        return go;
    }

    private void SetSelected(GameObject go)
    {
        if (mUIManager.IsTouchMode) return;

        EventSystem.current.SetSelectedGameObject(go);

        var standaloneInputModule = EventSystem.current.currentInputModule as StandaloneInputModule;
        if (standaloneInputModule != null) return;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SelectOrInvokeButton(GameObject go, BaseEventData e)
    {
        if (!mUIManager.InstantCancelButton && EventSystem.current.currentSelectedGameObject != go)
        {
            if (!mUIManager.IsTouchMode) EventSystem.current.SetSelectedGameObject(go);
        }
        else
        {
            go.GetComponent<ISubmitHandler>().OnSubmit(e);
        }
    }

    public int CompareTo(UIBaseScreen obj)
    {
        int result = 0;

        if ((int)Priority != (int)obj.Priority)
        {
            result = ((int)Priority).CompareTo((int)obj.Priority);
        }

        return result;
    }
}
