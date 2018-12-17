using System;
using System.Collections;
using System.Collections.Generic;

public interface IEventBase { };

public interface IEvent<T> : IEventBase
{
    void OnEvent(T eventType);
}

public static class GameEvent
{
    private static Dictionary<Type, List<IEventBase>> mSubscribers = new Dictionary<Type, List<IEventBase>>();

    static GameEvent()
    {
    }

    public static void AddListener<Event>(IEvent<Event> e) where Event : struct
    {
        Type eventType = typeof(Event);

        if (!mSubscribers.ContainsKey(eventType))
            mSubscribers[eventType] = new List<IEventBase>();

        if (!IsSubscriberExists(eventType, e))
            mSubscribers[eventType].Add(e);
    }

    public static void RemoveListener<Event>(IEvent<Event> e) where Event : struct
    {
        Type eventType = typeof(Event);

        if (!mSubscribers.ContainsKey(eventType))
        {
            return;
        }

        List<IEventBase> subscribers = mSubscribers[eventType];

        for (int i = 0; i < subscribers.Count; i++)
        {
            if (subscribers[i] == subscribers)
            {
                subscribers.Remove(subscribers[i]);

                if (subscribers.Count == 0)
                    mSubscribers.Remove(eventType);

                return;
            }
        }
    }

    private static bool IsSubscriberExists(Type type, IEventBase e)
    {
        List<IEventBase> subscribers;

        if (!mSubscribers.TryGetValue(type, out subscribers)) return false;

        bool exists = false;

        for (int i = 0; i < subscribers.Count; i++)
        {
            if (subscribers[i] == e)
            {
                exists = true;
                break;
            }
        }

        return exists;
    }

    public static void TriggerEvent<Event>(Event e) where Event : struct
    {
        List<IEventBase> subscribers;
        if (!mSubscribers.TryGetValue(typeof(Event), out subscribers))
            return;

        for (int i = 0; i < subscribers.Count; i++)
        {
            (subscribers[i] as IEvent<Event>).OnEvent(e);
        }
    }
}