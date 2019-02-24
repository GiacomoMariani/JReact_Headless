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
    public abstract class J_Pool<T> : J_Service
        where T : J_PoolItem_Mono<T>
    {
        #region VALUES AND PROPERTIES
        // --------------- STATE --------------- //
        //the prefabs are an array to differentiate them. Also an array of one can be used if we want always the same
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private T[] _prefabItem;
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_TransformGenerator _parentTransform;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _startingItems = 50;
        //set this to true if we want to disable items when they get back to pool
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _disableItemInPool = true;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private T _firstItem;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId = -1;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private JGenericDelegate<T> _initAction;
        #endregion

        #region INITIALIZATION
        /// <summary>
        /// adds an action to be sent to the elements to be initiated
        /// </summary>
        /// <param name="itemSetupAction">the action we want to set for the pool items</param>
        public void AddSetupForItems(JGenericDelegate<T> itemSetupAction) { _initAction = itemSetupAction; }

        /// <inheritdoc />
        /// <summary>
        /// activates the pool
        /// </summary>
        public override void Activate()
        {
            base.Activate();
            // --------------- CHECKS --------------- //
            SanityChecks();
            // --------------- SETUP --------------- //
            _instanceId = GetInstanceID();
            // --------------- START RECURSION --------------- //
            Timing.RunCoroutine(Populate(_startingItems, _initAction), Segment.SlowUpdate, _instanceId,
                                JCoroutineTags.COROUTINE_PoolTag);
        }

        //checks that everything has been setup properly
        private void SanityChecks()
        {
            Assert.IsNotNull(_parentTransform, $"{name} requires an element for _parentTransform");
            Assert.IsNotNull(_prefabItem, $"{name} requires an element for _prefab");
            Assert.IsTrue(_startingItems > 0, $"{name} requires a positive number for the pool items");
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
                                    JCoroutineTags.COROUTINE_PoolTag);
        }
        #endregion

        #region PRIVATE COMMANDS
        private T AddItemIntoPool()
        {
            var poolItem = Instantiate(_prefabItem.GetRandomElement(), _parentTransform.ThisTransform);
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
            itemToPool.ConnectWithPool(_firstItem);
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
            if (!IsActive) Activate();

            //check if the first element in the pool is missing, otherwise add one
            if (_firstItem == null) AddItemIntoPool();

            //update the elements and return the next one 
            var element = _firstItem;
            _firstItem = element.NextItemInPool;
            element.GetFromPool();
            return element;
        }
        #endregion
    }
}
