using UnityEngine;
using System.Collections.Generic;

public class SoundSystem : MonoBehaviour
{
    [System.Serializable]
    public class SoundInfo
    {
        public float radius;
        public float duration;
    }

    [SerializeField] private float _baseHearingRadius = 10f;
    [SerializeField] private LayerMask _obstacleLayers;

    private Dictionary<string, SoundInfo> _soundTypes = new Dictionary<string, SoundInfo>
    {
        { "Footstep", new SoundInfo { radius = 5f, duration = 0.5f } },
        { "Door", new SoundInfo { radius = 15f, duration = 1f } },
        { "Crash", new SoundInfo { radius = 20f, duration = 2f } }
    };

    public bool CanHear(Vector3 soundPosition, string soundType)
    {
        if (!_soundTypes.TryGetValue(soundType, out SoundInfo soundInfo))
        {
            Debug.LogWarning($"Unrecognized sound type: {soundType}");
            return false;
        }

        float distance = Vector3.Distance(transform.position, soundPosition);
        
        if (distance <= soundInfo.radius)
        {
            // Проверяем, нет ли препятствий между источником звука и слушателем
            if (!Physics.Raycast(transform.position, soundPosition - transform.position, distance, _obstacleLayers))
            {
                return true;
            }
        }

        return false;
    }

    public void MakeSound(Vector3 position, string soundType)
    {
        // Этот метод будет вызываться игроком или объектами, издающими звук
        Debug.Log($"Sound {soundType} made at {position}");
        // Здесь можно добавить логику для уведомления всех NPC в радиусе слышимости
    }
}