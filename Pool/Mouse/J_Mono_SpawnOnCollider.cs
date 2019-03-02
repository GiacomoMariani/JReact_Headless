using JReact.Pool.SpecialEffect;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JReact
{
    /// <summary>
    /// spawn effects when pointer is on a gameobject
    /// </summary>
    public class J_Mono_SpawnOnCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region FIELDS AND VALUES
        [BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required] private J_Pool_SpecialEffects _effectsPool;
        [BoxGroup("Setup", true, true, 0), SerializeField, Required] private Camera _mainCamera;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _spawnOnEnter;
        [BoxGroup("Setup", true, true, 0), SerializeField] private bool _spawnOnExit;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Quaternion _particleRotation;
        #endregion

        #region EVENT STARTERS
        private void EnterArea()
        {
            if (_spawnOnEnter) _effectsPool.TriggerEffectOnPosition(GetMousePosition(), _particleRotation);
        }

        private void ExitArea()
        {
            if (_spawnOnExit) _effectsPool.TriggerEffectOnPosition(GetMousePosition(), _particleRotation);
        }
        #endregion

        #region HELPER
        //this method is used to get the mouse current position
        private Vector3 GetMousePosition() => _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        #endregion

        #region PARTICLE SPAWNERS
        #endregion

        #region INTERFACE IMPLEMENTATION
        //when mouse enter and exit the collider
        public void OnPointerEnter(PointerEventData eventData) { EnterArea(); }
        public void OnPointerExit(PointerEventData eventData) { ExitArea(); }
        #endregion
    }
}
