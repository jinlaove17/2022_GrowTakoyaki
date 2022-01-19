using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DictButton : MonoBehaviour
{
    private static bool isOpened = false;

    private GameObject whiskDict = null;
    private GameObject recipeDict = null;

    private Button whiskDictButton = null;
    private Button recipeDictButton = null;

    private Animator dictAnimator = null;

    private Color activeColor = new Color(240.0f / 255, 200.0f / 255, 150.0f / 255);
    private Color inactiveColor = new Color(220.0f / 255, 220.0f / 255, 220.0f / 255);

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
        GameObject dict = GameObject.Find("Dictionary");

        whiskDict = dict.transform.GetChild(0).gameObject;

        if (whiskDict == null)
        {
            Debug.LogError("whiskDict is null!");
        }

        recipeDict = dict.transform.GetChild(1).gameObject;

        if (recipeDict == null)
        {
            Debug.LogError("recipeDict is null!");
        }

        whiskDictButton = dict.transform.GetChild(2).gameObject.GetComponent<Button>();

        if (whiskDictButton == null)
        {
            Debug.LogError("whiskDictButton is null!");
        }

        recipeDictButton = dict.transform.GetChild(3).gameObject.GetComponent<Button>();

        if (recipeDictButton == null)
        {
            Debug.LogError("recipeDictButton is null!");
        }

        dictAnimator = dict.GetComponent<Animator>();

        if (dictAnimator == null)
        {
            Debug.LogError("dictAnimator is null!");
        }

        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("audioSource is null!");
        }

        SoundManager.instance.RegisterAudioclip("BookSlap", audioSource.clip);

        audioSource = dict.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("audioSource is null!");
        }

        SoundManager.instance.RegisterAudioclip("BookFlip", audioSource.clip);
    }

    public void OnClickDictButton()
    {
        if (!isOpened)
        {
            isOpened = true;
            dictAnimator.SetTrigger("doShow");

            SoundManager.instance.PlaySFX("BookSlap");
        }
    }

    public void OnClickExitDictButton()
    {
        if (isOpened)
        {
            IsOpened = false;
            dictAnimator.SetTrigger("doHide");
        }
    }

    public void OnClickWhiskDictButton()
    {
        if (recipeDict.activeSelf)
        {
            whiskDictButton.GetComponent<Image>().color = activeColor;
            whiskDict.SetActive(true);

            recipeDictButton.GetComponent<Image>().color = inactiveColor;
            recipeDict.SetActive(false);

            SoundManager.instance.PlaySFX("BookFlip");
        }
    }

    public void OnClickRecipeDictButton()
    {
        if (whiskDict.activeSelf)
        {
            whiskDictButton.GetComponent<Image>().color = inactiveColor;
            whiskDict.SetActive(false);

            recipeDictButton.GetComponent<Image>().color = activeColor;
            recipeDict.SetActive(true);

            SoundManager.instance.PlaySFX("BookFlip");
        }
    }
}
