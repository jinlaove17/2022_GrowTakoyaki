using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    // �ɼ�â�� ���ȴ����� ���� ����
    private static bool isOpened = false;
    
    // �ɼ�â ��ü ������
    private Transform   option = null;

    // ���� ���� ������
    private Slider      bgmSlider = null;
    private Slider      sfxSlider = null;
    public AudioClip[]  audioClips = null;

    public static bool IsOpened
    {
        set
        {
            isOpened = value;
        }

        get
        {
            return isOpened;
        }
    }

    private void Awake()
    {
        option = GameObject.Find("UI").transform.Find("Option");
        SystemManager.CheckNull(option);

        bgmSlider = option.GetChild(0).GetChild(0).GetChild(2).GetComponent<Slider>();
        SystemManager.CheckNull(bgmSlider);

        sfxSlider = option.GetChild(0).GetChild(0).GetChild(4).GetComponent<Slider>();
        SystemManager.CheckNull(sfxSlider);

        SystemManager.CheckNull(audioClips);
        SoundManager.instance.RegisterAudioclip("PAUSE_IN", audioClips[0]);
        SoundManager.instance.RegisterAudioclip("PAUSE_OUT", audioClips[1]);
    }

    private void Update()
    {
        if (isOpened)
        {
            // �ɼ�â�� �������� ����, ������� ȿ������ ũ�⸦ �����̴� ���� ���� ���� �����Ѵ�.
            SoundManager.instance.audioSources[(int)SoundType.BGM].volume = bgmSlider.value;
            SoundManager.instance.audioSources[(int)SoundType.SFX].volume = sfxSlider.value;
        }
    }

    public void OnClickOptionButton()
    {
        if (!isOpened)
        {
            isOpened = true;
            option.gameObject.SetActive(true);

            SoundManager.instance.PlaySFX("PAUSE_IN", 0.8f);
        }
    }

    public void OnClickResumeButton()
    {
        if (isOpened)
        {
            isOpened = false;
            option.gameObject.SetActive(false);

            SoundManager.instance.PlaySFX("PAUSE_OUT", 1.0f);
        }
    }

    public void OnClickExitButton()
    {
        if (isOpened)
        {
            Application.Quit();
        }
    }
}
