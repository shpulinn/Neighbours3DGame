using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;

        [HideInInspector]
        public AudioSource source;
    }

    private Sound[] _sounds;
    private AudioSource _musicSource;

    public Sound[] Sounds => _sounds;
    public AudioSource MusicSource => _musicSource;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in _sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name, float volume = 1f)
    {
        Sound s = System.Array.Find(_sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        volume = Mathf.Clamp(volume, 0f, 1f);
        s.source.volume = volume;
        s.source.Play();
    }

    public void StopPlaying(string name)
    {
        Sound s = System.Array.Find(_sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void PlayMusic(AudioClip musicClip)
    {
        StartCoroutine(CrossfadeMusic(musicClip));
    }

    private System.Collections.IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        float fadeTime = 1f;
        float t = 0;

        if (_musicSource.clip != null)
        {
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(1, 0, t / fadeTime);
                yield return null;
            }
        }

        _musicSource.clip = newClip;
        _musicSource.Play();

        t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(0, 1, t / fadeTime);
            yield return null;
        }
    }
}