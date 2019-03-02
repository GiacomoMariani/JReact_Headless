using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Pool.SpecialEffect
{
    /// <summary>
    /// particles effect implemented as pool item
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class J_PoolItem_Particles : J_PoolItem_SpecialEffect
    {
        #region FIELDS AND PROPERTIES
        #endregion
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private ParticleSystem _particles;
        private ParticleSystem _ThisParticles
        {
            get
            {
                if (_particles == null) _particles = GetComponent<ParticleSystem>();
                return _particles;
            }
        }

        #region INITIALIZATION
        protected override void SanityChecks()
        {
            base.SanityChecks();
            Assert.IsNotNull(_ThisParticles, $"{gameObject.name} requires a particle effect");
        }
        #endregion

        #region IMPLEMENTATION
        protected override void TriggerThisEffect()
        {
            _ThisParticles.Play();
            RemoveAfterSeconds(_ThisParticles.main.duration);
        }
        #endregion
    }
}
