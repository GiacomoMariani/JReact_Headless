using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pool
{
    /// <summary>
    /// a monobehaviour that might be reused multiple times. it can be attached as a singe component
    /// </summary>
    /// <typeparam name="T">a reference to itself to make sure it is recognized by the poll</typeparam>
    public class J_PoolItem_Mono<T> : MonoBehaviour
        where T : J_PoolItem_Mono<T>
    {
        #region VALUES AND PROPERTIES
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _returnInPoolAtDisable = true;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] internal T NextItemInPool { get; private set; }
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Pool<T> _poolOwner;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private bool _inPool;
        #endregion

        #region POOL METHODS
        internal void InjectPoolOwner(J_Pool<T> owner) { _poolOwner = owner; }

        internal virtual void GetFromPool()
        {
            Assert.IsTrue(_inPool, $"{gameObject.name} was not in the pool.");
            _inPool = false;
        }

        internal virtual void ReturnToPool()
        {
            Assert.IsFalse(_inPool, $"{gameObject.name} seems to be in the pool already. Cancel command.");
            if (!_inPool) _poolOwner.PlaceInPool((T) this);
        }

        internal void ConnectWithPool(T next)
        {
            NextItemInPool = next;
            _inPool        = true;
        }
        #endregion

        #region UNITY EVENTS
        protected virtual void OnDisable()
        {
            if (_returnInPoolAtDisable && !_inPool) ReturnToPool();
        }
        #endregion
    }
}
