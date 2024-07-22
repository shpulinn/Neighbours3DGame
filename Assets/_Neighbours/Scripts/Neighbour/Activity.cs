using UnityEngine;

public class Activity : MonoBehaviour
{
    [SerializeField] private string activityName;
    [SerializeField] private Vector3 position;
    [SerializeField] private float duration;
    /* todo:
        animation to play?
        sound to play?
        connected objects to interact with?
        information about corrupted state of activity? 
        item to use to make interaction?
     */

    public string Name => activityName;
    public Vector3 Position => transform.position;
    public float Duration => duration;

}
