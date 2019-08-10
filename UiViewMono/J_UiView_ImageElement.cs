using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace JReact.UiView
{
    /// <summary>
    /// this class is used to show an image
    /// </summary>
    [RequireComponent(typeof(Image))]
    public abstract class J_UiView_ImageElement : MonoBehaviour
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        //reference to the text
        private Image _thisImage;
        [BoxGroup("Base", true, true, -5), ReadOnly] protected Image ThisImage
        {
            get
            {
                if (_thisImage == null) _thisImage = GetComponent<Image>();
                return _thisImage;
            }
        }

        //a reference to activate and deactivate the image
        private bool _isActive;
        [BoxGroup("Base", true, true, -5), ReadOnly] public bool IsActive
        {
            get => _isActive;
            protected set
            {
                _isActive         = value;
                ThisImage.enabled = value;
            }
        }

        // --------------- INITIALIZATION --------------- //
        private void Awake()
        {
            InitThis();
            SanityChecks();
        }

        protected virtual void InitThis()     {}
        protected virtual void SanityChecks() { Assert.IsNotNull(ThisImage, "Requires a TextMeshProUGUI: " + gameObject); }

        // --------------- COMMANDS --------------- //
        //sets the sprite on the image
        protected virtual void SetImage(Sprite image) { ThisImage.sprite = image; }
    }
}
