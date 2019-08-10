using System;
using System.Collections.Generic;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pool
{
    /// <summary>
    /// implements a pool of monobehaviours
    /// like explained http://www.gameprogrammingpatterns.com/object-pool.html
    /// </summary>
    public abstract class J_Pool<T> : J_Service
        where T : J_PoolItem_Mono<T>
    {
        // --------------- SETUP --------------- //
        //the prefabs are an array to differentiate them. Also an array of one can be used if we want always the same
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly, Required] private T[] _prefabVariations;
        [BoxGroup("Setup", true, true), SerializeField, AssetsOnly] private J_TransformGenerator _parentTransform;
        [BoxGroup("Setup", true, true), SerializeField] private int _startPopulation = 50;
        //set this to true if we want to disable items when they get back to pool
        [BoxGroup("Setup", true, true), SerializeField] private bool _disableItemInPool = true;
        [BoxGroup("Setup", true, true), SerializeField] private bool _instantPopulation;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Stack<T> _poolStack;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private int _instanceId = -1;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Action<T> _initAction;

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int Count => _poolStack.Count;

        // --------------- CREATION --------------- //
        public void SetupPool(T[]  prefabs, int population, Action<T> itemSetupAction = null,
                              bool instantPopulation = false)
        {
            if (IsActive) End();
            _prefabVariations  = prefabs;
            _startPopulation   = population;
            _parentTransform   = J_TransformGenerator.CreateTransformGenerator("Instantiated");
            _instantPopulation = instantPopulation;
            _initAction        = itemSetupAction;
        }

        /// <summary>
        /// adds an action to be sent to the elements to be initiated
        /// </summary>
        /// <param name="itemSetupAction">the action we want to set for the pool items</param>
        public void AddSetupForItems(Action<T> itemSetupAction) { _initAction = itemSetupAction; }

        // --------------- INITIALIZATION --------------- //
        protected override void ActivateThis()
        {
            base.ActivateThis();
            // --------------- CHECKS --------------- //
            if (!SanityChecks()) return;
            // --------------- SETUP --------------- //
            _instanceId = GetInstanceID();
            _poolStack  = new Stack<T>(_startPopulation);
            // --------------- START RECURSION --------------- //
            Populate(_startPopulation);
        }

        //checks that everything has been setup properly
        private bool SanityChecks()
        {
            Assert.IsNotNull(_parentTransform,  $"{name} requires an element for _parentTransform");
            Assert.IsNotNull(_prefabVariations, $"{name} requires an element for _prefab");
            if (_startPopulation <= 0)
            {
                JLog.Error($"{name} {nameof(_startPopulation)} ({_startPopulation}) needs to be positive. Cancel activation.",
                           JLogTags.Pool, this);

                return false;
            }

            return true;
        }

        private void Populate(int remainingObjects)
        {
            // --------------- ITEM CREATION --------------- //
            T itemToAdd = AddItemIntoPool();
            _initAction?.Invoke(itemToAdd);

            // --------------- CHECK --------------- //
            //check is set at the end to avoid a unnecessary coroutine
            remainingObjects--;
            if (remainingObjects <= 0) return;

            // --------------- MOVE NEXT --------------- //
            if (_instantPopulation) Populate(remainingObjects);
            else Timing.RunCoroutine(WaitAndPopulate(remainingObjects), Segment.SlowUpdate, _instanceId, JCoroutineTags.PoolTag);
        }

        private IEnumerator<float> WaitAndPopulate(int remainingObjects)
        {
            yield return Timing.WaitForOneFrame;
            Populate(remainingObjects);
        }

        // --------------- COMMANDS --------------- //
        /// <summary>
        /// gets an element from the pool
        /// creates a new element if there are no available
        /// </summary>
        /// <returns>an item taken the pool</returns>
        public T GetItemFromPool()
        {
            //initialize, if required
            if (!IsActive) Activate();

            //check if the first element in the pool is missing, otherwise add one
            if (_poolStack.Count == 0) AddItemIntoPool();

            //update the elements and return the next one 
            T element = _poolStack.Pop();
            element.GetFromPool();
            return element;
        }

        public T PeekItemInPool()
        {
            if (!IsActive) Activate();
            return _poolStack.Peek();
        }

        //sets the item at the end of the pool
        internal void PlaceInPool(T itemToPool)
        {
            //disable the item if requested
            if (_disableItemInPool && itemToPool.gameObject.activeSelf) itemToPool.gameObject.SetActive(false);
            _poolStack.Push(itemToPool);
        }

        private T AddItemIntoPool()
        {
            T poolItem = Instantiate(_prefabVariations.GetRandomElement(), _parentTransform.ThisTransform);
            poolItem.InjectPoolOwner(this);
            PlaceInPool(poolItem);
            return poolItem;
        }
    }
}
