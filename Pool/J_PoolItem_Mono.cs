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
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true), SerializeField] private bool _returnInPoolAtDisable = true;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private J_Pool<T> _poolOwner;
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public bool InPool { get; private set; }

        // --------------- POOL METHODS --------------- //
        internal void InjectPoolOwner(J_Pool<T> owner)
        {
            _poolOwner = owner;
            InPool     = true;
        }

        internal virtual void GetFromPool()
        {
            Assert.IsTrue(InPool, $"{gameObject.name} was not in the pool.");
            InPool = false;
        }

        protected virtual void ReturnToPool()
        {
            if (InPool)
            {
                JLog.Warning($"{gameObject.name} seems to be in the pool already. Cancel command.");
                return;
            }

            if (!InPool) _poolOwner.PlaceInPool((T) this);
            InPool = true;
        }

        // --------------- UNITY EVENTS --------------- //
        protected virtual void OnDisable()
        {
            if (_returnInPoolAtDisable && !InPool) ReturnToPool();
        }
    }
}
