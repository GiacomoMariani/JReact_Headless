using UnityEngine;
using UnityEngine.Events;

namespace JReact
{
    public abstract class J_ProcessableAction : ScriptableObject, iProcessable
    {
        // --------------- FIELDS AND PROPERTIES --------------- //
        public UnityAction ThisAction => Process;
        public abstract void Process();
    }
}
