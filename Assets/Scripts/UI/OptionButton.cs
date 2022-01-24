//#define EDITOR_QUIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    private static bool isOpened = false;

    private GameObject option = null;

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

        if (option == null)
        {
            Debug.LogError("option is null!");
        }

        bgmSlider = option.transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).gameObject.GetComponent<Slider>();
       
        if (bgmSlider == null)
        {
            Debug.LogError("bgmSlider is null!");
        }

        sfxSlider = option.transform.GetChild(0).transform.GetChild(0).transform.GetChild(4).gameObject.GetComponent<Slider>();
        
        if (sfxSlider == null)
        {
            Debug.LogError("sfxSlider is null!");
        }

        AudioSource[] audioSources = GetComponents<AudioSource>();

        SoundManager.instance.RegisterAudioclip("PAUSE_IN", audioSources[0].clip);
        SoundManager.instance.RegisterAudioclip("PAUSE_OUT", audioSources[1].clip);
    }

    private void Update()
    {
        if (isOpened)
        {
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
#if EDITOR_QUIT
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
