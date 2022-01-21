using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    BGM,
    SFX,
    TypeCount
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    
    public List<AudioSource> audioSources = new List<AudioSource>();
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GetComponents<AudioSource>(audioSources);

        audioSources[(int)SoundType.BGM].loop = true;

        AudioSource[] canvasAudioSources = GameObject.Find("Canvas").GetComponents<AudioSource>();

        RegisterAudioclip("BGM", canvasAudioSources[0].clip);
        RegisterAudioclip("FEVER_TIME", canvasAudioSources[1].clip);
        PlayBGM("BGM");
    }

    public void RegisterAudioclip(string clipName, AudioClip audioClip)
    {
        if (clipName != null && audioClip != null)
        {
            if (!audioClips.ContainsKey(clipName))
            {
                audioClips.Add(clipName, audioClip);
            }
        }
    }

    public void PlayBGM(string clipName)
    {
        if (clipName != null && audioClips.ContainsKey(clipName))
        {
            if (audioSources[(int)SoundType.BGM].isPlaying)
            {
                audioSources[(int)SoundType.BGM].Stop();
            }

            audioSources[(int)SoundType.BGM].clip = audioClips[clipName];
            audioSources[(int)SoundType.BGM].Play();
        }
    }

    public void PlaySFX(string clipName, float volume = 1.0f)
    {
        if (clipName != null && audioClips.ContainsKey(clipName))
        {
            audioSources[(int)SoundType.SFX].PlayOneShot(audioClips[clipName], volume);
        }
    }
}
