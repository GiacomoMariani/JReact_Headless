using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControls
{
    /// <summary>
    /// this class is a scriptable object that contains a state that might be injected into any
    /// script who require to follow this state. It acts just like a JSimple event, but it has also
    /// an exit event and not just an enter event
    /// this script represent a game state
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Game States/J Reactive State")]
    public class J_State : J_Event
    {
        //the main event to set the new state
        private event JAction OnExitEvent;
        
        //to check if this state is active
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _isActivated;
        public bool IsActivated { get { return _isActivated; } private set { _isActivated = value; } }

        //raise event also activate the state
        public override void RaiseEvent()
        {
            IsActivated = true;
            base.RaiseEvent();
        }

        //this is the property we want to track
        [ButtonGroup("State trigger", 200), Button("Raise Exit Event", ButtonSizes.Medium)]
        public virtual void RaiseExitEvent()
        {
            IsActivated = false;
            if (OnExitEvent != null) OnExitEvent();
        }

        //a way to subscribe and unsubscribe to the exit event
        public void SubscribeToExitEvent(JAction actionToSend) { OnExitEvent += actionToSend; }
        public void UnSubscribeToExitEvent(JAction actionToSend) { OnExitEvent -= actionToSend; }
    }
}