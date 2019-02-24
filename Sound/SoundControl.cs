using UnityEngine;

namespace JReact.Sound
{
    /// <summary>
    /// an abstract class to playe the sound
    /// </summary>
    public abstract class SoundControl : ScriptableObject
    {
        public abstract void PlaySound(string sound);
    }
}

