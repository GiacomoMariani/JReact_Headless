using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// helper to create a transform
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Basics/Transform Creator", fileName = "Transformis")]
    public sealed class J_TransformGenerator : ScriptableObject
    {
        #region FIELDS AND PROPERTIES
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_TransformGenerator Parent;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Transform _thisTransform;
        public Transform ThisTransform
        {
            get
            {
                if (_thisTransform == null) _thisTransform = GenerateNewTransform();
                return _thisTransform;
            }
            private set => _thisTransform = value;
        }
        #endregion

        //creates the transform when missing
        private Transform GenerateNewTransform()
        {
            // --------------- CHECKS --------------- //
            if (!AllParentsValid()) return null;

            // --------------- CREATION WITH NAME --------------- //
            Transform transformToSpawn = new GameObject(name).transform;

            // --------------- PARENTING --------------- //
            if (Parent != null) transformToSpawn.SetParent(Parent.ThisTransform);

            //COMPLETE
            return transformToSpawn;
        }

        //used to avoid a circular parenting
        private bool AllParentsValid()
        {
            J_TransformGenerator currentCheck = Parent;
            while (currentCheck != null)
            {
                if (currentCheck != this)
                {
                    currentCheck = currentCheck.Parent;
                    continue;
                }

                JLog.Error($"{name} has a circular parent.", JLogTags.SceneView, this);
                return false;
            }

            return true;
        }
    }
}
