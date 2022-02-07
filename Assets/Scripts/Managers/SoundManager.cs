using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ����
public enum SoundType
{
    BGM,
    SFX,
    ENUM_COUNT
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    // ����� Ŭ�� ����
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    // ����� �÷��̾�(0: �����(BGM), 1: ȿ����(SFX))
    public List<AudioSource> audioSources = new List<AudioSource>();

    // ����� Ŭ�� ����
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
        // �⺻ ����� + �ǹ���� ������� ����Ѵ�.
        RegisterAudioclip("BGM", bgmClips[0]);
        RegisterAudioclip("FEVER_TIME", bgmClips[1]);

        // �⺻ ������� ����Ѵ�.
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
                // ������� ������� �ִٸ�, ���� ������Ų��.
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
