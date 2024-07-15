using UnityEngine;
using UnityEngine.AI;

namespace _Neighbours.Scripts.States
{
    public abstract class StateMachine : MonoBehaviour
    {
        private State _currentState;

        public NavMeshAgent Agent { get; private set; }

        protected virtual void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
        }

        public void ChangeState(State newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        protected virtual void Update()
        {
            _currentState?.Execute();
        }
    }
}