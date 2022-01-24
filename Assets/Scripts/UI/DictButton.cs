using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DictButton : MonoBehaviour
{
    private static bool isOpened = false;

    // 0: Whisk, 1: Recipe, 2: Skill, 3: Building, 4: Cat, 5: Pet
    private Image[] categoryButtonImages = null;
    private GameObject[] dictContents = null;

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
        GameObject dictionary = GameObject.Find("Dictionary");
        GameManager.CheckNull(dictionary);

        GameObject category = dictionary.transform.GetChild(1).gameObject;
        GameManager.CheckNull(category);

        Debug.Log(category.transform.childCount);

        categoryButtonImages = new Image[category.transform.childCount];

        for (int i = 0; i < category.transform.childCount; ++i)
        {
            categoryButtonImages[i] = category.transform.GetChild(i).GetComponent<Image>();
        }

        GameObject contents = dictionary.transform.GetChild(2).gameObject;
        GameManager.CheckNull(contents);

        dictContents = new GameObject[categoryButtonImages.Length];

        for (int i = 0; i < categoryButtonImages.Length; ++i)
        {
            dictContents[i] = contents.transform.GetChild(i).gameObject;
        }

        dictAnimator = dictionary.GetComponent<Animator>();
        GameManager.CheckNull(dictAnimator);

        AudioSource[] audioSources = GetComponents<AudioSource>();
        GameManager.CheckNull(audioSources);
        SoundManager.instance.RegisterAudioclip("BookSlap", audioSources[0].clip);
        SoundManager.instance.RegisterAudioclip("BookFlip", audioSources[1].clip);
    }

    public void OnClickDictButton()
    {
        if (GameManager.instance.IsClosed())
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

    public void OnClickCategoryButton(int index)
    {
        if (index < 0 || index > categoryButtonImages.Length)
        {
            return;
        }

        if (!dictContents[index].activeSelf)
        {
            for (int i = 0; i < categoryButtonImages.Length; ++i)
            {
                if (i == index)
                {
                    categoryButtonImages[i].color = activeColor;
                    dictContents[i].SetActive(true);
                }
                else
                {
                    categoryButtonImages[i].color = inactiveColor;
                    dictContents[i].SetActive(false);
                }
            }

            SoundManager.instance.PlaySFX("BookFlip");
        }
    }
}
