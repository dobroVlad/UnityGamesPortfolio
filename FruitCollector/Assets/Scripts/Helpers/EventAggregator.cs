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
public class GameStartEvent{}
public class NewTaskEvent
{
    private FoodType _type;
    public FoodType Type => _type;
    public NewTaskEvent(FoodType type)
    {
        _type = type;
    }
}
public class TakeFoodEvent
{
    private FoodUnit _food;
    public FoodUnit Food => _food;
    public TakeFoodEvent(FoodUnit food)
    {
        _food = food;
    }
}
public class CaughtFoodEvent
{
    private FoodUnit _food;
    public FoodUnit Food => _food;
    public CaughtFoodEvent(FoodUnit food)
    {
        _food = food;
    }
}
public class PackedFoodEvent{}
public class GameFinishEvent{}
