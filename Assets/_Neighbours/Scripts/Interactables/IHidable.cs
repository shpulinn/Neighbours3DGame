namespace _Neighbours.Scripts.Interactables
{
    public interface IHidable : IInteractable
    {
        public void HideInteraction(PlayerController playerController);
    }
}