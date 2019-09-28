using System.Linq;
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
    public abstract class J_ItemRetriever<TKey, TValue> : J_ReactiveDictionary<TKey, TValue>
    {
        //the array of buildings we want to have
        [BoxGroup("Setup", true, true), SerializeField] protected TValue[] _items;

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
            if (_Dictionary       == null ||
                _Dictionary.Count != _items.Length) PopulateThis();

            Assert.IsTrue(_Dictionary.ContainsKey(id), $"Name Key -{id}- not found in -{name}-");
            return _Dictionary[id];
        }

        /// <summary>
        /// add an item to the list, also at runtime,  low performance method (uses linq)
        /// </summary>
        public void InjectNewElement(TValue item) => _items = _items.AddItemToArray(item);

        /// <summary>
        /// this is the main implementation to get the name from the element
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract TKey GetItemId(TValue item);

        // --------------- DISABLE AND RESET --------------- //
        protected virtual void OnEnable() => PopulateThis();

        [BoxGroup("Commands", true, true, 100), Button(ButtonSizes.Medium)]
        public virtual void PopulateThis()
        {
            //reset this, then add all the required item to the dictionary 
            Clear();
            for (int i = 0; i < _items.Length; i++) Add(GetItemId(_items[i]), _items[i]);
        }
    }
}
