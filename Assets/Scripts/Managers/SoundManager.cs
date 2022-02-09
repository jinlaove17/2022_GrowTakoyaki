using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 싱글톤 패턴
    public static SoundManager            instance = null;

    // 문자열로 오디오 클립을 관리하는 딕셔너리
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    // 오디오 플레이어(0: BGM, 1: SFX)
    public List<AudioSource>              audioSources = new List<AudioSource>();

    // 여러 배경음의 오디오 클립
    public AudioClip[]                    bgmClips = null;

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
