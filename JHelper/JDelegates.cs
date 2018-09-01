namespace JReact
{
    //the simple action
    public delegate void JAction();
    //an action related to activation
    public delegate void JActivationAction(bool isActive);
    //a generic delegate
    public delegate void JGenericDelegate<T>(T item);
    
    //the interface to identify a task
    public interface iTask
    {
        string TaskName { get; }
        event JAction OnComplete;
        JAction ThisTask { get; }
        bool IsRunning { get; }
    }
}
