using _Neighbours.Scripts.Interactables;
using _Neighbours.Scripts.States;
using UnityEngine;

namespace _Neighbours.Scripts
{
    public class PlayerStateMachine : StateMachine
    {
        [SerializeField] private bool logCurrentState = false;
        
        private State _currentState;

        public void ChangeState(State newState)
        {
            if (_currentState != null)
            {
                _currentState.Exit();
            }
            _currentState = newState;
            _currentState.Enter();
        }

        public void Update()
        {
            if (_currentState != null)
            {
                if (logCurrentState)
                {
                    Debug.Log(_currentState);
                }
                _currentState.Execute();
            }
        }
    }

    public class IdleState : State
    {
        private PlayerStateMachine _player;

        public IdleState(PlayerStateMachine player)
        {
            _player = player;
        }

        public override void Enter() { }
        public override void Execute()
        {
            // Логика для состояния покоя
        }
        public override void Exit() { }
    }

    public class MoveState : State
    {
        private PlayerStateMachine _player;
        private Vector3 _destination;

        public MoveState(PlayerStateMachine player, Vector3 destination)
        {
            _player = player;
            _destination = destination;
        }

        public override void Enter()
        {
            _player.Agent.SetDestination(_destination);
        }
        public override void Execute()
        {
            var playerTransformPos = _player.transform.position;
            var playerPosWithoutY = new Vector3(playerTransformPos.x, 0, playerTransformPos.z);
            if (Vector3.Distance(playerPosWithoutY, _destination) < 0.6f)
            {
                _player.ChangeState(new IdleState(_player));
            }
        }
        public override void Exit() { }
    }
    
    public class ApproachState : State
    {
        private PlayerStateMachine _player;
        private Vector3 _targetPosition;
        private IInteractable _interactable;
        private IInventoryInteractable _inventoryInteractable;
        private Inventory _inventory;

        public ApproachState(PlayerStateMachine player, Vector3 targetPosition, IInteractable interactable, Inventory inventory)
        {
            _player = player;
            _targetPosition = targetPosition;
            _interactable = interactable;
            _inventoryInteractable = interactable as IInventoryInteractable;
            _inventory = inventory;
        }

        public override void Enter()
        {
            _player.Agent.SetDestination(_targetPosition);
        }

        public override void Execute()
        {
            if (Vector3.Distance(_player.transform.position, _targetPosition) < 1.0f)
            {
                if (_inventoryInteractable != null)
                {
                    _player.ChangeState(new InventoryInteractState(_player, _inventoryInteractable, _inventory));
                }
                else
                {
                    _player.ChangeState(new InteractState(_player, _interactable));
                }
            }
        }

        public override void Exit() { }
    }

    public class InteractState : State
    {
        private PlayerStateMachine _player;
        private IInteractable _interactable;
        
        private float _interactionTime;
        private float _elapsedTime;

        public InteractState(PlayerStateMachine player, IInteractable interactable)
        {
            _player = player;
            _interactable = interactable;
            _interactionTime = interactable.InteractionDuration;
        }

        public override void Enter()
        {
            _interactable.Interact();
            _elapsedTime = 0f;
        }

        public override void Execute()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _interactionTime)
            {
                _player.ChangeState(new IdleState(_player));
            }
        }

        public override void Exit()
        {
            if (_elapsedTime < _interactionTime)
            {
                _interactable.TerminateInteraction();
            }
        }
    }

    public class InventoryInteractState : State
    {
        private PlayerStateMachine _player;
        private IInventoryInteractable _interactable;
        private Inventory _inventory;

        private float _interactionTime;
        private float _elapsedTime;

        public InventoryInteractState(PlayerStateMachine player, IInventoryInteractable interactable, Inventory inventory)
        {
            _player = player;
            _interactable = interactable;
            _inventory = inventory;
            _interactionTime = interactable.InteractionDuration;
        }

        public override void Enter()
        {
            _interactable.InteractWithAnItem(_inventory);
            _elapsedTime = 0f;
        }

        public override void Execute()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _interactionTime)
            {
                _player.ChangeState(new IdleState(_player));
            }
        }

        public override void Exit() { }
    }
}