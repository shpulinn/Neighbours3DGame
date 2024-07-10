using UnityEngine;

namespace _Neighbours.Scripts
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    public class Item : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public bool isStackable;
    }

}