using JReact.Collections;
using UnityEngine;

namespace JReact.InventorySystem
{
    [CreateAssetMenu(menuName = "Reactive/Equipment/Inventory", fileName = "Inventory")]
    public class J_Inventory : J_ReactiveList<J_Equippable>
    {
    }
}
