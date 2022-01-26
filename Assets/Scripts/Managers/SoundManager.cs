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

    // 오디오 클립 모음
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    // 오디오 플레이어(0: 배경음(BGM), 1: 효과음(SFX))
    public List<AudioSource> audioSources = new List<AudioSource>();

    // 배경음 클립 모음
    public AudioClip[] bgmClips = null;

    private void Awake()
    {
        instance = this;

        GetComponents<AudioSource>(audioSources);
    }

    private void Start()
    {
        RegisterAudioclip("BGM", bgmClips[0]);
        RegisterAudioclip("FEVER_TIME", bgmClips[1]);
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
