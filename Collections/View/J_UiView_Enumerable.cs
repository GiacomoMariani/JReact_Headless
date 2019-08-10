using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections.UiView
{
    public abstract class J_UiView_Enumerable<T> : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected abstract iReactiveEnumerable<T> _Enumerable { get; }
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
            Assert.IsNotNull(_PrefabActor, $"{gameObject.name} requires a {nameof(_PrefabActor)}");
            Assert.IsNotNull(_Enumerable,  $"{gameObject.name} requires a {nameof(_Enumerable)}");
        }

        protected virtual void InitThis() {}

        // --------------- VIEW UPDATER --------------- //
        protected virtual void OpenThis()
        {
            foreach (T t in _Enumerable) Add(t);
        }

        protected virtual void CloseThis() {}

        // --------------- ADD --------------- //
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
            AddedView(item, newUiView);
        }

        //used to decide if we want to hide some element
        protected virtual bool WantToShowElement(T item) => !_trackedElements.ContainsKey(item);

        //an helper method if we want to apply further elements
        protected virtual void AddedView(T itemAdded, J_Mono_Actor<T> newUiView) {}

        // --------------- REMOVE --------------- //
        private void Remove(T itemRemoved)
        {
            RemovedView(itemRemoved, _trackedElements[itemRemoved]);
            Destroy(_trackedElements[itemRemoved].gameObject);
            _trackedElements.Remove(itemRemoved);
        }

        //further adjustments if we want to remove a view
        protected virtual void RemovedView(T itemRemoved, J_Mono_Actor<T> newUiView) {}

        // --------------- UNITY EVENTS --------------- //
        private void OnEnable()
        {
            OpenThis();
            _Enumerable.SubscribeToAdd(Add);
            _Enumerable.SubscribeToRemove(Remove);
        }

        private void OnDisable()
        {
            _Enumerable.UnSubscribeToAdd(Add);
            _Enumerable.UnSubscribeToRemove(Remove);
            CloseThis();
        }
    }
}
