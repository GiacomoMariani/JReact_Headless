using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// used to get an element and use it as reference
    /// </summary>
    /// <typeparam name="T">the element we want to get</typeparam>
    public abstract class J_ReferenceInject<T> : ScriptableObject
        where T : class
    {
        #region FIELDS AND PROPERTIES
        //the element as reference
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected T _thisElement;
        public virtual T ThisElement
        {
            get => _thisElement ?? (_thisElement = RetrieveElement());
            private set => _thisElement = value;
        }
        #endregion

        #region RETRIEVERS
        //the main retriever for this element
        protected abstract T RetrieveElement();
        #endregion

        #region INJECTORS
        /// <summary>
        /// used to inject the element
        /// </summary>
        /// <param name="elementToInject">the element to inject</param>
        public void InjectReference(T elementToInject) { ThisElement = elementToInject; }
        #endregion
    }
}
