using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using UnityEngine;

namespace JReact.Pool
{
    /// <summary>
    /// implements a pool of monobehaviours
    /// like explained http://www.gameprogrammingpatterns.com/object-pool.html
    /// </summary>
    public abstract class J_Pool<T> : ScriptableObject
        where T : J_PoolItem_Mono<T>
    {
        #region VALUES AND PROPERTIES
        // --------------- STATE --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private T _prefabItem;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_TransformGenerator _parentTransform;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _startingItems = 50;
        //set this to true if we want to disable items when they get back to pool
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _disableItemInPool = true;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T _firstItem;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId = -1;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _initializationComplete = false;
        #endregion

        #region INITIALIZATION
        /// <summary>
        /// initialize the pool
        /// </summary>
        /// <param name="itemSetupAction">an action to perform to the items during creation</param>
        public void InitPool(JGenericDelegate<T> itemSetupAction = null)
        {
            // --------------- CHECKS --------------- //
            SanityChecks();
            if (_initializationComplete) return;
            // --------------- SETUP --------------- //
            _initializationComplete = true;
            _instanceId             = GetInstanceID();
            // --------------- START RECURSION --------------- //
            Timing.RunCoroutine(Populate(_startingItems, itemSetupAction), Segment.SlowUpdate, _instanceId,
                                J_CoroutineTags.COROUTINE_PoolTag);
        }

        //checks that everything has been setup properly
        private void SanityChecks()
        {
            Assert.IsNotNull(_parentTransform, $"{name} requires an element for _parentTransform");
            Assert.IsNotNull(_prefabItem, $"{name} requires an element for _prefab");
            Assert.IsTrue(_startingItems > 0, $"{name} requires a positive number for the pool items");
            Assert.IsFalse(_initializationComplete, $"The pool {name} made of {typeof(T)} has already been initialized");
        }

        //populates the pool
        private IEnumerator<float> Populate(int remainingObjects, JGenericDelegate<T> itemSetupAction)
        {
            // --------------- ITEM CREATION --------------- //
            var itemToAdd = AddItemIntoPool();
            if (itemSetupAction != null) itemSetupAction(itemToAdd);
            yield return Timing.WaitForOneFrame;

            // --------------- MOVE NEXT --------------- //
            remainingObjects--;
            if (remainingObjects > 0)
                Timing.RunCoroutine(Populate(remainingObjects, itemSetupAction), Segment.SlowUpdate, _instanceId,
                                    J_CoroutineTags.COROUTINE_PoolTag);
        }
        #endregion

        #region PRIVATE COMMANDS
        private T AddItemIntoPool()
        {
            var poolItem = Instantiate(_prefabItem, _parentTransform.ThisTransform);
            poolItem.InjectPoolOwner(this);
            PlaceInPool(poolItem);
            return poolItem;
        }
        #endregion

        #region COMMANDS
        //sets the item at the end of the pool
        internal void PlaceInPool(T itemToPool)
        {
            //disable the item if requested
            if (_disableItemInPool && itemToPool.gameObject.activeSelf) itemToPool.gameObject.SetActive(false);

            //replace the first item
            itemToPool.SetNext(_firstItem);
            _firstItem = itemToPool;

            //a sanity check to avoid a common bug
            Assert.IsFalse(_firstItem == _firstItem.NextItemInPool,
                           $"{name} first item {_firstItem.gameObject.name} points to itself");
        }

        /// <summary>
        /// gets an element from the pool
        /// creates a new element if there are no available
        /// </summary>
        /// <returns>an item taken the pool</returns>
        public T GetElementFromPool()
        {
            //initialize, if required
            if (!_initializationComplete) InitPool();

            //check if the first element in the pool is missing, otherwise add one
            if (_firstItem == null) AddItemIntoPool();

            //update the elements and return the next one 
            var element = _firstItem;
            _firstItem = element.NextItemInPool;
            return element;
        }
        #endregion

        #region DISABLE AND RESET
        protected virtual void OnDisable() { ResetThis(); }
        private void ResetThis() { _initializationComplete = false; }
        #endregion
    }
}
