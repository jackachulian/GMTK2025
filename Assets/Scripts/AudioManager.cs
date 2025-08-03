using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] music, sfx;
    public AudioSource musicSource;
    public AudioHighPassFilter musicHighPassFilter;

    [SerializeField] private int maxSounds = 16;
    private List<AudioSource> sfxSources = new List<AudioSource>();
    private Sound currentMusic = null;

    public static AudioManager Instance;
    private Dictionary<string, int> sfxPlayCounts = new();
    private void Awake()
    {
        if (Instance != null)
        {
            // Destroy(gameObject);
            return;
        }
        // DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start()
    {
        GameObject newObject = new();
        AudioSource source = newObject.AddComponent<AudioSource>();
        
        for (int i = 0; i < maxSounds; i++)
        {
            GameObject o = Instantiate(newObject, transform);
            sfxSources.Add(o.GetComponent<AudioSource>());
        }

        foreach (var sfx in sfx)
            sfxPlayCounts[sfx.name] = 0;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var sfx in sfx)
            sfxPlayCounts[sfx.name] = 0;
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(music, x => x.name == name);

        if (s == null) Debug.Log("Invalid music name");
        else
        {
            if (currentMusic?.name == s.name) return;

            musicSource.clip = s.Clip;
            musicSource.Play();
            currentMusic = s;
        }

    }

    public void PlaySfx(string name)
    {
        Sound s = Array.Find(sfx, x => x.name == name);

        if (s == null) Debug.Log("Invalid sfx name");
        else
        {
            List<AudioSource> freeSources = sfxSources.Where(s => !s.isPlaying).ToList();
            if (freeSources.Count == 0) return;
            AudioSource sfxSource = freeSources[0];

            if (s.sequencePitches.Length > 0)
            {
                sfxSource.pitch = s.sequencePitches[sfxPlayCounts[name] % s.sequencePitches.Length];
            }
            else
            {
                sfxSource.pitch = UnityEngine.Random.Range(s.pitchRange.x, s.pitchRange.y);
            }
            
            sfxSource.clip = s.Clip;
            sfxSource.volume = s.volumeScale;
            sfxSource.Play();
            sfxPlayCounts[name]++;
        }

    }

}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip[] clips;
    public AudioClip Clip {get => (clips.Length > 0 ) ? clips[UnityEngine.Random.Range(0, clips.Length)] : null; }
    public float volumeScale = 1f;
    public Vector2 pitchRange = Vector2.one;
    public float[] sequencePitches;
}