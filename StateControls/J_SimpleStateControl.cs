using UnityEngine;

namespace JReact.StateControl
{
    /// <summary>
    /// This class implements a state machine sending event to be tracked by other scripts using 
    /// Dependency Injection (we may inject just the desired state or this entire state control)
    /// EXAMPLES OF USAGE: menu flow and transitions, weather system, simple artificial intelligence,
    /// but also friend invitation or any other element that may be tracked with a state machine
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/State Control")]
    public class J_SimpleStateControl : J_StateControl<J_State>
    {
    }
}
