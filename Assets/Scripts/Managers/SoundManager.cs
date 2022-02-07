using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사운드의 종류
public enum SoundType
{
    BGM,
    SFX,
    ENUM_COUNT
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    // 오디오 클립 모음
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    // 오디오 플레이어(0: 배경음(BGM), 1: 효과음(SFX))
    public List<AudioSource> audioSources = new List<AudioSource>();

    // 배경음 클립 모음
    public AudioClip[] bgmClips = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        GetComponents<AudioSource>(audioSources);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 기본 배경음 + 피버모드 배경음을 등록한다.
        RegisterAudioclip("BGM", bgmClips[0]);
        RegisterAudioclip("FEVER_TIME", bgmClips[1]);

        // 기본 배경음을 재생한다.
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
                // 재생중인 배경음이 있다면, 먼저 정지시킨다.
                audioSources[(int)SoundType.BGM].Stop();
            }

            audioSources[(int)SoundType.BGM].clip = audioClips[clipName];
            audioSources[(int)SoundType.BGM].Play();
        }
    }

    public void PlaySFX(string clipName, float volume = 1.0f)
    {
        if (clipName != null)
        {
            if (audioClips.ContainsKey(clipName))
            {
                audioSources[(int)SoundType.SFX].PlayOneShot(audioClips[clipName], volume);
            }
        }
    }
}
