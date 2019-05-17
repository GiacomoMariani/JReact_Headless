using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// used to display an array and convert into a dictionary with the name of the array element as key and the array value as value
    /// it can be used to reload the elements
    /// </summary>
    /// <typeparam name="TValue">the element to track</typeparam>
    /// <typeparam name="TKey">the key to track the item</typeparam>
    public abstract class J_ItemTracker<TKey, TValue> : J_ReactiveDictionary<TKey, TValue>
    {
        //the array of buildings we want to have
        [BoxGroup("Setup", true, true, 0), SerializeField] private TValue[] _elementsToStore;

        /// <summary>
        /// checks if an element with given id can be found in this retriever
        /// </summary>
        public bool ContainsId(TKey id) => _Dictionary.ContainsKey(id);

        /// <summary>
        /// retrieves the item from the id
        /// </summary>
        /// <param name="id">the id to retrieve</param>
        /// <returns>returns the value requests</returns>
        public TValue GetItemFromId(TKey id)
        {
            Assert.IsTrue(_Dictionary.ContainsKey(id), $"Name Key -{id}- not found in -{name}-");
            return _Dictionary[id];
        }

        /// <summary>
        /// this is the main implementation to get the name from the element
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract TKey GetItemId(TValue item);

        #region DISABLE AND RESET
        protected virtual void OnEnable() { PopulateThis(); }

        protected virtual void PopulateThis()
        {
            //reset this, then add all the required item to the dictionary 
            Clear();
            for (int i = 0; i < _elementsToStore.Length; i++)
                Add(GetItemId(_elementsToStore[i]), _elementsToStore[i]);
        }
        #endregion
    }
}
