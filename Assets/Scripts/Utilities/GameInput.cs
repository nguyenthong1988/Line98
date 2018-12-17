using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : Singleton<GameInput>
{
    public enum TouchType { TouchDown, TouchUp, TouchMove }

    protected Dictionary<TouchType, List<Action<Vector3>>> mTouchListeners = new Dictionary<TouchType, List<Action<Vector3>>>();
    protected Camera mMainCamera;

    void Start()
    {
        mMainCamera = Camera.main;
    }

    void Update()
    {
        ProcessTouch();
    }

    protected virtual void ProcessTouch()
    {
        Vector3 position;

        if (Input.GetMouseButton(0))
        {
            position = mMainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (mTouchListeners.ContainsKey(TouchType.TouchMove))
            {
                foreach (var action in mTouchListeners[TouchType.TouchMove])
                {
                    action(new Vector3(position.x, position.y, 0.0f));
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            position = mMainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (mTouchListeners.ContainsKey(TouchType.TouchDown))
            {
                foreach (var action in mTouchListeners[TouchType.TouchDown])
                {
                    action(new Vector3(position.x, position.y, 0.0f));
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            position = mMainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (mTouchListeners.ContainsKey(TouchType.TouchUp))
            {
                foreach (var action in mTouchListeners[TouchType.TouchUp])
                {
                    action(new Vector3(position.x, position.y, 0.0f));
                }
            }
        }
    }

    protected void AddTouchListener(TouchType type, Action<Vector3> action)
    {
        if (!mTouchListeners.ContainsKey(type))
            mTouchListeners[type] = new List<Action<Vector3>>();


        for (int i = 0; i < mTouchListeners[type].Count; i++)
        {
            if (mTouchListeners[type][i] == action)
            {
                return;
            }
        }

        mTouchListeners[type].Add(action);
    }

    protected void RemoveTouchListener(TouchType type, Action<Vector3> action)
    {
        if (!mTouchListeners.ContainsKey(type))
        {
            return;
        }

        for (int i = 0; i < mTouchListeners[type].Count; i++)
        {
            mTouchListeners[type].Remove(action);
        }
    }

    public static void RegisterListener(TouchType type, Action<Vector3> action)
    {
        Instance.AddTouchListener(type, action);
    }

    public static void UnRegisterListener(TouchType type, Action<Vector3> action)
    {
        Instance.RemoveTouchListener(type, action);
    }
}
