using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact
{
    /// <summary>
    /// this is something to activate a number of views on the scene
    /// </summary>
    public class J_Mono_ViewActivator : MonoBehaviour
    {
        #region FIELDS AND PROPERTIES
        public event JActivationDelegate OnActivation;

        //the views we want to activate
        [BoxGroup("Views", true, true, -50), SerializeField, Required] private GameObject[] _views;
        //to decide if we want to start them as active
        [BoxGroup("Views", true, true, -50), SerializeField] protected bool _startsActive;
        #endregion

        #region INITIALIZATION
        //used for initialization
        private void Awake()
        {
            //check that everything is as expected
            SanityChecks();
            //setup components and other elements
            InitThis();
        }

        //used to check that every element is valid
        protected virtual void SanityChecks() { Assert.IsTrue(_views.Length > 0, $"{gameObject.name} requires at least one view"); }

        //used to initialize this element
        protected virtual void InitThis() { ActivateView(_startsActive); }
        #endregion

        #region ACTIVATION
        //used to activate the views
        public void ActivateView(bool activateView)
        {
            for (int i = 0; i < _views.Length; i++)
            {
                if (_views[i] == null) continue;
                ActivateSpecificView(_views[i], activateView);
            }

            if (OnActivation != null) OnActivation(activateView);
        }

        //this is used to activate a specific view
        private void ActivateSpecificView(GameObject viewToActivate, bool activeNow)
        {
            viewToActivate.SetActive(activeNow);
            if (activeNow) ActivateThis(viewToActivate);
            else DeActivateThis(viewToActivate);
        }
        #endregion

        #region TEMPLATES
        //if we want to add further actions to the view
        protected virtual void ActivateThis(GameObject viewToActivate) {}
        protected virtual void DeActivateThis(GameObject viewToActivate) {}
        #endregion
    }
}
