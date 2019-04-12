﻿using System;

namespace JReact
{
    public interface iSelectable<out T> where T : class
    {
        string NameOfThis { get; }
        bool IsSelected { get; set; }
        T ThisElement { get; }
    }

    public interface iInitiator<in T>
    {
        void InjectThis(T elementToInject);
    }

    public interface iUpdater<in T>
    {
        void UpdateThis(T elementToUpdate);
    }

    public interface iResettable
    {
        void ResetThis();
    }

    public interface iDestroyable
    {
        void DestroyThis();
    }

    public interface iActivable : iResettable
    {
        bool IsActive { get; }
        void Activate();
        void End();
    }

    public interface iObservable
    {
        void Subscribe(JAction action);
        void UnSubscribe(JAction action);
    }

    public interface iStateObservable : iTask, iObservable
    {
    }

    public interface iObservable<out T>
    {
        void Subscribe(Action<T> action);
        void UnSubscribe(Action<T> action);
    }

    public interface iObservableValue<out T> : iObservable<T>
    {
        T CurrentValue { get; }
    }

    public interface iStackable : iObservableValue<int>
    {
        int Grant(int amount);
        int Remove(int amount);
    }

    public interface iFillable : iStackable
    {
        int Max { get; }
        int FreeCapacity { get; }
        void SubscribeToMaxCapacity(Action<int> action);
        void UnSubscribeToMaxCapacity(Action<int> action);
    }

    public interface iTask
    {
        string Name { get; }
        bool IsActive { get; }
        void Activate();
        void SubscribeToEnd(JAction action);
        void UnSubscribeToEnd(JAction action);
    }

    //an interface of a ticker that may give a timer
    public interface iDeltaTime
    {
        float ThisDeltaTime { get; }
    }

    public interface iInputAxisGetter
    {
        float GetAxis(string axisId);
        float GetAxisRaw(string axisId);
    }

    public interface iTestable
    {
        void RunTest();
        void StopTest();
    }
}
