using _Neighbours.Scripts.Interactables;
using _Neighbours.Scripts.States;
using _Neighbours.Scripts.UI;
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
        private PlayerController _playerController;
        private Vector3 _targetPosition;
        private float _minDistance;
        private IInteractable _interactable;
        private IInventoryInteractable _inventoryInteractable;
        private Inventory _inventory;
        private IHidable _hidable;

        public ApproachState(PlayerStateMachine player, Vector3 targetPosition, float minDistance, IInteractable interactable, Inventory inventory = null, PlayerController playerController = null)
        {
            _player = player;
            _playerController = playerController;
            _targetPosition = targetPosition;
            _minDistance = minDistance;
            _interactable = interactable;
            _inventoryInteractable = interactable as IInventoryInteractable;
            _inventory = inventory;
            _hidable = interactable as IHidable;
        }

        public override void Enter()
        {
            _player.Agent.SetDestination(_targetPosition);
        }

        public override void Execute()
        {
            if (Vector3.Distance(_player.transform.position, _targetPosition) < _minDistance)
            {
                if (_inventoryInteractable != null)
                {
                    _player.ChangeState(new InventoryInteractState(_player, _inventoryInteractable, _inventory));
                }
                else if(_hidable != null)
                {
                    _player.ChangeState(new HideState(_player, _playerController));
                }
                else
                {
                    _player.ChangeState(new InteractState(_player, _interactable, _playerController.ProgressBar));
                }
            }
        }

        public override void Exit() { }
    }

    public class InteractState : State
    {
        private PlayerStateMachine _player;
        private IInteractable _interactable;
        private ProgressBar _progressBar;
        
        private float _interactionTime;
        private float _elapsedTime;

        public InteractState(PlayerStateMachine player, IInteractable interactable, ProgressBar progressBar)
        {
            _player = player;
            _interactable = interactable;
            _interactionTime = interactable.InteractionDuration;
            _progressBar = progressBar;
        }

        public override void Enter()
        {
            _interactable.Interact();
            _progressBar.StartAction("InteractWith", _interactionTime);
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
                _progressBar.StopAction();
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
    
    public class HideState : State
    {
        private PlayerStateMachine _player;
        private PlayerController _playerController;

        public HideState(PlayerStateMachine player, PlayerController playerController)
        {
            _player = player;
            _playerController = playerController;
        }

        public override void Enter()
        {
            _playerController.transform.GetChild(0).gameObject.SetActive(false);
        }
        public override void Execute()
        { }

        public override void Exit()
        {
            _playerController.transform.GetChild(0).gameObject.SetActive(true);

        }
    }
}