using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// this class is a component or view of an actor element and requires an J_Mono_Actor to be tracked
    /// </summary>
    /// <typeparam name="T">the type of the actor data to be controlled by this component</typeparam>
    public abstract class J_Mono_ActorElement<T> : MonoBehaviour, iUpdater<T>
    {
        #region FIELDS AND PROPERTIES
        //the package related to this view
        [BoxGroup("Base", true, true, -5), ReadOnly] protected T _actorElement;
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
        //update the element if we already have one
        protected virtual void OnEnable() { if (_actorElement != null) ActorUpdate(_actorElement); }

        //make sure everything is removed on destroy
        protected virtual void OnDisable() { if (_actorElement != null) ActorIsRemoved(_actorElement); }
        protected virtual void OnDestroy() { if (_actorElement != null) ActorIsRemoved(_actorElement); }
        #endregion
    }
}


