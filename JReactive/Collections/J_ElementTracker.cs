using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Collections
{
    /// <summary>
    /// used to display an array and convert into a dictionary with the name of the array element as key and the array value as value
    /// it can be used to reload the elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class J_ElementTracker<T> : J_ReactiveDictionary<string, T>
    {
        //the array of buildings we want to have
        [BoxGroup("Setup", true, true, 0), SerializeField] private T[] _elementsToStore;

        /// <summary>
        /// check if an element with name can be found in this retriever
        /// </summary>
        /// <param name="elementNameKey">the name of the desired element</param>
        /// <returns></returns>
        public bool HasElement(string elementNameKey) { return ThisDictionary.ContainsKey(elementNameKey); }

        /// <summary>
        /// the main method to retrieve the element from its name
        /// </summary>
        /// <param name="elementNameKey">the element want to retrieve</param>
        /// <returns>returns the value requestes</returns>
        public T GetElementFromName(string elementNameKey)
        {
            Assert.IsTrue(ThisDictionary.ContainsKey(elementNameKey), $"Name Key -{elementNameKey}- not found in -{name}-");
            return ThisDictionary[elementNameKey];
        }

        /// <summary>
        /// this is the main implementation to get the name from the element
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected abstract string GetElementName(T p);

        #region DISABLE AND RESET
        //we reset this on disable
        protected virtual void OnEnable() { PopulateThis(); }

        protected virtual void PopulateThis()
        {
            //reset this, then add all the required item to the dictionary 
            ResetThis();
            for (int i = 0; i < _elementsToStore.Length; i++)
                UpdateElement(GetElementName(_elementsToStore[i]), _elementsToStore[i]);
        }
        #endregion
    }
}
