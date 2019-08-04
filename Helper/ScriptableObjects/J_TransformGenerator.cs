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
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly] private J_TransformGenerator _parent;

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
        
        // --------------- CREATION --------------- //
        public static J_TransformGenerator CreateTransformGenerator(string nameToSet, J_TransformGenerator parent = null)
        {
            var generator = CreateInstance<J_TransformGenerator>();
            generator.name = nameToSet;
            generator._parent = parent;
            return generator;
        }
        

        //creates the transform when missing
        private Transform GenerateNewTransform()
        {
            // --------------- CHECKS --------------- //
            if (!AllParentsValid()) return null;

            // --------------- CREATION WITH NAME --------------- //
            Transform transformToSpawn = new GameObject(name).transform;

            // --------------- PARENTING --------------- //
            if (_parent != null) transformToSpawn.SetParent(_parent.ThisTransform);

            //COMPLETE
            return transformToSpawn;
        }

        //used to avoid a circular parenting
        private bool AllParentsValid()
        {
            J_TransformGenerator currentCheck = _parent;
            while (currentCheck != null)
            {
                if (currentCheck != this)
                {
                    currentCheck = currentCheck._parent;
                    continue;
                }

                JLog.Error($"{name} has a circular parent.", JLogTags.SceneView, this);
                return false;
            }

            return true;
        }
    }
}
