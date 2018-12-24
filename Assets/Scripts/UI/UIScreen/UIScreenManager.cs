using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIScreenManager : MonoBehaviour
{
    public enum ScreenPriority : int
    {
        Low = -100,
        Normal = 0,
        Popup = 100,
        High = 200,
        Alert = 300,
    };

    public UIBaseScreen DefaultScreen;

    public bool alwaysOnSelection = false;

    public bool IsTouchMode = false;

    public bool InstantCancelButton = false;

    [NonSerialized]
    public UIBaseScreen CurrentScreen = null;

    public List<UIBaseScreen>.Enumerator Breadcrumbs
    {
        get
        {
            return screenQueue.GetEnumerator();
        }
    }

    public Action<UIBaseScreen> onScreenShow, onScreenHide;

    private Dictionary<string, UIBaseScreen> mListScreen;

    private List<UIBaseScreen> screenQueue;

    private GameObject lastSelection;

    private bool mScreenQueueDirty = false;
    private UIBaseScreen mScreenToKill = null;
    private UIBaseScreen mScreenToKeepOnTop = null;
    private UIBaseScreen mScreenToShowInTheEnd = null;

    private void Awake()
    {
        Initialize();
    }
    public void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(CoroutineUpdate());
    }
    public void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Initialize()
    {
        ServiceLocator.Register<UIScreenManager>(this, true);

        screenQueue = new List<UIBaseScreen>(50);

        mListScreen = new Dictionary<string, UIBaseScreen>();

        foreach (UIBaseScreen screen in GetComponentsInChildren<UIBaseScreen>(true))
        {
            screen.Initialize(this, false);
            mListScreen[screen.name] = screen;
        }

        ShowDefault();

        StartCoroutine(CoroutineUpdate());
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator CoroutineUpdate()
    {
        var waitTime = new WaitForSecondsRealtime(0.1f);

        while (true)
        {
            if (mScreenQueueDirty)
            {
                if (mScreenToKill != null && mScreenToKill == CurrentScreen)
                {
                    if (onScreenHide != null) onScreenHide.Invoke(CurrentScreen);

                    int screenToKillIndex = screenQueue.FindLastIndex(x => x == mScreenToKill);
                    if (screenToKillIndex != -1) screenQueue.RemoveAt(screenToKillIndex);

                    EventSystem.current.SetSelectedGameObject(null);
                    mScreenToKill.SelectedObject = null;
                    mScreenToKill.OnDeactivated(true, true);
                    if (mScreenToKill.keepOnTopWhenHiding) mScreenToKeepOnTop = mScreenToKill;
                    mScreenToKill = null;
                    CurrentScreen = null;
                }

                if (screenQueue.Count == 0 && mScreenToShowInTheEnd != null)
                {
                    screenQueue.Add(mScreenToShowInTheEnd);
                    mScreenToShowInTheEnd = null;
                }

                UIBaseScreen maxPriorityScreen = screenQueue.LastOrDefault();

                if (CurrentScreen != maxPriorityScreen)
                {
                    UIBaseScreen previousScreen = CurrentScreen;

                    if (previousScreen != null)
                    {
                        previousScreen.SelectedObject = EventSystem.current.currentSelectedGameObject;
                        EventSystem.current.SetSelectedGameObject(null);
                    }

                    if (maxPriorityScreen.Transition)
                    {
                        CurrentScreen = null;
                        mScreenQueueDirty = true;
                        yield return waitTime;
                        continue;
                    }
                    else
                    {
                        CurrentScreen = maxPriorityScreen;

                        if (CurrentScreen == null && DefaultScreen != null) CurrentScreen = DefaultScreen;

                        if (CurrentScreen != null)
                        {
                            if (onScreenShow != null) onScreenShow.Invoke(CurrentScreen);
                            CurrentScreen.OnActivated();
                        }

                        if (previousScreen != null)
                        {
                            previousScreen.OnDeactivated(CurrentScreen.hideCurrent);
                        }

                        if (mScreenToKeepOnTop != null && mScreenToKeepOnTop.isActiveAndEnabled)
                        {
                            mScreenToKeepOnTop.transform.SetAsLastSibling();
                            mScreenToKeepOnTop = null;
                        }
                    }
                }

                mScreenQueueDirty = false;
            }

            if (!IsTouchMode && alwaysOnSelection)
            {
                if (CurrentScreen != null && !CurrentScreen.Transition)
                {
                    GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
                    bool isCurrentActive = (selectedGameObject != null && selectedGameObject.activeInHierarchy);

                    if (!isCurrentActive)
                    {
                        if (lastSelection != null && lastSelection.activeInHierarchy && lastSelection.transform.IsChildOf(CurrentScreen.transform))
                        {
                            EventSystem.current.SetSelectedGameObject(lastSelection);
                        }
                        else if (CurrentScreen.DefaultSelection != null && CurrentScreen.DefaultSelection.gameObject.activeInHierarchy)
                        {
                            EventSystem.current.SetSelectedGameObject(CurrentScreen.DefaultSelection.gameObject);
                            lastSelection = CurrentScreen.DefaultSelection.gameObject;
                        }
                    }
                    else
                    {
                        lastSelection = selectedGameObject;
                    }
                }
            }

            yield return waitTime;
        }
    }

    public T ShowPopup<T>(string screenName, string data = null, Action<object> positive = null, Action<object> negative = null) where T : UIPopup
    {
        if (!mListScreen.ContainsKey(screenName))
        {
            throw new KeyNotFoundException("ScreenManager: Show failed. Screen with name '" + screenName + "' does not exist.");
        }

        GameObject newDupeScreen = GameObject.Instantiate(mListScreen[screenName].gameObject);
        newDupeScreen.transform.SetParent(transform, false);
        UIPopup popup = newDupeScreen.GetComponent<UIPopup>();
        popup.Initialize(this, true);
        popup.SetData(data);
        popup.SetCallback(positive, negative);

        newDupeScreen.name = screenName + " (" + (popup.ID) + ")";


        return ShowScreen(popup, true) as T;
    }

    public void ShowDefault(bool force = false)
    {
        if (DefaultScreen != null) ShowScreen(DefaultScreen, force);
    }

    public void HideAllAndShow(string screenName)
    {
        if (!mListScreen.ContainsKey(screenName))
        {
            throw new KeyNotFoundException("ScreenManager: HideAllAndShow failed. Screen with name '" + screenName + "' does not exist.");
        }
        HideAll();
        mScreenToShowInTheEnd = mListScreen[screenName];
    }

    public void HideAllAndShow(UIBaseScreen screen)
    {
        HideAll();
        mScreenToShowInTheEnd = screen;
    }

    public void Show(string screenName)
    {
        ShowScreen(screenName);
    }

    public void Show(UIBaseScreen screen)
    {
        ShowScreen(screen);
    }

    public UIBaseScreen ShowScreen(string screenName, bool force = false)
    {
        if (!mListScreen.ContainsKey(screenName))
        {
            throw new KeyNotFoundException("ScreenManager: Show failed. Screen with name '" + screenName + "' does not exist.");
        }
        return ShowScreen(mListScreen[screenName], force);
    }

    public UIBaseScreen ShowScreen(UIBaseScreen screen, bool force = false)
    {
        if (screen == null)
        {
            throw new KeyNotFoundException("UIScreenManager: Show(UIBaseScreen) failed, screen is Null.");
        }

        if (!force && (mScreenQueueDirty || screenQueue.LastOrDefault() == screen))
        {
            return screen;
        }

        screenQueue.Add(screen);
        InsertionSort(screenQueue);

        if (CurrentScreen == null || (int)CurrentScreen.Priority <= (int)screen.Priority)
        {
            if (CurrentScreen != null) CurrentScreen.OnDeactivated(false);
            mScreenQueueDirty = true;
        }

        return screen;
    }

    public void Hide()
    {
        if (!mScreenQueueDirty && CurrentScreen != null && CurrentScreen.Transition) return;

        mScreenToKill = CurrentScreen;
        mScreenQueueDirty = true;
    }

    public void HideAll()
    {
        foreach (var item in screenQueue)
        {
            if (item == CurrentScreen) continue;
            item.SelectedObject = null;
            item.OnDeactivated(true, true);
        }
        screenQueue.Clear();

        mScreenToKill = CurrentScreen;
        mScreenQueueDirty = true;
    }

    private static void InsertionSort(IList<UIBaseScreen> list)
    {
        if (list.IsNullOrEmpty()) return;

        int count = list.Count;
        for (int j = 1; j < count; j++)
        {
            UIBaseScreen key = list[j];

            int i = j - 1;
            for (; i >= 0 && CompareBaseScreens(list[i], key) > 0; i--)
            {
                list[i + 1] = list[i];
            }
            list[i + 1] = key;
        }
    }

    private static int CompareBaseScreens(UIBaseScreen x, UIBaseScreen y)
    {
        int result = 1;
        if (x != null && x is UIBaseScreen &&
            y != null && y is UIBaseScreen)
        {
            UIBaseScreen screenX = (UIBaseScreen)x;
            UIBaseScreen screenY = (UIBaseScreen)y;
            result = screenX.CompareTo(screenY);
        }
        return result;
    }
}
