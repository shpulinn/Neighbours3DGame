using _Neighbours.Scripts;

public interface IInteractable
{
    public void Interact();
    public void TerminateInteraction();
    public float InteractionDuration { get; }

    public float InteractionProgress { get; }
}
