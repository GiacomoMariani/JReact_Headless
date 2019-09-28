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

        // --------------- INITIALIZATION --------------- //
        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_ThisReactiveItem, $"{gameObject.name} requires a {nameof(_ThisReactiveItem)}");
        }

        // --------------- VIEW UPDATES --------------- //
        //update of the selected element
        protected virtual void SelectionUpdate(T selectedElement) => ActorUpdate(selectedElement);

        // --------------- LISTENER SETUP --------------- //
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_ThisReactiveItem.Current != null) SelectionUpdate(_ThisReactiveItem.Current);
            _ThisReactiveItem.Subscribe(ActorUpdate);
        }

        private void OnDisable() => _ThisReactiveItem.UnSubscribe(SelectionUpdate);
    }
}
