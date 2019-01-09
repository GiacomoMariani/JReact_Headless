using Sirenix.OdinInspector;
using UnityEngine;

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
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] internal T NextItemInPool { get; private set; }
        [BoxGroup("State", true, true, 5), ReadOnly, ShowInInspector] private J_Pool<T> _poolOwner;
        #endregion

        #region POOL METHODS
        internal void InjectPoolOwner(J_Pool<T> owner) { _poolOwner = owner; }
        internal void SetNext(T next) { NextItemInPool              = next; }
        protected void ReturnToPool() { _poolOwner.PlaceInPool((T) this); }
        #endregion

        #region UNITY EVENTS
        protected virtual void OnDisable()
        {
            if (_returnInPoolAtDisable) ReturnToPool();
        }
        #endregion
    }
}
