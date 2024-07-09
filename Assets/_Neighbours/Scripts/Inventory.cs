using System.Collections.Generic;
using UnityEngine;

namespace _Neighbours.Scripts
{
    public class Inventory : MonoBehaviour
    {
        private List<Item> items = new List<Item>();

        public bool HasItem(Item item)
        {
            return items.Contains(item);
        }

        public void AddItem(Item item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
            }
        }

        public void RemoveItem(Item item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
            }
        }
    }
}