using UnityEngine;
using UnityEngine.AI;

namespace _Neighbours.Scripts.States
{
    public abstract class StateMachine : MonoBehaviour
    {
        private State _currentState;
        protected NavMeshAgent agent;

        public NavMeshAgent Agent => agent;

        protected virtual void Start()
        {
            agent = GetComponent<NavMeshAgent>();
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