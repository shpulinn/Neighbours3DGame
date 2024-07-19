using System.Collections;
using System.Threading.Tasks;
using _Neighbours.Scripts.Neighbour;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Neighbours.Scripts.States
{
    public class NeighbourStateMachine : StateMachine
    {
        [SerializeField] private ActivityRoute _activityRoute;
        
        public ActivityRoute ActivityRoute => _activityRoute;

        public int CurrentPointIndex { get; set; }
        
        private NeighbourVision _visionSystem;
        [SerializeField] private Transform _playerTransform;
        
        private Coroutine _visionCheckCoroutine;

        protected override void Start()
        {
            base.Start();
            _visionSystem = GetComponent<NeighbourVision>();
            if (_visionSystem == null)
            {
                Debug.LogError("VisionSystem component not found on the Neighbour!");
            }
            ChangeState(new PatrolState(this));
            
            _visionCheckCoroutine = StartCoroutine(VisionCheckRoutine());
        }
        
        private IEnumerator VisionCheckRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(_visionSystem.VisionCheckInterval);
        
            while (true)
            {
                if (_visionSystem != null && _visionSystem.CanSeeTarget(_playerTransform))
                {
                    ChangeState(new ChasePlayerState(this, _playerTransform));
                }
            
                yield return wait;
            }
        }

        protected  void OnDisable()
        {
            // Останавливаем корутину при отключении объекта
            if (_visionCheckCoroutine != null)
            {
                StopCoroutine(_visionCheckCoroutine);
            }
        }
    }
    
    public class IdleState : State
    {
        private StateMachine _stateMachine;

        public IdleState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public override void Enter()
        {
            _stateMachine.ChangeState(new PatrolState((NeighbourStateMachine)_stateMachine));
        }

        public override void Execute()
        {
            
        }

        public override void Exit() { }
    }

    public class MoveState : State
    {
        private StateMachine _stateMachine;
        private Vector3 _destination;

        public MoveState(StateMachine stateMachine, Vector3 destination)
        {
            _stateMachine = stateMachine;
            _destination = destination;
        }

        public override void Enter()
        {
            _stateMachine.Agent.SetDestination(_destination);
        }

        public override void Execute()
        {
            if (Vector3.Distance(_stateMachine.transform.position, _destination) <= _stateMachine.Agent.stoppingDistance)
            {
                _stateMachine.ChangeState(new IdleState(_stateMachine));
            }
        }

        public override void Exit() { }
    }

    public class ChasePlayerState : State
    {
        private NeighbourStateMachine _neighbour;
        private Transform _player;

        public ChasePlayerState(NeighbourStateMachine neighbour, Transform player)
        {
            _neighbour = neighbour;
            _player = player;
        }

        public override void Enter()
        {
            _neighbour.Agent.isStopped = false;
        }

        public override void Execute()
        {
            if (_player != null)
            {
                _neighbour.Agent.SetDestination(_player.position);

                if (Vector3.Distance(_neighbour.transform.position, _player.position) <= _neighbour.Agent.stoppingDistance)
                {
                    // Игрок пойман, завершаем игру
                    Debug.Log("Игрок пойман! Игра окончена.");
                    // Здесь можно вызвать метод для завершения игры
                }
            }
            else
            {
                _neighbour.ChangeState(new IdleState(_neighbour));
            }
        }

        public override void Exit()
        {
            _neighbour.Agent.isStopped = true;
        }
    }

    public class SleepState : State
    {
        private NeighbourStateMachine _neighbour;

        public SleepState(NeighbourStateMachine neighbour)
        {
            _neighbour = neighbour;
        }

        public override void Enter()
        {
            // Логика для входа в сон
        }

        public override void Execute()
        {
            // Логика для состояния сна
        }

        public override void Exit()
        {
            // Логика для выхода из состояния сна
        }
    }
    
    public class PatrolState : State
    {
        private NeighbourStateMachine _neighbour;
        private ActivityRoute _activityRoute;
        private int _currentPointIndex;
        private bool _isExecuting;

        public PatrolState(NeighbourStateMachine neighbour)
        {
            _neighbour = neighbour;
            _activityRoute = _neighbour.ActivityRoute;
            _currentPointIndex = _neighbour.CurrentPointIndex;
        }

        public override void Enter()
        {
            _isExecuting = false;
            MoveToNextPoint().Forget();
        }
        
        public override async void Execute()
        {
            // dk is it really need in here
        }

        public override void Exit()
        {
            _neighbour.CurrentPointIndex = _currentPointIndex;
        }

        private async UniTaskVoid MoveToNextPoint()
        {
            if (_activityRoute.activities.Count == 0 || IsAgentValid() == false)
                return;

            _neighbour.Agent.SetDestination(_activityRoute.activities[_currentPointIndex].Position);

            while (IsAgentValid() && (_neighbour.Agent.pathPending || _neighbour.Agent.remainingDistance > _neighbour.Agent.stoppingDistance))
            {
                await UniTask.Yield();
            }

            if (!_isExecuting)
            {
                _isExecuting = true;
                await PerformActivity(_activityRoute.activities[_currentPointIndex]);
                _isExecuting = false;
                _currentPointIndex = (_currentPointIndex + 1) % _activityRoute.activities.Count;
                MoveToNextPoint().Forget();
            }
        }

        private async UniTask PerformActivity(Activity activity)
        {
            // add animation/other logic?
            await UniTask.Delay((int)(activity.Duration * 1000));
        }
        
        private bool IsAgentValid()
        {
            return _neighbour.Agent != null && _neighbour.Agent.isActiveAndEnabled && _neighbour.Agent.isOnNavMesh;
        }
    }

}