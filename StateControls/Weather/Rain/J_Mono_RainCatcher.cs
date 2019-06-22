using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.StateControl.Weather
{
    /// <summary>
    /// spawns particles on specific states
    /// </summary>
    public sealed class J_Mono_RainCatcher : J_Mono_StateElement
    {
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private ParticleSystem _rainParticles;

        protected override void EnterState() { _rainParticles.Play(); }

        protected override void ExitState()
        {
            if (!Application.isPlaying) return;
            _rainParticles.Stop();
        }
    }
}
