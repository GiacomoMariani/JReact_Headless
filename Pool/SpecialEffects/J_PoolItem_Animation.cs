using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pool.SpecialEffect
{
    /// <summary>
    /// animation effect implemented as pool item
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class J_PoolItem_Animation : J_PoolItem_SpecialEffect
    {
        #region FIELDS AND PROPERTIES
        // --------------- SETUP --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private float _animationLength;
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _animatorTrigger;

        // --------------- STATE --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Animator _thisAnimator;
        private Animator _ThisAnimator
        {
            get
            {
                if (_thisAnimator == null) _thisAnimator = GetComponent<Animator>();
                return _thisAnimator;
            }
        }
        #endregion

        #region INITIALIZATION
        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_ThisAnimator, $"{gameObject.name} requires an animator ");
        }
        #endregion

        #region IMPLEMENTATION
        protected override void TriggerThisEffect()
        {
            _ThisAnimator.SetTrigger(_animatorTrigger);
            RemoveAfterSeconds(_animationLength);
        }
        #endregion
    }
}
