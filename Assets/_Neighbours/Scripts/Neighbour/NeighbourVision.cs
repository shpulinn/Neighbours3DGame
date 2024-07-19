using System;
using System.Collections;
using UnityEngine;

namespace _Neighbours.Scripts.Neighbour
{
    public class NeighbourVision : MonoBehaviour
    {
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _fieldOfViewAngle = 90f;
        [SerializeField] private float _visionCheckInterval = .5f;
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private LayerMask _obstacleLayer;

        public float VisionCheckInterval => _visionCheckInterval;

        public bool CanSeeTarget(Transform target)
        {
            if (target == null)
                return false;

            Vector3 directionToTarget = target.position - transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget <= _detectionRadius)
            {
                float angle = Vector3.Angle(transform.forward, directionToTarget);
                if (angle <= _fieldOfViewAngle * 0.5f)
                {
                    // Проверяем, нет ли препятствий между соседом и игроком
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, directionToTarget, out hit, _detectionRadius, _targetLayer | _obstacleLayer))
                    {
                        // Если луч попал в игрока, и нет препятствий между ними
                        if (hit.transform == target)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);

            Vector3 viewAngleA = DirFromAngle(-_fieldOfViewAngle / 2, false);
            Vector3 viewAngleB = DirFromAngle(_fieldOfViewAngle / 2, false);

            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * _detectionRadius);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * _detectionRadius);
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
        // [SerializeField] private float _radius;
        // [Range(0,360)]
        // [SerializeField] private float _angle;
        //
        // [SerializeField] private GameObject _playerRef;
        //
        // [SerializeField] private LayerMask _targetMask;
        // [SerializeField] private LayerMask _obstructionMask;
        //
        // public bool canSeePlayer;
        //
        // public float Radius => _radius;
        // public float Angle => _angle;
        // public GameObject PlayerRef => _playerRef;
        //
        // private void Start()
        // {
        //     _playerRef = GameObject.FindGameObjectWithTag("Player");
        //     StartCoroutine(FOVRoutine());
        // }
        //
        // private IEnumerator FOVRoutine()
        // {
        //     WaitForSeconds wait = new WaitForSeconds(0.2f);
        //
        //     while (true)
        //     {
        //         yield return wait;
        //         FieldOfViewCheck();
        //     }
        // }
        //
        // private void FieldOfViewCheck()
        // {
        //     Collider[] rangeChecks = Physics.OverlapSphere(transform.position, _radius, _targetMask);
        //     Debug.Log(rangeChecks.Length);
        //
        //     if (rangeChecks.Length != 0)
        //     {
        //         Transform target = rangeChecks[0].transform;
        //         Vector3 directionToTarget = (target.position - transform.position).normalized;
        //
        //         if (Vector3.Angle(transform.forward, directionToTarget) < _angle / 2)
        //         {
        //             float distanceToTarget = Vector3.Distance(transform.position, target.position);
        //
        //             if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstructionMask))
        //                 canSeePlayer = true;
        //             else
        //                 canSeePlayer = false;
        //         }
        //         else
        //             canSeePlayer = false;
        //     }
        //     else if (canSeePlayer)
        //         canSeePlayer = false;
        // }
        //
        // private void OnDrawGizmos()
        // {
        //     Gizmos.DrawLine(transform.position, _playerRef.transform.position);
        //     Gizmos.DrawWireSphere(transform.position, _radius);
        // }
    }
}