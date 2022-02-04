using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementButton : MonoBehaviour
{
    // ����â�� ���ȴ����� ���� ����
    private static bool isOpened = false;

    // �� �����迭�� �ε����̴�.
    private static uint goldIndex = 0;
    private static uint doughIndex = 0;
    private static uint comboIndex = 0;
    private static uint maxComboIndex = 0;

    // ������ ������� ������� �迭�̴�.
    private Text[] titleTexts = null;
    private Text[] conditionTexts = null;
    private Button[] getButtons = null;

    // ������ ������� ������� �迭�̴�.
    private string[] goldTitles = new string[] { "Ƽ����� �»�", "���� ���� ������", "���� ���� �����", "�︸����", "������� ��������" };
    private string[] doughTitles = new string[] { "���� �Թ���", "���� ������", "������ ����", "������ ��Ź��", "�λ��� �����ϴ�" };
    private string[] comboTitles = new string[] { "�߰��� ����", "�⽺�� ����", "�ܷõ� ����", "���� �ε帮�� ����", "���� ��ü ����" };
    private string[] maxComboTitles = new string[] { "�հ����� �㳯��", "�հ��� ���� �ӹ�", "��Ÿ ����", "�ǵ�� �´´�", "�޺���" };

    // ������ ��ǥ���� ������� �迭�̴�.
    private uint[] goldGoal = new uint[] { 100000, 3000000, 25000000, 100000000, 999999999 };
    private uint[] doughGoal = new uint[] { 100000, 3000000, 25000000, 100000000, 999999999 };
    private uint[] comboGoal = new uint[] { 1000, 10000, 50000, 100000, 999999 };
    private uint[] maxComboGoal = new uint[] { 300, 800, 1200, 2000, 5000 };

    // ����â�� �ִϸ��̼� ����� ���� �ִϸ�����
    private Animator achievementAnimator = null;

    // ȿ�� ����
    public AudioClip[] audioClips = null;

    private void Awake()
    {
        GameObject achievement = GameObject.Find("Achievement");
        GameManager.CheckNull(achievement);

        GameObject contents = achievement.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).gameObject;

        int childCount = contents.transform.childCount;
        titleTexts = new Text[childCount];
        conditionTexts = new Text[childCount];
        getButtons = new Button[childCount];

        for (int i = 0; i < childCount; ++i)
        {
            GameObject content = contents.transform.GetChild(i).gameObject;

            titleTexts[i] = content.transform.GetChild(0).gameObject.GetComponent<Text>();
            GameManager.CheckNull(titleTexts[i]);

            conditionTexts[i] = content.transform.GetChild(1).gameObject.GetComponent<Text>();
            GameManager.CheckNull(conditionTexts[i]);

            getButtons[i] = content.transform.GetChild(2).gameObject.GetComponent<Button>();
            GameManager.CheckNull(getButtons[i]);
        }

        achievementAnimator = achievement.GetComponent<Animator>();
        GameManager.CheckNull(achievementAnimator);

        GameManager.CheckNull(audioClips);
        SoundManager.instance.RegisterAudioclip("Clear", audioClips[0]);
    }

    private void Start()
    {
        LoadData();
    }

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

    public static uint GoldIndex
    {
        set
        {
            goldIndex = value;
        }

        get
        {
            return goldIndex;
        }
    }

    public static uint DoughIndex
    {
        set
        {
            doughIndex = value;
        }

        get
        {
            return doughIndex;
        }
    }

    public static uint ComboIndex
    {
        set
        {
            comboIndex = value;
        }

        get
        {
            return comboIndex;
        }
    }
   
    public static uint MaxComboIndex
    {
        set
        {
            maxComboIndex = value;
        }

        get
        {
            return maxComboIndex;
        }
    }

    private void Update()
    {
        if (isOpened)
        {
            UpdateAchievements();
        }
    }

    private void LoadData()
    {
        getButtons[0].GetComponentInChildren<Text>().text = string.Format("{0:#,0}", (uint)(1000000 * Mathf.Pow(2, goldIndex))) + "G";
        getButtons[1].GetComponentInChildren<Text>().text = string.Format("{0:#,0}", (uint)(1000000 * Mathf.Pow(2, doughIndex))) + "G";
        getButtons[2].GetComponentInChildren<Text>().text = string.Format("{0:#,0}", (uint)(1000000 * Mathf.Pow(2, comboIndex))) + "G";
        getButtons[3].GetComponentInChildren<Text>().text = string.Format("{0:#,0}", (uint)(1000000 * Mathf.Pow(2, maxComboIndex))) + "G";

        UpdateAchievements();
    }

    private void ActivateReward()
    {
        if (goldIndex < goldGoal.Length && GameManager.instance.TotalGold >= goldGoal[goldIndex])
        {
            getButtons[0].interactable = true;
        }

        if (doughIndex < doughGoal.Length && GameManager.instance.TotalDough >= doughGoal[doughIndex])
        {
            getButtons[1].interactable = true;
        }

        if (comboIndex < comboGoal.Length && GameManager.instance.TotalCombo >= comboGoal[comboIndex])
        {
            getButtons[2].interactable = true;
        }

        if (maxComboIndex < maxComboGoal.Length && GameManager.instance.TotalMaxCombo >= maxComboGoal[maxComboIndex])
        {
            getButtons[3].interactable = true;
        }
    }

    private void UpdateAchievements()
    {
        // ��� ����
        titleTexts[0].text = "[���] " + goldTitles[goldIndex];
        conditionTexts[0].text = "���� ��� ȹ��\n" + string.Format("{0:#,0}", GameManager.instance.TotalGold) + "G/" + string.Format("{0:#,0}", goldGoal[goldIndex]) + "G ����!";

        // ���׷� ����
        titleTexts[1].text = "[���׷�] " + doughTitles[doughIndex];
        conditionTexts[1].text = "���� ���׷� ȹ��\n" + string.Format("{0:#,0}", GameManager.instance.TotalDough) + "L/" + string.Format("{0:#,0}", doughGoal[doughIndex]) + "L ����!";

        // �޺� ����
        titleTexts[2].text = "[�޺�] " + comboTitles[comboIndex];
        conditionTexts[2].text = "���� �޺� ȹ��\n" + string.Format("{0:#,0}", GameManager.instance.TotalCombo) + "ȸ/" + string.Format("{0:#,0}", comboGoal[comboIndex]) + "ȸ ����!";

        // �ִ� �޺� ����
        titleTexts[3].text = "[�ִ� �޺�] " + maxComboTitles[maxComboIndex];
        conditionTexts[3].text = "���� �ִ� �޺� ȹ��\n" + string.Format("{0:#,0}", GameManager.instance.TotalMaxCombo) + "ȸ/" + string.Format("{0:#,0}", maxComboGoal[maxComboIndex]) + "ȸ ����!";
        
        ActivateReward();
    }

    public void OnClickAchievementButton()
    {
        if (GameManager.instance.IsClosed())
        {
            isOpened = true;
            UpdateAchievements();
            achievementAnimator.SetTrigger("doShow");
            SoundManager.instance.PlaySFX("BookSlap");
        }
    }

    public void OnClickExitDictButton()
    {
        if (isOpened)
        {
            IsOpened = false;
            achievementAnimator.SetTrigger("doHide");
        }
    }

    public void OnClickGetReward(int Index)
    {
        if ( Index < 0 || Index >= getButtons.Length)
        {
            return;
        }

        uint rewardGold = 0;

        switch (Index)
        {
            case 0:
                rewardGold = (uint)(1000000 * Mathf.Pow(2, ++goldIndex));
                break;
            case 1:
                rewardGold = (uint)(1000000 * Mathf.Pow(2, ++doughIndex));
                break;
            case 2:
                rewardGold = (uint)(1000000 * Mathf.Pow(2, ++comboIndex));
                break;
            case 3:
                rewardGold = (uint)(1000000 * Mathf.Pow(2, ++maxComboIndex));
                break;
            case 4:
                break;
        }

        StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold + rewardGold, GameManager.instance.Gold));

        getButtons[Index].GetComponentInChildren<Text>().text = string.Format("{0:#,0}", rewardGold) + "G";
        getButtons[Index].interactable = false;

        UpdateAchievements();
        SoundManager.instance.PlaySFX("Clear");
    }
}