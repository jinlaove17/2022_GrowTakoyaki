using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    // * �ɼ�â�� ���ȴ����� ���� ����
    private static bool isOpened = false;
    
    // * �ɼ�â
    private GameObject option = null;

    // * ���� ���� ������
    public AudioClip[] audioClips = null;
    private Slider bgmSlider = null;
    private Slider sfxSlider = null;

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
        option = GameObject.Find("UI").transform.Find("Option").gameObject;
        SystemManager.CheckNull(option);

        bgmSlider = option.transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).gameObject.GetComponent<Slider>();
        SystemManager.CheckNull(bgmSlider);

        sfxSlider = option.transform.GetChild(0).transform.GetChild(0).transform.GetChild(4).gameObject.GetComponent<Slider>();
        SystemManager.CheckNull(sfxSlider);

        SystemManager.CheckNull(audioClips);
        SoundManager.instance.RegisterAudioclip("PAUSE_IN", audioClips[0]);
        SoundManager.instance.RegisterAudioclip("PAUSE_OUT", audioClips[1]);
    }

    private void Update()
    {
        if (isOpened)
        {
            // �ɼ�â�� �������� ����, ������� ȿ������ ũ�⸦ �����̴� ���� ���� ���� ����
            SoundManager.instance.audioSources[(int)SoundType.BGM].volume = bgmSlider.value;
            SoundManager.instance.audioSources[(int)SoundType.SFX].volume = sfxSlider.value;
        }
    }

    public void OnClickOptionButton()
    {
        if (!isOpened)
        {
            isOpened = true;
            option.SetActive(true);

            SoundManager.instance.PlaySFX("PAUSE_IN", 0.8f);
        }
    }

    public void OnClickResumeButton()
    {
        if (isOpened)
        {
            isOpened = false;
            option.SetActive(false);

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
