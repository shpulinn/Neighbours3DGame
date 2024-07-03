using System.Collections;
using _Neighbours.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerStateMachine _stateMachine;

    private void Start()
    {
        _stateMachine = GetComponent<PlayerStateMachine>();
        _stateMachine.ChangeState(new IdleState(_stateMachine));
    }

    private void Update()
    {
        _stateMachine.Update();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, groundLayer))
            {
                _stateMachine.ChangeState(new MoveState(_stateMachine, hit.point));
            }

            if (Physics.Raycast(ray, out hit, 100, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    float distance = Vector3.Distance(transform.position, hit.point);
                    if (distance < 1.0f) // Если достаточно близко
                    {
                        _stateMachine.ChangeState(new InteractState(_stateMachine, interactable));
                    }
                    else // Если далеко, то сначала подойти
                    {
                        _stateMachine.ChangeState(new ApproachState(_stateMachine, hit.point, interactable));
                    }
                }
            }
        }
    }

    private void FindAndUseDoor(Vector3 destination)
    {
        Collider[] doors = Physics.OverlapSphere(transform.position, 10, doorLayer);

        if (doors.Length > 0)
        {
            Collider nearestDoor = doors[0];
            float nearestDistance = Vector3.Distance(transform.position, doors[0].transform.position);

            foreach (Collider door in doors)
            {
                float distance = Vector3.Distance(transform.position, door.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDoor = door;
                    nearestDistance = distance;
                }
            }

            Door doorScript = nearestDoor.GetComponent<Door>();
            doorScript.OpenDoor();
            agent.SetDestination(nearestDoor.transform.position);
            StartCoroutine(CloseDoorAfterUse(doorScript, destination));
        }
    }

    private IEnumerator CloseDoorAfterUse(Door door, Vector3 destination)
    {
        yield return new WaitForSeconds(1);
        door.CloseDoor();
        agent.SetDestination(destination);
    }
}


