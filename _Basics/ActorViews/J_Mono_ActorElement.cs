using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// component or view of an actor element and requires an J_Mono_Actor to be tracked
    /// </summary>
    /// <typeparam name="T">the type of the actor data to be controlled by this component</typeparam>
    public abstract class J_Mono_ActorElement<T> : MonoBehaviour, iUpdater<T>
    {
        #region FIELDS AND PROPERTIES
        //the package related to this view
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected T _actorElement;
        #endregion

        //this is sent when the actor has been changed
        public void UpdateThis(T actor)
        {
            //remove the previous actor if any
            if (_actorElement != null) ActorIsRemoved(_actorElement);
            //set the new actor
            _actorElement = actor;
            ActorUpdate(actor);
        }

        #region ABSTRACT IMPLEMENTATION
        //change and remove actor methods
        protected abstract void ActorUpdate(T element);
        protected virtual void ActorIsRemoved(T element) { }
        #endregion

        #region UNITY EVENTS
        protected virtual void OnEnable() { if (_actorElement != null) ActorUpdate(_actorElement); }
        protected virtual void OnDisable() { if (_actorElement != null) ActorIsRemoved(_actorElement); }
        protected virtual void OnDestroy() { if (_actorElement != null) ActorIsRemoved(_actorElement); }
        #endregion
    }
}


