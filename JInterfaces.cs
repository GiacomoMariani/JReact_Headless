namespace JReact
{
    public interface iSelectable<out T> where T : class
    {
        string NameOfThis { get; }
        bool IsSelected { get; set; }
        T ThisElement { get; }
    }

    public interface iInitiator<T>
    {
        void InjectThis(T elementToInject);
    }

    public interface iUpdater<T>
    {
        void UpdateThis(T elementToUpdate);
    }

    public interface iResettable
    {
        void ResetThis();
    }

    public interface iActivable : iResettable
    {
        bool IsActive { get; }
        void Activate();
    }

    public interface iObservable
    {
        void Subscribe(JAction action);
        void UnSubscribe(JAction action);
    }

    public interface iStateObservable : iTask, iObservable
    {
    }

    public interface iObservable<T>
    {
        void Subscribe(JGenericDelegate<T> action);
        void UnSubscribe(JGenericDelegate<T> action);
    }

    public interface iObservableValue<T> : iObservable<T>
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
        int MaxCapacity { get; }
        int FreeCapacity { get; }
        void SubscribeToMaxCapacity(JGenericDelegate<int> action);
        void UnSubscribeToMaxCapacity(JGenericDelegate<int> action);
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
}
