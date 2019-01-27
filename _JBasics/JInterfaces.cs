namespace JReact
{
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

    //something that can be subscribed
    public interface iObservable
    {
        void Subscribe(JAction action);
        void UnSubscribe(JAction action);
    }

    public interface iObservable<T>
    {
        void Subscribe(JGenericDelegate<T> action);
        void UnSubscribe(JGenericDelegate<T> action);
    }

    public interface iTrackable
    {
        bool IsTracking { get; }
        void StartTracking();
        void StopTracking();
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
