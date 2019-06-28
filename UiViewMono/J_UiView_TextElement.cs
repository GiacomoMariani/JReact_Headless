using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.UiView
{
    /// <summary>
    /// used to show a text
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class J_UiView_TextElement : MonoBehaviour
    {
         private Lazy<TextMeshProUGUI> _thisText => new Lazy<TextMeshProUGUI>(GetComponent<TextMeshProUGUI>());
         [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private TextMeshProUGUI ThisText => _thisText.Value;

        #region INITIALIZATION
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        protected virtual void InitThis() {}

        protected virtual void SanityChecks() { Assert.IsNotNull(ThisText, "Requires a TextMeshProUGUI: " + gameObject); }
        #endregion

        #region COMMANDS
        //the main element to be implemented
        protected virtual void SetText(string text) { ThisText.text = text; }

        //set the color of the text
        protected virtual void SetColor(Color color) { ThisText.color = color; }
        #endregion
    }
}
