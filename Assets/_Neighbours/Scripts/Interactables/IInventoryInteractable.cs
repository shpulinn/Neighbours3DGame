namespace _Neighbours.Scripts.Interactables
{
    public interface IInventoryInteractable : IInteractable
    {
        public void InteractWithAnItem(Inventory inventory);
    }
}