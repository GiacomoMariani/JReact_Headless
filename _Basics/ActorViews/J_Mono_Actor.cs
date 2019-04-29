using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// this class controls the main actor that is sent to all child of the type J_Mono_ActorElement
    /// this class is a way tp implement the actor pattern, explained in these slides
    /// https://gamedevacademy.org/lessons-learned-in-unity-after-5-years/
    /// </summary>
    /// <typeparam name="T">the type of the actor data to be controlled by this component</typeparam>
    public abstract class J_Mono_Actor<T> : MonoBehaviour
    {
        #region FIELDS AND PROPERTIES
        //sets the actor directly or by injection
        [BoxGroup("Setup", true, true, 0), SerializeField] protected T _actor;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _initCompleted;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private iUpdater<T>[] _relatedElements;
        #endregion

        #region INITIALIZATION
        //if we require any check on the derived class
        protected virtual void SanityChecks() {}

        //used to initialize this element, instead of using the awake
        protected virtual void InitThis()
        {
            if (_initCompleted) return;
            _relatedElements = GetComponentsInChildren<iUpdater<T>>(true);
            _initCompleted   = true;
        }
        #endregion

        /// <summary>
        /// we can use this to inject a new element
        /// </summary>
        /// <param name="element">the element to be injected</param>
        public void UpdateElement(T element)
        {
            if (_actor != null) RemovePreviousActor(_actor);
            _actor = element;
            SanityChecks();
            if (!_initCompleted) InitThis();
            UpdateAllViews(element);
            SpecificInitialization(element);
        }

        //if we need to deinitialize the previous actor
        protected virtual void RemovePreviousActor(T actorElement) {}

        //if we want to add further adjustments when we update the element
        protected virtual void SpecificInitialization(T element) {}

        //update all the views with the request
        protected virtual void UpdateAllViews(T element)
        {
            for (int i = 0; i < _relatedElements.Length; i++)
                UpdateView(_relatedElements[i], element);
        }

        /// <summary>
        /// this method is used to update all the specific views on this actor
        /// </summary>
        /// <param name="view">the view to updated</param>
        /// <param name="element">the element selected</param>
        protected virtual void UpdateView(iUpdater<T> view, T element) { view.UpdateThis(element); }

        //set the on enable listener
        protected virtual void OnEnable()
        {
            if (_actor != null) UpdateElement(_actor);
        }
    }
}
