using UnityEngine;

namespace _Neighbours.Scripts
{
    public class FloorManager : MonoBehaviour
    {
        [SerializeField] private Transform[] doorsToSecondFloor;
        [SerializeField] private Transform[] doorsToFirstFloor;

        public Transform FindNearestDoor(Transform[] doors, Vector3 currentPos)
        {
            Transform nearestDoor = doors[0];
            float nearestDistance = Vector3.Distance(currentPos, doors[0].position);

            foreach (Transform door in doors)
            {
                float distance = Vector3.Distance(currentPos, door.position);
                if (distance < nearestDistance)
                {
                    nearestDoor = door;
                    nearestDistance = distance;
                }
            }

            return nearestDoor;
        }
    }

}