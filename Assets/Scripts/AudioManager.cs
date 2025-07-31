using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] music, sfx;
    public AudioSource musicSource, sfxSource;

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(music, x => x.name == name);

        if (s == null) Debug.Log("Invalid music name");
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }

    }

    public void PlaySfx(string name)
    {
        Sound s = Array.Find(sfx, x => x.name == name);

        if (s == null) Debug.Log("Invalid sfx name");
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.Play();
        }

    }

}


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}