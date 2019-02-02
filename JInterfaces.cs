namespace JReact
{
    //something that may be filled
    public interface iFillable : iStackable
    {
        int MaxCapacity { get; }
        void SubscribeToMaxCapacity(JGenericDelegate<int> action);
        void UnSubscribeToMaxCapacity(JGenericDelegate<int> action);
    }
    
    //something that might be stackable, such as inventory items
    public interface iStackable : iObservable<int>
    {
        int CurrentAmount { get; }
    }
    
    public interface iSelectable<out T> where T : class
    {
        string NameOfThis { get; }
        bool IsSelected { get; set; }
        T ThisElement { get; }
    }

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
    
    public interface iActivable : iResettable
    {
        bool IsActive { get; }
        void Initialize();
    }

    //something that can be subscribed
    public interface iObservable
    {
        void Subscribe(JAction action);
        void UnSubscribe(JAction action);
    }
    
    //the state event
    public interface iStateObservable : iObservable
    {
        bool IsActive { get; }
        void SubscribeToExit(JAction action);
        void UnSubscribeToExit(JAction action);
    }

    public interface iObservable<T>
    {
        void Subscribe(JGenericDelegate<T> action);
        void UnSubscribe(JGenericDelegate<T> action);
    }

    //the interface to identify a task
    public interface iTask
    {
        string TaskName { get; }
        event JAction OnComplete;
        JAction ThisTask { get; }
        bool IsRunning { get; }
    }
}
