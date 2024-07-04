using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _Neighbours.Scripts.Player
{
    public class PlayerNavMeshLinkController : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Animator animator;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            agent.autoTraverseOffMeshLink = false;
        }

        void Update()
        {
            if (agent.isOnOffMeshLink)
            {
                StartCoroutine(HandleOffMeshLink());
            }
        }

        private IEnumerator HandleOffMeshLink()
        {
            OffMeshLinkData data = agent.currentOffMeshLinkData;
            Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

            //animator.SetTrigger("Jump");

            float jumpDuration = .2f;
            float jumpSpeed = .5f;

            float time = 0.0f;
            Vector3 startPos = transform.position;
            var renderer = agent.GetComponentInChildren<MeshRenderer>();
            renderer.enabled = false;
            while (time < jumpDuration)
            {
                time += Time.deltaTime * jumpSpeed;
                agent.transform.position = Vector3.Lerp(startPos, endPos, time / jumpDuration);
                yield return null;
            }

            renderer.enabled = true;

            agent.CompleteOffMeshLink();
        }
    }

}