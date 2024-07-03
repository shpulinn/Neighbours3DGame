using System.Collections;
using _Neighbours.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask doorLayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, groundLayer))
            {
                Vector3 destination = hit.point;
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(destination, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetDestination(destination);
                }
                else
                {
                    FindAndUseDoor(destination);
                }
            }
        }
    }

    private void FindAndUseDoor(Vector3 destination)
    {
        Collider[] doors = Physics.OverlapSphere(transform.position, 10, doorLayer);

        if (doors.Length > 0)
        {
            Collider nearestDoor = doors[0];
            float nearestDistance = Vector3.Distance(transform.position, doors[0].transform.position);

            foreach (Collider door in doors)
            {
                float distance = Vector3.Distance(transform.position, door.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDoor = door;
                    nearestDistance = distance;
                }
            }

            Door doorScript = nearestDoor.GetComponent<Door>();
            doorScript.OpenDoor();
            agent.SetDestination(nearestDoor.transform.position);
            StartCoroutine(CloseDoorAfterUse(doorScript, destination));
        }
    }

    private IEnumerator CloseDoorAfterUse(Door door, Vector3 destination)
    {
        yield return new WaitForSeconds(1);
        door.CloseDoor();
        agent.SetDestination(destination);
    }
}


