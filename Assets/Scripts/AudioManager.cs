using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] music, sfx;
    public AudioSource musicSource, sfxSource;
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(music, x => x.name == name);

        if (s == null) Debug.Log("Invalid music name");
        else
        {
            sfxSource.pitch = UnityEngine.Random.Range(s.pitchRange.x, s.pitchRange.y);
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
            sfxSource.pitch = UnityEngine.Random.Range(s.pitchRange.x, s.pitchRange.y);
            sfxSource.clip = s.Clip;
            sfxSource.Play();
        }

    }

}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip[] clips;
    public AudioClip Clip {get => clips[UnityEngine.Random.Range(0, clips.Length)]; }
    public Vector2 pitchRange = Vector2.one;
}