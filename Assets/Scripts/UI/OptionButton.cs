using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    // 옵션창이 열렸는지에 대한 여부
    private static bool isOpened = false;
    
    // 옵션창 객체 데이터
    private Transform   option = null;

    // 사운드 관련 데이터
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
            // 옵션창이 열려있을 때만, 배경음과 효과음의 크기를 슬라이더 바의 값에 따라 조정한다.
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
