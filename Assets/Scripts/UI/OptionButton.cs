//#define EDITOR_QUIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    private GameObject option = null;

    private static bool isOpened = false;

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

        bgmSlider = option.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Slider>();
       
        if (bgmSlider == null)
        {
            Debug.LogError("bgmSlider is null!");
        }

        sfxSlider = option.transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).gameObject.GetComponent<Slider>();
        
        if (sfxSlider == null)
        {
            Debug.LogError("sfxSlider is null!");
        }
    }

    private void Update()
    {
        SoundManager.instance.audioSources[(int)SoundType.BGM].volume = bgmSlider.value;
        SoundManager.instance.audioSources[(int)SoundType.SFX].volume = sfxSlider.value; 
    }

    public void OnClickOptionButton()
    {
        if (!isOpened)
        {
            isOpened = true;
            option.SetActive(true);
        }
    }

    public void OnClickResumeButton()
    {
        if (isOpened)
        {
            isOpened = false;
            option.SetActive(false);
        }
    }

    public void OnClickResetButton()
    {
        if (isOpened)
        {
            GameManager.instance.ResetData();
            Application.Quit();
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
