using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// this class represent an actor that may have many elements, implementing the actor pattern, explained in these slides
    /// https://gamedevacademy.org/lessons-learned-in-unity-after-5-years/
    /// remember to seal the derived class for better performance
    /// </summary>
    /// <typeparam name="T">the actor type</typeparam>
    public abstract class J_Mono_Actor<T> : MonoBehaviour
    {
        #region FIELDS AND PROPERTIES
        //sets the actor directly or by injection
        [InfoBox("Null => needs to be injected via code"), BoxGroup("Setup", true, true, 0), SerializeField]
        protected T _actor;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _initCompleted;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private iUpdater<T>[] _relatedElements;
        #endregion

        #region INITIALIZATION
        protected virtual void SanityChecks() {}

        protected virtual void InitThis()
        {
            if (_initCompleted) return;
            _relatedElements = GetComponentsInChildren<iUpdater<T>>(true);
            _initCompleted   = true;
        }
        #endregion

        public void ActorUpdate(T actor)
        {
            if (_actor != null) ActorRemoved(_actor);
            _actor = actor;
            SanityChecks();
            if (!_initCompleted) InitThis();
            UpdateAllViews(actor);
            ActorAdded(actor);
        }

        // --------------- VIEW UPDATE --------------- //
        protected virtual void UpdateAllViews(T element)
        {
            for (int i = 0; i < _relatedElements.Length; i++)
                UpdateView(_relatedElements[i], element);
        }

        /// <summary>
        /// updates the specific views on this actor
        /// </summary>
        protected virtual void UpdateView(iUpdater<T> view, T element) { view.UpdateThis(element); }

        // --------------- ABSTRACT IMPLEMENTATION --------------- //
        protected virtual void ActorRemoved(T actorElement) {}
        protected virtual void ActorAdded(T element) {}

        protected virtual void OnEnable()
        {
            if (_actor != null) ActorUpdate(_actor);
        }
    }
}
