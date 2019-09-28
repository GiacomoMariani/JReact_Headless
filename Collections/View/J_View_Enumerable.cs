using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections.View
{
    public abstract class J_View_Enumerable<T> : MonoBehaviour
    {
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
        protected virtual void Open()
        {
            foreach (T t in _Enumerable) UpdateView(t);
        }

        protected virtual void Close() {}

        // --------------- ADD / UPDATE --------------- //
        private void UpdateView(T item)
        {
            //some views might be ignored
            if (!WantToShowElement(item)) return;
            // --------------- VIEW CREATION --------------- //
            //get or instantiate
            J_Mono_Actor<T> view = _trackedElements.ContainsKey(item)
                                       ? _trackedElements[item]
                                       : Instantiate(_PrefabActor, transform);

            //update
            view.ActorUpdate(item);
            //track if required
            if (_trackedElements.ContainsKey(item)) return;
            _trackedElements[item] = view;
            AddedView(item, view);
        }

        //used to decide if we want to hide some element
        protected virtual bool WantToShowElement(T item) => true;

        //an helper method if we want to apply further elements
        protected virtual void AddedView(T item, J_Mono_Actor<T> view) {}

        // --------------- REMOVE --------------- //
        private void Remove(T itemRemoved)
        {
            RemovedView(itemRemoved, _trackedElements[itemRemoved]);
            Destroy(_trackedElements[itemRemoved].gameObject);
            _trackedElements.Remove(itemRemoved);
        }

        //further adjustments if we want to remove a view
        protected virtual void RemovedView(T item, J_Mono_Actor<T> view) {}

        // --------------- UNITY EVENTS --------------- //
        private void OnEnable()
        {
            Open();
            _Enumerable.SubscribeToAdd(UpdateView);
            _Enumerable.SubscribeToRemove(Remove);
        }

        private void OnDisable()
        {
            _Enumerable.UnSubscribeToAdd(UpdateView);
            _Enumerable.UnSubscribeToRemove(Remove);
            Close();
        }
    }
}
