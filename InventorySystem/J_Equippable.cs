using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.InventorySystem
{
    /// <summary>
    /// an item that might be equippable
    /// </summary>
    [CreateAssetMenu(menuName = "Reactive/Equipment/Item", fileName = "Item")]
    public class J_Equippable : ScriptableObject
    {
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _id;
        public int Id => _id;
        [BoxGroup("Setup", true, true, 0), SerializeField] private string _name;
        public string NameOfThis => _name;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _weight = 1;
        public int Weight => _weight;
        [BoxGroup("Setup", true, true, 0), SerializeField] private int _price = 1;
        public int Price => _price;
        [BoxGroup("Setup", true, true, 0), SerializeField] private Sprite _view;
        public Sprite View => _view;

        // --------------- STATE --------------- //
        [BoxGroup("Setup", true, true, 0), SerializeField] private J_EquipmentCategory category;
        public J_EquipmentCategory Category => category;
    }
}
