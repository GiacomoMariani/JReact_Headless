using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// an actor related to a selector
    /// </summary>
    /// <typeparam name="T">a selectable</typeparam>
    public abstract class J_Mono_ReactiveActor<T> : J_Mono_Actor<T>
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [BoxGroup("Setup", true, true), ReadOnly, ShowInInspector] protected abstract jObservableValue<T> _ThisReactiveItem { get; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected T _current;

        // --------------- INITIALIZATION --------------- //
        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_ThisReactiveItem, $"{gameObject.name} requires a {nameof(_ThisReactiveItem)}");
        }
        
        // --------------- TRACKING --------------- //
        private void TrackSelection()
        {
            if (_ThisReactiveItem.Current != null) ActorUpdate(_ThisReactiveItem.Current);
            _ThisReactiveItem.Subscribe(SelectionUpdate);
        }
        protected virtual void StopTracking() => _ThisReactiveItem.UnSubscribe(SelectionUpdate);

        // --------------- VIEW UPDATES --------------- //
        //update of the selected element
        protected virtual void SelectionUpdate(T selectedElement)
        {
            _current = selectedElement;
            ActorUpdate(selectedElement);
        }

        // --------------- LISTENER SETUP --------------- //
        protected override void OnEnable()
        {
            base.OnEnable();
            TrackSelection();
        }

        private void OnDisable() => StopTracking();

    }
}
