using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventAggregator
{
    public static void Subscribe<T>(System.Action<object, T> eventCallback)
    {
        Event<T>.EventCallback += eventCallback;
    }
    public static void Unsubscribe<T>(System.Action<object, T> eventCallback)
    {
        Event<T>.EventCallback -= eventCallback;
    }
    public static void Post<T>(object sender, T eventData)
    {
        Event<T>.Post(sender, eventData);
    }
    private static class Event<T>
    {
        public static System.Action<object, T> EventCallback;

        public static void Post(object sender, T eventData)
        {
            EventCallback?.Invoke(sender, eventData);
        }
    }
}
public class GameOverEvent
{
    public bool Victory { get; private set; }
    public GameOverEvent (bool victory)
    {
        Victory = victory;
    }
}