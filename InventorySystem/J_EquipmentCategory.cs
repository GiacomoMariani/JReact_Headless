using JReact.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JReact.InventorySystem
{
    /// <summary>
    /// a part of the avatar
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Equipment/Category", fileName = "EquipmentCategory")]
    public sealed class J_EquipmentCategory : J_ReactiveArray<J_Equippable>
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] private byte _id;
        public byte Id => _id;

        [InfoBox("NULL => default is null"), BoxGroup("Setup", true, true, 0), SerializeField, AssetsOnly, Required]
        private J_Equippable _default;
        public J_Equippable GetBaseItem() => _default;
    }
}
