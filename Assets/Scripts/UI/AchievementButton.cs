using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementButton : MonoBehaviour
{
    // 업적창이 열렸는지에 대한 여부
    private static bool isOpened = false;

    // 각 업적 배열의 인덱스
    private static uint goldIndex = 0;
    private static uint doughIndex = 0;
    private static uint comboIndex = 0;
    private static uint maxComboIndex = 0;

    // 업적의 내용들을 담기위한 배열
    private Transform[] achievementContents = null;
    private Text[]      titleTexts = null;
    private Text[]      conditionTexts = null;
    private Button[]    getButtons = null;

    // 업적의 제목들을 담기위한 배열
    private string[]    goldTitles = new string[] { "티끌모아 태산", "꽉찬 돼지 저금통", "돈이 돈을 만든다", "억만장자", "노블레스 오블리주" };
    private string[]    doughTitles = new string[] { "반죽 입문자", "반죽 숙련자", "반죽의 달인", "제빵왕 김탁구", "인생을 반죽하다" };
    private string[]    comboTitles = new string[] { "견고한 액정", "기스난 액정", "단련된 액정", "액정 두드리기 장인", "액정 교체 권장" };
    private string[]    maxComboTitles = new string[] { "손가락에 쥐날듯", "손가락 골절 임박", "난타 공연", "건들면 맞는다", "콤보왕" };

    // 업적의 목표량을 담기위한 배열
    private uint[]      goldGoal = new uint[] { 100000, 3000000, 25000000, 100000000, 999999999 };
    private uint[]      doughGoal = new uint[] { 100000, 3000000, 25000000, 100000000, 999999999 };
    private uint[]      comboGoal = new uint[] { 1000, 10000, 50000, 100000, 999999 };
    private uint[]      maxComboGoal = new uint[] { 300, 800, 1200, 2000, 5000 };

    // 업적의 애니메이션 관련 데이터
    private Animator    achievementAnimator = null;

    // 사운드 관련 데이터
    public AudioClip[]  audioClips = null;

    private void Awake()
    {
        Transform achievement = GameObject.Find("Achievement").transform;
        SystemManager.CheckNull(achievement);

        Transform contents = achievement.GetChild(1).GetChild(0).GetChild(0);
        int childCount = contents.childCount;

        achievementContents = new Transform[childCount];
        titleTexts = new Text[childCount];
        conditionTexts = new Text[childCount];
        getButtons = new Button[childCount];

        for (int i = 0; i < childCount; ++i)
        {
            Transform content = contents.GetChild(i);

            achievementContents[i] = content;
            SystemManager.CheckNull(achievementContents[i]);

            titleTexts[i] = content.GetChild(0).GetComponent<Text>();
            SystemManager.CheckNull(titleTexts[i]);

            conditionTexts[i] = content.GetChild(1).GetComponent<Text>();
            SystemManager.CheckNull(conditionTexts[i]);

            getButtons[i] = content.GetChild(2).GetComponent<Button>();
            SystemManager.CheckNull(getButtons[i]);
        }

        achievementAnimator = achievement.GetComponent<Animator>();
        SystemManager.CheckNull(achievementAnimator);

        SystemManager.CheckNull(audioClips);
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
        const uint basicReward = 1000000;

        // 각 업적의 버튼에 출력되는 보상 골드의 텍스트를 인덱스 값에 따라 갱신한다.
        getButtons[0].GetComponentInChildren<Text>().text = string.Format("{0:#,0}", (uint)(basicReward * Mathf.Pow(2, goldIndex))) + "G";
        getButtons[1].GetComponentInChildren<Text>().text = string.Format("{0:#,0}", (uint)(basicReward * Mathf.Pow(2, doughIndex))) + "G";
        getButtons[2].GetComponentInChildren<Text>().text = string.Format("{0:#,0}", (uint)(basicReward * Mathf.Pow(2, comboIndex))) + "G";
        getButtons[3].GetComponentInChildren<Text>().text = string.Format("{0:#,0}", (uint)(basicReward * Mathf.Pow(2, maxComboIndex))) + "G";

        UpdateAchievements();
    }

    private void ActivateReward()
    {
        // 각 업적이 조건을 충족한다면, 버튼을 활성화 시킨다.
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
        // 골드 업적
        if (goldIndex < goldGoal.Length)
        {
            titleTexts[0].text = "[골드] " + goldTitles[goldIndex];
            conditionTexts[0].text = "누적 골드 획득\n" + string.Format("{0:#,0}", GameManager.instance.TotalGold) + "G/" + string.Format("{0:#,0}", goldGoal[goldIndex]) + "G 돌파!";
        }

        // 반죽량 업적
        if (doughIndex < doughGoal.Length)
        {
            titleTexts[1].text = "[반죽량] " + doughTitles[doughIndex];
            conditionTexts[1].text = "누적 반죽량 획득\n" + string.Format("{0:#,0}", GameManager.instance.TotalDough) + "L/" + string.Format("{0:#,0}", doughGoal[doughIndex]) + "L 돌파!";
        }

        // 콤보 업적
        if (comboIndex < comboGoal.Length)
        {
            titleTexts[2].text = "[콤보] " + comboTitles[comboIndex];
            conditionTexts[2].text = "누적 콤보 획득\n" + string.Format("{0:#,0}", GameManager.instance.TotalCombo) + "회/" + string.Format("{0:#,0}", comboGoal[comboIndex]) + "회 돌파!";
        }

        // 최대 콤보 업적
        if (maxComboIndex < maxComboGoal.Length)
        {
            titleTexts[3].text = "[최대 콤보] " + maxComboTitles[maxComboIndex];
            conditionTexts[3].text = "누적 최대 콤보 획득\n" + string.Format("{0:#,0}", GameManager.instance.TotalMaxCombo) + "회/" + string.Format("{0:#,0}", maxComboGoal[maxComboIndex]) + "회 돌파!";
        }

        ActivateReward();
    }

    public void OnClickAchievementButton()
    {
        if (GameManager.instance.IsAllClosed())
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

    public void OnClickGetReward(int index)
    {
        if ( index < 0 || index >= getButtons.Length)
        {
            return;
        }

        const uint basicReward = 1000000;
        uint rewardGold = 0;
        uint nextRewardGold = 0;

        switch (index)
        {
            case 0:
                rewardGold = (uint)(basicReward * Mathf.Pow(2, goldIndex));
                nextRewardGold = (uint)(basicReward * Mathf.Pow(2, ++goldIndex));
                break;
            case 1:
                rewardGold = (uint)(basicReward * Mathf.Pow(2, doughIndex));
                nextRewardGold = (uint)(basicReward * Mathf.Pow(2, ++doughIndex));
                break;
            case 2:
                rewardGold = (uint)(basicReward * Mathf.Pow(2, comboIndex));
                nextRewardGold = (uint)(basicReward * Mathf.Pow(2, ++comboIndex));
                break;
            case 3:
                rewardGold = (uint)(basicReward * Mathf.Pow(2, maxComboIndex));
                nextRewardGold = (uint)(basicReward * Mathf.Pow(2, ++maxComboIndex));
                break;
            case 4:
                break;
        }

        StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold + rewardGold, GameManager.instance.Gold));

        if (goldIndex >= goldGoal.Length || doughIndex >= doughGoal.Length || comboIndex >= comboGoal.Length || maxComboIndex >= maxComboGoal.Length)
        {
            // 최종 단계의 업적을 클리어한 경우에는, 업적 리스트에서 비활성화 시킨다.
            achievementContents[index].gameObject.SetActive(false);
        }
        else
        {
            getButtons[index].GetComponentInChildren<Text>().text = string.Format("{0:#,0}", nextRewardGold) + "G";
            getButtons[index].interactable = false;

            UpdateAchievements();
        }

        SoundManager.instance.PlaySFX("Clear");
    }
}
