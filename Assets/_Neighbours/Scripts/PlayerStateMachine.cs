﻿using System;
using UnityEngine;
using UnityEngine.AI;

namespace _Neighbours.Scripts
{
    public abstract class State
    {
        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();
    }

    public class PlayerStateMachine : MonoBehaviour
    {
        [SerializeField] private bool logCurrentState = false;
        
        private State currentState;

        private NavMeshAgent _agent;

        public NavMeshAgent Agent => _agent;

        private void Start()
        {
            _agent = FindObjectOfType<PlayerController>().GetComponent<NavMeshAgent>();
        }

        public void ChangeState(State newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }
            currentState = newState;
            currentState.Enter();
        }

        public void Update()
        {
            if (currentState != null)
            {
                if (logCurrentState)
                {
                    Debug.Log(currentState);
                }
                currentState.Execute();
            }
        }
    }

    public class IdleState : State
    {
        private PlayerStateMachine player;

        public IdleState(PlayerStateMachine player)
        {
            this.player = player;
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
        private PlayerStateMachine player;
        private Vector3 destination;

        public MoveState(PlayerStateMachine player, Vector3 destination)
        {
            this.player = player;
            this.destination = destination;
        }

        public override void Enter()
        {
            player.Agent.SetDestination(destination);
        }
        public override void Execute()
        {
            Vector3 playerPosWithoutY = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            if (Vector3.Distance(playerPosWithoutY, destination) < 0.6f)
            {
                player.ChangeState(new IdleState(player));
            }
        }
        public override void Exit() { }
    }
    
    public class ApproachState : State
    {
        private PlayerStateMachine player;
        private Vector3 targetPosition;
        private IInteractable interactable;

        public ApproachState(PlayerStateMachine player, Vector3 targetPosition, IInteractable interactable)
        {
            this.player = player;
            this.targetPosition = targetPosition;
            this.interactable = interactable;
        }

        public override void Enter()
        {
            player.Agent.SetDestination(targetPosition);
        }

        public override void Execute()
        {
            if (Vector3.Distance(player.transform.position, targetPosition) < 1.0f)
            {
                player.ChangeState(new InteractState(player, interactable));
            }
        }

        public override void Exit() { }
    }

    public class InteractState : State
    {
        private PlayerStateMachine player;
        private IInteractable interactable;
        
        private float interactionTime;
        private float elapsedTime;

        public InteractState(PlayerStateMachine player, IInteractable interactable)
        {
            this.player = player;
            this.interactable = interactable;
            this.interactionTime = interactable.InteractionDuration;
        }

        public override void Enter()
        {
            interactable.Interact();
            elapsedTime = 0f;
        }

        public override void Execute()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= interactionTime)
            {
                player.ChangeState(new IdleState(player));
            }
        }

        public override void Exit()
        {
            if (elapsedTime < interactionTime)
            {
                interactable.TerminateInteraction();
            }
        }
    }

}