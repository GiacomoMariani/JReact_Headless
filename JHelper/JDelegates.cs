namespace JReact
{
    //the simple action
    public delegate void JAction();
    //an action related to activation
    public delegate void JActivationAction(bool isActive);
    //a generic delegate
    public delegate void JGenericDelegate<T>(T item);
}
