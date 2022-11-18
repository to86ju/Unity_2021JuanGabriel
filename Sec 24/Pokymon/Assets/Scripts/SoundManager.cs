using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource effectsSurce, musicSource;

    public Vector2 pitchRange = Vector2.zero;

    public static SoundManager sharedInstance;

    private void Awake()
    {
        if (sharedInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            sharedInstance = this;
        }

        DontDestroyOnLoad(gameObject); //no se destrulle 
    }

    public void PlaySound(AudioClip clip)
    {
        effectsSurce.pitch = 1;
        effectsSurce.Stop();
        effectsSurce.clip = clip;        
        effectsSurce.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int index = Random.Range(0, clips.Length);
        float pitch = Random.Range(pitchRange.x, pitchRange.y);

        effectsSurce.pitch = pitch;
        PlaySound(clips[index]);
    }
}
