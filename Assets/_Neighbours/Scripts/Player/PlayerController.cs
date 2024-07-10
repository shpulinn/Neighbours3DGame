using System.Collections;
using _Neighbours.Scripts;
using _Neighbours.Scripts.Interactables;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerStateMachine _stateMachine;
    private Inventory _inventory;
    
    private void Start()
    {
        _stateMachine = GetComponent<PlayerStateMachine>();
        _inventory = GetComponent<Inventory>();
        _stateMachine.ChangeState(new IdleState(_stateMachine));
    }

    private void Update()
    {
        _stateMachine.Update();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Physics.Raycast(ray, out hit, 100);
            if (Physics.Raycast(ray, out hit, 100, groundLayer))
            {
                _stateMachine.ChangeState(new MoveState(_stateMachine, hit.point));
            } 
            else if (Physics.Raycast(ray, out hit, 100, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    float distance = Vector3.Distance(transform.position, hit.point);
                    if (distance < 2.0f) // Если достаточно близко
                    {
                        if (interactable is IInventoryInteractable)
                        {
                            _stateMachine.ChangeState(new InventoryInteractState(_stateMachine, (IInventoryInteractable)interactable, _inventory));
                        }
                        else
                        {
                            _stateMachine.ChangeState(new InteractState(_stateMachine, interactable));
                        }
                        //_stateMachine.ChangeState(new InteractState(_stateMachine, interactable, _inventory));
                    }
                    else // Если далеко, то сначала подойти
                    {
                        _stateMachine.ChangeState(new ApproachState(_stateMachine, hit.point, interactable, _inventory));
                    }
                }
            }
        }
    }
}


