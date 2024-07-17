using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Neighbours.Scripts.States
{
    public class NeighbourStateMachine : StateMachine
    {
        [SerializeField] private ActivityRoute _activityRoute;
        
        public ActivityRoute ActivityRoute => _activityRoute;

        public int CurrentPointIndex { get; set; }

        protected override void Start()
        {
            base.Start();
            ChangeState(new PatrolState(this));
        }

        // private void Update()
        // {
        //     if (PlayerDetected())
        //     {
        //         ChangeState(new ChasePlayerState(this, Player));
        //     }
        // }

        // private bool PlayerDetected()
        // {
        //     if (Player == null)
        //         return false;
        //
        //     return Vector3.Distance(transform.position, Player.position) <= detectionRadius;
        // }
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

        public override void Enter() { }

        public override void Execute()
        {
            _neighbour.Agent.SetDestination(_player.position);
        }

        public override void Exit() { }
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
                //Debug.Log(_activityRoute.activities[_currentPointIndex]);
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