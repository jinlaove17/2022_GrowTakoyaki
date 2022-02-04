using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementButton : MonoBehaviour
{
    // 업적창이 열렸는지에 대한 여부
    private static bool isOpened = false;

    // 각 업적배열의 인덱스이다.
    private static uint goldIndex = 0;
    private static uint doughIndex = 0;
    private static uint comboIndex = 0;
    private static uint maxComboIndex = 0;

    // 업적의 내용들을 담기위한 배열이다.
    private Text[] titleTexts = null;
    private Text[] conditionTexts = null;
    private Button[] getButtons = null;

    // 업적의 제목들을 담기위한 배열이다.
    private string[] goldTitles = new string[] { "티끌모아 태산", "꽉찬 돼지 저금통", "돈이 돈을 만든다", "억만장자", "노블레스 오블리주" };
    private string[] doughTitles = new string[] { "반죽 입문자", "반죽 숙련자", "반죽의 달인", "제빵왕 김탁구", "인생을 반죽하다" };
    private string[] comboTitles = new string[] { "견고한 액정", "기스난 액정", "단련된 액정", "액정 두드리기 장인", "액정 교체 권장" };
    private string[] maxComboTitles = new string[] { "손가락에 쥐날듯", "손가락 골절 임박", "난타 공연", "건들면 맞는다", "콤보왕" };

    // 업적의 목표량을 담기위한 배열이다.
    private uint[] goldGoal = new uint[] { 100000, 3000000, 25000000, 100000000, 999999999 };
    private uint[] doughGoal = new uint[] { 100000, 3000000, 25000000, 100000000, 999999999 };
    private uint[] comboGoal = new uint[] { 1000, 10000, 50000, 100000, 999999 };
    private uint[] maxComboGoal = new uint[] { 300, 800, 1200, 2000, 5000 };

    // 업적창의 애니메이션 출력을 위한 애니메이터
    private Animator achievementAnimator = null;

    // 효과 사운드
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
        // 골드 업적
        titleTexts[0].text = "[골드] " + goldTitles[goldIndex];
        conditionTexts[0].text = "누적 골드 획득\n" + string.Format("{0:#,0}", GameManager.instance.TotalGold) + "G/" + string.Format("{0:#,0}", goldGoal[goldIndex]) + "G 돌파!";

        // 반죽량 업적
        titleTexts[1].text = "[반죽량] " + doughTitles[doughIndex];
        conditionTexts[1].text = "누적 반죽량 획득\n" + string.Format("{0:#,0}", GameManager.instance.TotalDough) + "L/" + string.Format("{0:#,0}", doughGoal[doughIndex]) + "L 돌파!";

        // 콤보 업적
        titleTexts[2].text = "[콤보] " + comboTitles[comboIndex];
        conditionTexts[2].text = "누적 콤보 획득\n" + string.Format("{0:#,0}", GameManager.instance.TotalCombo) + "회/" + string.Format("{0:#,0}", comboGoal[comboIndex]) + "회 돌파!";

        // 최대 콤보 업적
        titleTexts[3].text = "[최대 콤보] " + maxComboTitles[maxComboIndex];
        conditionTexts[3].text = "누적 최대 콤보 획득\n" + string.Format("{0:#,0}", GameManager.instance.TotalMaxCombo) + "회/" + string.Format("{0:#,0}", maxComboGoal[maxComboIndex]) + "회 돌파!";
        
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
