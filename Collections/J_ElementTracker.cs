using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// used to display an array and convert into a dictionary with the name of the array element as key and the array value as value
    /// it can be used to reload the elements
    /// </summary>
    /// <typeparam name="T">the element to track</typeparam>
    public abstract class J_ElementTracker<T> : J_ReactiveDictionary<string, T>
    {
        //the array of buildings we want to have
        [BoxGroup("Setup", true, true, 0), SerializeField] private T[] _elementsToStore;

        /// <summary>
        /// check if an element with name can be found in this retriever
        /// </summary>
        /// <param name="elementNameKey">the name of the desired element</param>
        /// <returns></returns>
        public bool HasItem(string elementNameKey) => _Dictionary.ContainsKey(elementNameKey);

        /// <summary>
        /// the main method to retrieve the element from its name
        /// </summary>
        /// <param name="nameKey">the element want to retrieve</param>
        /// <returns>returns the value requestes</returns>
        public T GetItemFromName(string nameKey)
        {
            Assert.IsTrue(_Dictionary.ContainsKey(nameKey), $"Name Key -{nameKey}- not found in -{name}-");
            return _Dictionary[nameKey];
        }

        /// <summary>
        /// this is the main implementation to get the name from the element
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected abstract string GetItemName(T p);

        #region DISABLE AND RESET
        //we reset this on disable
        protected virtual void OnEnable() { PopulateThis(); }

        protected virtual void PopulateThis()
        {
            //reset this, then add all the required item to the dictionary 
            Clear();
            for (int i = 0; i < _elementsToStore.Length; i++)
                Set(GetItemName(_elementsToStore[i]), _elementsToStore[i]);
        }
        #endregion
    }
}
