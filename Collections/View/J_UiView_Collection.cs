using System.Collections.Generic;
using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.UiView.Collections
{
    /// <summary>
    /// shows a collection of elements
    /// </summary>
    public abstract class J_UiView_Collection<T> : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract J_ReactiveCollection<T> _Collection { get; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract J_Mono_Actor<T> _PrefabActor { get; }
        //the dictionary is used for safety and to track the current elements on this viewer
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        private Dictionary<T, J_Mono_Actor<T>> _trackedElements = new Dictionary<T, J_Mono_Actor<T>>();

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            SanityChecks();
            InitThis();
        }

        protected virtual void SanityChecks()
        {
            Assert.IsNotNull(_PrefabActor, $"{gameObject.name} requires a _PrefabActor");
            Assert.IsNotNull(_Collection,  $"{gameObject.name} requires a _Collection");
        }

        protected virtual void InitThis() {}

        // --------------- VIEW UPDATER --------------- //
        protected virtual void OpenThis()
        {
            //make sure all the elements are shown
            for (int i = 0; i < _Collection.Count; i++)
                Add(_Collection[i]);
        }

        protected virtual void CloseThis() {}

        private void Add(T item)
        {
            //some views might be ignored
            if (!WantToShowElement(item)) return;
            // --------------- VIEW CREATION --------------- //
            //Instantiate => updated => track on dictionary
            J_Mono_Actor<T> newUiView = Instantiate(_PrefabActor, transform);
            newUiView.ActorUpdate(item);
            _trackedElements[item] = newUiView;
            //add further adjustments here
            AddedView(item);
        }

        //used to decide if we want to hide some element
        protected virtual bool WantToShowElement(T item) => !_trackedElements.ContainsKey(item);

        //an helper method if we want to apply further elements
        protected virtual void AddedView(T itemAdded) {}

        private void Remove(T itemRemoved)
        {
            RemovedView(itemRemoved);
            Destroy(_trackedElements[itemRemoved].gameObject);
            _trackedElements.Remove(itemRemoved);
        }

        //further adjustments if we want to remove a view
        protected virtual void RemovedView(T itemRemoved) {}

        // --------------- UNITY EVENTS --------------- //
        private void OnEnable()
        {
            OpenThis();
            _Collection.SubscribeToAdd(Add);
            _Collection.SubscribeToRemove(Remove);
        }

        private void OnDisable()
        {
            _Collection.UnSubscribeToAdd(Add);
            _Collection.UnSubscribeToRemove(Remove);
            CloseThis();
        }
    }
}
