using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DictButton : MonoBehaviour
{
    // ����â�� �����ִ����� ���� ����
    private static bool isOpened = false;

    // ī�װ�/������ ���� ������(0: Whisk, 1: Recipe, 2: Building, 3: Cat, 4: Pet)
    private Image[]     categoryButtonImages = null;
    private Transform[] dictContents = null;

    // ������ �ִϸ��̼� ���� ������
    private Animator    dictAnimator = null;

    // ī�װ� ��ư�� Ȱ��/��Ȱ�� ����
    private Color       activeColor = new Color(240.0f / 255, 200.0f / 255, 150.0f / 255);
    private Color       inactiveColor = new Color(220.0f / 255, 220.0f / 255, 220.0f / 255);

    // ���� ���� ������
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
        Transform dictionary = GameObject.Find("Dictionary").transform;
        SystemManager.CheckNull(dictionary);

        Transform category = dictionary.GetChild(1);
        SystemManager.CheckNull(category);

        int childCount = category.childCount;
        categoryButtonImages = new Image[childCount];
        
        for (int i = 0; i < childCount; ++i)
        {
            categoryButtonImages[i] = category.GetChild(i).GetComponent<Image>();
            SystemManager.CheckNull(categoryButtonImages[i]);
        }

        Transform contents = dictionary.GetChild(2);
        SystemManager.CheckNull(contents);

        int arrayLength = categoryButtonImages.Length;
        dictContents = new Transform[arrayLength];

        for (int i = 0; i < arrayLength; ++i)
        {
            dictContents[i] = contents.GetChild(i);
            SystemManager.CheckNull(dictContents[i]);
        }

        dictAnimator = dictionary.GetComponent<Animator>();
        SystemManager.CheckNull(dictAnimator);

        SystemManager.CheckNull(audioClips);
        SoundManager.instance.RegisterAudioclip("BookSlap", audioClips[0]);
        SoundManager.instance.RegisterAudioclip("BookFlip", audioClips[1]);
    }

    public void UnlockWhiskDict(uint level, Text[] infoTexts)
    {
        // dictContents[0]: WhiskDict
        // dictContents[0].GetChild(0): Contents;
        Transform target = dictContents[0].GetChild(0).GetChild((int)level);

        // �̹��� �� Ȱ��ȭ
        target.GetChild(0).GetComponent<Image>().color = Color.white;

        // �ؽ�Ʈ Ȱ��ȭ
        Text[] newTexts = target.GetComponentsInChildren<Text>();

        for (int i = 0; i < newTexts.Length; ++i)
        {
            newTexts[i].text = infoTexts[i].text;
            newTexts[i].color = infoTexts[i].color;
        }
    }

    public void UnlockRecipeDict(uint level, Text[] infoTexts)
    {
        // dictContents[1]: RecipeDict
        // dictContents[1].GetChild(0): Contents;
        Transform target = dictContents[1].GetChild(0).GetChild((int)level);

        // �̹��� �� Ȱ��ȭ
        target.GetChild(0).GetComponent<Image>().color = Color.white;

        // �ؽ�Ʈ Ȱ��ȭ
        Text[] newTexts = target.GetComponentsInChildren<Text>();

        for (int i = 0; i < newTexts.Length; ++i)
        {
            newTexts[i].text = infoTexts[i].text;
            newTexts[i].color = infoTexts[i].color;
        }
    }

    public void UnlockPetDict(PetType petType, Text[] infoTexts)
    {
        // dictContents[4]: PetDict
        // dictContents[4].GetChild(0): Contents;
        Transform target = dictContents[4].GetChild(0).GetChild(((int)petType));

        // �̹��� �� Ȱ��ȭ
        target.GetChild(0).GetComponent<Image>().color = Color.white;

        // �ؽ�Ʈ Ȱ��ȭ
        Text[] newTexts = target.GetComponentsInChildren<Text>();

        for (int i = 0; i < newTexts.Length; ++i)
        {
            newTexts[i].text = infoTexts[i].text;
            newTexts[i].color = infoTexts[i].color;
        }
    }

    public void OnClickDictButton()
    {
        if (GameManager.instance.IsAllClosed())
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
        int arrayLength = categoryButtonImages.Length;

        if (index < 0 || index >= arrayLength)
        {
            return;
        }

        if (!dictContents[index].gameObject.activeSelf)
        {
            for (int i = 0; i < arrayLength; ++i)
            {
                if (i == index)
                {
                    categoryButtonImages[i].color = activeColor;
                    dictContents[i].gameObject.SetActive(true);
                }
                else
                {
                    categoryButtonImages[i].color = inactiveColor;
                    dictContents[i].gameObject.SetActive(false);
                }
            }

            SoundManager.instance.PlaySFX("BookFlip");
        }
    }
}
