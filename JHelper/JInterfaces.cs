using System;
using UnityEngine.Events;

namespace JReact
{
    //an interface to setup the state of the injectors
    public interface iInitiator<T>
    {
        void InjectThis(T elementToInject);
    }

    //an element on the ui that requires update, different because here we expect multiple updates
    public interface iUpdater<T>
    {
        void UpdateThis(T elementToUpdate);
    }

    //elements that can be resetted
    public interface iResettable
    {
        void ResetThis();
    }

    //something that can be subscribed
    public interface iBaseObservable
    {
        void Subscribe(JAction action);
        void UnSubscribe(JAction action);
    }

    public interface iObservable<T>
    {
        void Subscribe(JGenericDelegate<T> action);
        void UnSubscribe(JGenericDelegate<T> action);
    }
}
