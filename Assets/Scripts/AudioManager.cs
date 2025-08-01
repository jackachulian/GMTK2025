using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] music, sfx;
    public AudioSource musicSource;

    [SerializeField] private int maxSounds = 16;
    private List<AudioSource> sfxSources = new List<AudioSource>();

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

        foreach (var sfx in sfx)
            sfxPlayCounts[sfx.name] = 0;
        
        for (int i = 0; i < maxSounds; i++)
        {
            GameObject o = Instantiate(newObject, transform);
            sfxSources.Add(o.GetComponent<AudioSource>());
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(music, x => x.name == name);

        if (s == null) Debug.Log("Invalid music name");
        else
        {
            musicSource.pitch = UnityEngine.Random.Range(s.pitchRange.x, s.pitchRange.y);
            musicSource.clip = s.Clip;
            musicSource.Play();
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
    public AudioClip Clip {get => clips[UnityEngine.Random.Range(0, clips.Length)]; }
    public float volumeScale = 1f;
    public Vector2 pitchRange = Vector2.one;
    public float[] sequencePitches;
}