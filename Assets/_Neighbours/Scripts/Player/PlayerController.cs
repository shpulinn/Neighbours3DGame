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
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private float playerNormalMoveSpeed;
    [SerializeField] private float playerSlowMoveSpeed;

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

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            var isLeftClick = Input.GetMouseButtonDown(0);
            agent.speed = isLeftClick ? playerNormalMoveSpeed : playerSlowMoveSpeed;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable == null) return;
                float distance = Vector3.Distance(transform.position, hit.point);
                if (distance < interactionDistance) // interact
                {
                    if (interactable is IInventoryInteractable)
                    {
                        _stateMachine.ChangeState(new InventoryInteractState(_stateMachine, (IInventoryInteractable)interactable, _inventory));
                    }
                    else if (interactable is IHidable)
                    {
                        _stateMachine.ChangeState(new HideState(_stateMachine, this));
                    }
                    else
                    {
                        _stateMachine.ChangeState(new InteractState(_stateMachine, interactable));
                    }
                }
                else // approach to interactable object before interacting
                {
                    _stateMachine.ChangeState(new ApproachState(_stateMachine, hit.point, interactable, _inventory, this));
                }
            } else if (Physics.Raycast(ray, out hit, 100, groundLayer))
            {
                _stateMachine.ChangeState(new MoveState(_stateMachine, hit.point));
            }
        }
    }
}


