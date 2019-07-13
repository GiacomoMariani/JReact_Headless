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
        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private TextMeshProUGUI _text;
        private TextMeshProUGUI ThisText
        {
            get
            {
                if (_text == null) _text = GetComponent<TextMeshProUGUI>();
                return _text;
            }
        }

        // --------------- INIT --------------- //
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        protected virtual void InitThis() {}

        protected virtual void SanityChecks() { Assert.IsNotNull(ThisText, $"{gameObject.name} requires a {nameof(ThisText)}"); }

        // --------------- COMMANDS --------------- //
        protected virtual void SetText(string text) { ThisText.text = text; }

        protected virtual void SetColor(Color color) { ThisText.color = color; }
    }
}
