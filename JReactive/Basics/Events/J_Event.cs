using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// this is a base event we can reference on unity editor
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Events/Event")]
    public class J_Event : ScriptableObject, iBaseObservable
    {
        //the event raised by this property
        private event JAction OnEnter;

        //this is the property we want to track
        [ButtonGroup("Commands", 200), Button("Raise Event", ButtonSizes.Medium)]
        public virtual void RaiseEvent()
        {
            if (OnEnter != null) OnEnter();
        }

        //a way to subscribe and unsubscribe to this event
        public void Subscribe(JAction actionToSubscribe) { OnEnter   += actionToSubscribe; }
        public void UnSubscribe(JAction actionToSubscribe) { OnEnter -= actionToSubscribe; }
    }
}
