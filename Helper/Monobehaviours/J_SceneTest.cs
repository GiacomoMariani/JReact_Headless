using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact
{
    /// <summary>
    /// implements test in a scene
    /// </summary>
    public class J_SceneTest : MonoBehaviour
    {
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private iTestable[] _testables;

        [Button(ButtonSizes.Large)]
        public void RunAllTests()
        {
            _testables = GetComponentsInChildren<iTestable>();
            foreach (iTestable test in _testables)
                test.RunTest();
        }

        [Button(ButtonSizes.Large)]
        public void StopAllTests()
        {
            _testables = GetComponentsInChildren<iTestable>();
            foreach (iTestable test in _testables)
                test.StopTest();
        }
    }
}
