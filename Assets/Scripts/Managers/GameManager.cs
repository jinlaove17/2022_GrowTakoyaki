using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 상수 값 정의
static class Constants
{
    // 스킬의 상태(보유, 사용중 여부)
    public const int HAS_SKILL1 = 0x00000001;
    public const int HAS_SKILL2 = 0x00000002;
    public const int USING_SKILL1 = 0x00000004;
    public const int USING_SKILL2 = 0x00000008;
}

// 스킬의 종류(자동 판매, 피버모드 시간 증가)
public enum SkillType
{
    AUTO_SELL,
    PLUS_FEVER_TIME
}

// 재화의 종류(골드, 반죽량)
public enum GoodsType
{
    GOLD,
    DOUGH
};

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴
    public static GameManager instance = null;

    // 고양이의 애니메이션 출력을 위한 애니메이터
    private Animator catAnimator = null;

    // 게임 데이터는 LoadData() 함수에서 PlayerPrefs를 이용하여 초기화를 수행한다.
    private uint gold = 0;
    private uint totalGold = 0;
    private Text goldText = null;

    private uint dough = 0;
    private uint totalDough = 0;
    private Text doughText = null;
    private uint doughIncrement = 0;

    private uint currentCombo = 0;
    private uint totalCombo = 0;
    private uint lastCombo = 0;
    private uint maxCombo = 0;
    private uint totalMaxCombo = 0;

    // 피버모드 관련 데이터
    private bool isFeverMode = false;
    private Image backgroundImage = null;

    // 스킬 관련 처리를 하기 위한 데이터
    private SellButton sellButton = null;
    private Button[] skillButtons = null;
    private int skillStatus = 0x00000000;

    // 플레이어에게 안내메세지를 출력하기 위한 데이터
    private Animator messageAnimator = null;
    private Text messageText = null;

    public RectTransform canvasRectTransform = null;

    public GameObject starPrefab = null;
    public GameObject scorePrefab = null;
    public GameObject comboPrefab = null;
    public GameObject feverPrefab = null;

    public AudioClip[] audioClips = null;

    public uint Gold
    {
        set
        {
            gold = value;
            goldText.text = string.Format("{0:#,0}", gold);
        }

        get
        {
            return gold;
        }
    }

    public uint TotalGold
    {
        set
        {
            totalGold = value;
        }

        get
        {
            return totalGold;
        }
    }

    public uint Dough
    {
        set
        {
            dough = value;
            doughText.text = string.Format("{0:#,0}", dough);
        }

        get
        {
            return dough;
        }
    }

    public uint TotalDough
    {
        set
        {
            totalDough = value;
        }

        get
        {
            return totalDough;
        }
    }

    public uint DoughIncrement
    {
        set
        {
            doughIncrement = value;
        }

        get
        {
            return doughIncrement;
        }
    }

    public uint Combo
    {
        set
        {
            currentCombo = value;
        }

        get
        {
            return currentCombo;
        }
    }

    public uint TotalCombo
    {
        set
        {
            totalCombo = value;
        }

        get
        {
            return totalCombo;
        }
    }

    public uint MaxCombo
    {
        set
        {
            maxCombo = value;
        }

        get
        {
            return maxCombo;
        }
    }

    public uint TotalMaxCombo
    {
        set
        {
            totalMaxCombo = value;
        }

        get
        {
            return totalMaxCombo;
        }
    }

    public bool IsFeverMode
    {
        get
        {
            return isFeverMode;
        }
    }

    private void Awake()
    {
        // 프레임을 60으로 고정시킨다.
        Application.targetFrameRate = 60;

        // 싱글톤 패턴
        instance = this;

        catAnimator = GameObject.Find("Cat").GetComponent<Animator>();
        CheckNull(catAnimator);

        goldText = GameObject.Find("Gold").GetComponentInChildren<Text>();
        CheckNull(goldText);

        doughText = GameObject.Find("Dough").GetComponentInChildren<Text>();
        CheckNull(doughText);

        backgroundImage = canvasRectTransform.GetChild(0).gameObject.GetComponent<Image>();
        CheckNull(backgroundImage);

        sellButton = GameObject.Find("SellButton").GetComponent<SellButton>();
        CheckNull(sellButton);

        skillButtons = new Button[2];
        skillButtons[0] = GameObject.Find("UI").transform.GetChild(3).GetComponent<Button>();
        CheckNull(skillButtons[0]);
        skillButtons[1] = GameObject.Find("UI").transform.GetChild(4).GetComponent<Button>();
        CheckNull(skillButtons[1]);

        GameObject messageObject = GameObject.Find("Message");

        messageAnimator = messageObject.GetComponent<Animator>();
        CheckNull(messageAnimator);
        messageText = messageObject.GetComponentInChildren<Text>();
        CheckNull(messageText);

        CheckNull(audioClips);
        SoundManager.instance.RegisterAudioclip("Click", audioClips[0]);
    }

    private void Start()
    {
        LoadData();

        // 3초마다 자동으로 저장한다.
        StartCoroutine(AutoSave(3.0f));
        
        // 1초 후 안내 메세지를 출력해준다.
        StartCoroutine(WaitAndCall<string>(PrintMessage, "화면을 마구마구 터치해서 반죽을 모아보세요!", 1.0f));
    }

    private void Update()
    {
        Dough += 1;
        totalDough += 1;

        if (IsClosed())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

                if (hit.collider != null)
                {
                    StartCoroutine(UpdateCombo());
                    StartCoroutine(Count(GoodsType.DOUGH, dough + doughIncrement, dough));
                    GenerateEffect(position);

                    totalCombo += 1;
                }
            }
        }
    }

    public static void CheckNull(UnityEngine.Object obj)
    {
        if (!obj)
        {
            Debug.LogError(obj.name + "is null!");
        }
    }

    public static void CheckNull(UnityEngine.Object[] objs)
    {
        foreach (UnityEngine.Object obj in objs)
        {
            if (!obj)
            {
                Debug.LogError(obj.name + "is null!");
                return;
            }
        }
    }

    // 매개변수의 자료형이 uint면 유니티 엔진에서 인식되지 않는다는 것을 깨달음
    // 또한 매개변수는 최대 1개까지만 전달이 가능함
    public void GetSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.AUTO_SELL:
                skillStatus |= Constants.HAS_SKILL1;
                break;
            case SkillType.PLUS_FEVER_TIME:
                skillStatus |= Constants.HAS_SKILL2;
                break;
        }
    }

    public bool HasSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.AUTO_SELL:
                return (skillStatus & Constants.HAS_SKILL1) == Constants.HAS_SKILL1;
            case SkillType.PLUS_FEVER_TIME:
                return (skillStatus & Constants.HAS_SKILL2) == Constants.HAS_SKILL2;
        }

        return false;
    }

    public bool UsingSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.AUTO_SELL:
                return (skillStatus & Constants.USING_SKILL1) == Constants.USING_SKILL1;
            case SkillType.PLUS_FEVER_TIME:
                return (skillStatus & Constants.USING_SKILL2) == Constants.USING_SKILL2;
        }

        return false;
    }

    public void UseSkill(int skillIndex)
    {
        SkillType skillType = (SkillType)skillIndex;

        // 스킬을 보유하고 있고, 사용중이지 않은 경우에 사용할 수 있도록 한다.
        if (HasSkill(skillType) && !UsingSkill(skillType))
        {
            switch (skillType)
            {
                case SkillType.AUTO_SELL:
                    StartCoroutine(AutoSell());
                    skillStatus = skillStatus & ~Constants.HAS_SKILL1;
                    skillStatus |= Constants.USING_SKILL1;
                    break;
                case SkillType.PLUS_FEVER_TIME:
                    skillStatus = skillStatus & ~Constants.HAS_SKILL2;
                    skillStatus |= Constants.USING_SKILL2;
                    break;
            }

            skillButtons[skillIndex].gameObject.SetActive(false);
        }
    }

    public bool IsClosed()
    {
        return (!OptionButton.IsOpened) && (!AchievementButton.IsOpened) && (!ShopButton.IsOpened) && (!DictButton.IsOpened);
    }

    public void PrintMessage(string textContent)
    {
        messageAnimator.Play("Show", -1, 0.0f);
        messageText.text = textContent;
    }

    private void GenerateEffect(Vector3 position)
    {
        // 별 파티클 이펙트
        position.z = -50.0f;
        Instantiate(starPrefab, position, Quaternion.identity, canvasRectTransform);

        // 획득 점수 텍스트 이펙트
        position.y -= 65.0f;
        GameObject scoreEffect = Instantiate(scorePrefab, position, Quaternion.identity, canvasRectTransform);
        scoreEffect.GetComponent<Text>().text = "+" + doughIncrement.ToString() + "L";

        // 콤보 텍스트 이펙트
        GameObject comboEffect = Instantiate(comboPrefab, canvasRectTransform);
        Text comboText = comboEffect.GetComponent<Text>();
        Text maxComboText = comboEffect.transform.GetChild(0).GetComponent<Text>();
        comboText.text = currentCombo.ToString() + " COMBO!";
        maxComboText.text = "MAX " + maxCombo.ToString();

        // 고양이의 애니메이션 트리거 발생
        catAnimator.SetTrigger("doTouch");

        // 클릭사운드 출력
        SoundManager.instance.PlaySFX("Click", 0.5f);
    }

    public IEnumerator WaitAndCall<T>(Action<T> func, T param, float seconds)
    {
        // seconds초동안 기다린 후,
        yield return new WaitForSeconds(seconds);

        // 매개변수로 넘어온 함수를 호출한다.
        func(param);
    }

    // 참고 : https://unitys.tistory.com/7
    public IEnumerator Count(GoodsType goods, float target, float current)
    {
        float duration = 0.15f;
        float offset;

        switch (goods)
        {
            case GoodsType.DOUGH:
                offset = (target - current) / duration;

                if (offset >= 0.0f)
                {
                    while (current < target)
                    {
                        current += offset * Time.deltaTime;
                        doughText.text = string.Format("{0:#,0}", current);

                        yield return null;
                    }

                    totalDough += doughIncrement;
                }
                else
                {
                    while (current > target)
                    {
                        current += offset * Time.deltaTime;
                        doughText.text = string.Format("{0:#,0}", current);

                        yield return null;
                    }
                }

                Dough = (uint)target;
                break;
            case GoodsType.GOLD:
                offset = (target - current) / duration;

                if (offset >= 0.0f)
                {
                    while (current < target)
                    {
                        current += offset * Time.deltaTime;
                        goldText.text = string.Format("{0:#,0}", current);

                        yield return null;
                    }

                    totalGold += SellButton.Income;
                }
                else
                {
                    while (current > target)
                    {
                        current += offset * Time.deltaTime;
                        goldText.text = string.Format("{0:#,0}", current);

                        yield return null;
                    }
                }

                Gold = (uint)target;
                break;
        }
    }

    private IEnumerator UpdateCombo()
    {
        ++currentCombo;

        // 100 콤보 단위로 피버모드 진입
        if (currentCombo > 0 && (currentCombo % 100) == 0)
        {
            if (HasSkill(SkillType.PLUS_FEVER_TIME))
            {
                StartCoroutine(UpdateFeverMode(13.0f));
                UseSkill((int)SkillType.PLUS_FEVER_TIME);
            }
            else
            {
                StartCoroutine(UpdateFeverMode(8.0f));
            }
        }

        // 콤보는 마지막 클릭이후 0.65초동안 유지
        yield return new WaitForSeconds(0.65f);

        ++lastCombo;

        // 0.65초 이후에 이전과 콤보수가 같다면 최대 콤보를 갱신할지 검사한 후 콤보카운트를 0으로 설정
        if (currentCombo == lastCombo)
        {
            if (currentCombo > maxCombo)
            {
                totalMaxCombo = maxCombo = currentCombo;
            }

            currentCombo = lastCombo = 0;
        }
    }

    private IEnumerator UpdateFeverMode(float duration)
    {
        // 피버모드에 진입했다고 저장
        isFeverMode = true;

        GameObject feverEffect = Instantiate(feverPrefab, canvasRectTransform);
        Text feverText = feverEffect.transform.GetChild(0).GetComponent<Text>();
        feverText.text = duration + feverText.text;

        // 피버모드에 진입시, 8초동안 획득 반죽량과 획득 골드량 2배 증가
        doughIncrement *= 2;
        SellButton.Income *= 2;

        // 뒷 배경색을 붉은색으로 설정
        backgroundImage.color = Color.red;

        // 피버모드 진입 BGM 사운드 출력
        SoundManager.instance.PlayBGM("FEVER_TIME");

        yield return new WaitForSeconds(duration);

        // 지속시간 이후에는 다시 획득 반죽량과 획득 골드량을 원래대로 변경
        doughIncrement /= 2;
        SellButton.Income /= 2;

        // 위의 값을 원래대로 변경 후에 false로 만들어 줘야 잠깐 사이에도 업그레이드 방지
        isFeverMode = false;

        // 뒷 배경색을 원래대로 설정
        backgroundImage.color = Color.white;

        // 스킬 사용시간이 모두 지났으므로, 사용중 상태를 제거한다.
        skillStatus = skillStatus & ~Constants.USING_SKILL2;

        // 기존 BGM 사운드 출력
        SoundManager.instance.PlayBGM("BGM");
    }

    private IEnumerator AutoSell()
    {
        WaitForSeconds delayTime = new WaitForSeconds(6.0f * Time.deltaTime);
        float duration = 0.0f;

        while (duration < 10.0f)
        {
            duration += 6.0f * Time.deltaTime;
            sellButton.OnClickSellButton();

            yield return delayTime;
        }

        // 스킬 사용시간이 모두 지났으므로, 사용중 상태를 제거한다.
        skillStatus = skillStatus & ~Constants.USING_SKILL1;
    }

    private IEnumerator AutoSave(float cycleTime)
    {
        WaitForSeconds delayTime = new WaitForSeconds(cycleTime);

        while (true)
        {
            SaveData();

            yield return delayTime;
        }
    }

    public void SaveData()
    {
        // GameManager Data
        PlayerPrefs.SetInt("GOLD", (int)gold);
        PlayerPrefs.SetInt("TOTAL_GOLD", (int)totalGold);

        PlayerPrefs.SetInt("DOUGH", (int)dough);
        PlayerPrefs.SetInt("TOTAL_DOUGH", (int)totalDough);
        PlayerPrefs.SetInt("DOUGH_INCREMENT", (int)doughIncrement);

        PlayerPrefs.SetInt("MAX_COMBO", (int)maxCombo);
        PlayerPrefs.SetInt("TOTAL_COMBO", (int)totalCombo);
        PlayerPrefs.SetInt("TOTAL_MAX_COMBO", (int)totalMaxCombo);

        PlayerPrefs.SetInt("SKILL_STATUS", (int)skillStatus);

        // SellButton Data
        PlayerPrefs.SetInt("INCOME", (int)SellButton.Income);
        PlayerPrefs.SetInt("SALES", (int)SellButton.Sales);

        // AchievementButton Data
        PlayerPrefs.SetInt("GOLD_INDEX", (int)AchievementButton.GoldIndex);
        PlayerPrefs.SetInt("DOUGH_INDEX", (int)AchievementButton.DoughIndex);
        PlayerPrefs.SetInt("COMBO_INDEX", (int)AchievementButton.ComboIndex);
        PlayerPrefs.SetInt("MAX_COMBO_INDEX", (int)AchievementButton.MaxComboIndex);

        // ShopButton Data
        PlayerPrefs.SetInt("WHISK_LEVEL", (int)ShopButton.WhiskLevel);
        PlayerPrefs.SetInt("RECIPE_LEVEL", (int)ShopButton.RecipeLevel);
    }

    public void LoadData()
    {
        // GameManager Data
        if (PlayerPrefs.HasKey("GOLD")) Gold = (uint)PlayerPrefs.GetInt("GOLD");
        else Gold = 9999999;

        if (PlayerPrefs.HasKey("TOTAL_GOLD")) TotalGold = (uint)PlayerPrefs.GetInt("TOTAL_GOLD");
        else TotalGold = Gold;

        if (PlayerPrefs.HasKey("DOUGH")) Dough = (uint)PlayerPrefs.GetInt("DOUGH");
        else Dough = 9999999;

        if (PlayerPrefs.HasKey("TOTAL_DOUGH")) TotalDough = (uint)PlayerPrefs.GetInt("TOTAL_DOUGH");
        else TotalDough = Dough;

        if (PlayerPrefs.HasKey("DOUGH_INCREMENT")) DoughIncrement = (uint)PlayerPrefs.GetInt("DOUGH_INCREMENT");
        else DoughIncrement = 500;

        if (PlayerPrefs.HasKey("MAX_COMBO")) maxCombo = (uint)PlayerPrefs.GetInt("MAX_COMBO");
        else maxCombo = 0;

        if (PlayerPrefs.HasKey("TOTAL_COMBO")) totalCombo = (uint)PlayerPrefs.GetInt("TOTAL_COMBO");
        else totalCombo = 0;

        if (PlayerPrefs.HasKey("TOTAL_MAX_COMBO")) totalMaxCombo = (uint)PlayerPrefs.GetInt("TOTAL_MAX_COMBO");
        else totalMaxCombo = 0;

        if (PlayerPrefs.HasKey("SKILL_STATUS"))
        {
            skillStatus = PlayerPrefs.GetInt("SKILL_STATUS");

            if (HasSkill(SkillType.AUTO_SELL)) skillButtons[0].gameObject.SetActive(true);
            if (HasSkill(SkillType.PLUS_FEVER_TIME)) skillButtons[1].gameObject.SetActive(true);
        }
        else skillStatus = 0x00000000;

        // SellButton Data
        if (PlayerPrefs.HasKey("SALES")) SellButton.Sales = (uint)PlayerPrefs.GetInt("SALES");
        else SellButton.Sales = 100;

        if (PlayerPrefs.HasKey("INCOME")) SellButton.Income = (uint)PlayerPrefs.GetInt("INCOME");
        else SellButton.Income = 100;
        
        // AchievementButton Data
        if (PlayerPrefs.HasKey("GOLD_INDEX")) AchievementButton.GoldIndex = (uint)PlayerPrefs.GetInt("GOLD_INDEX");
        else AchievementButton.GoldIndex = 0;

        if (PlayerPrefs.HasKey("DOUGH_INDEX")) AchievementButton.DoughIndex = (uint)PlayerPrefs.GetInt("DOUGH_INDEX");
        else AchievementButton.DoughIndex = 0;

        if (PlayerPrefs.HasKey("COMBO_INDEX")) AchievementButton.ComboIndex = (uint)PlayerPrefs.GetInt("COMBO_INDEX");
        else AchievementButton.ComboIndex = 0;

        if (PlayerPrefs.HasKey("MAX_COMBO_INDEX")) AchievementButton.MaxComboIndex = (uint)PlayerPrefs.GetInt("MAX_COMBO_INDEX");
        else AchievementButton.MaxComboIndex = 0;

        // ShopButton Data
        if (PlayerPrefs.HasKey("WHISK_LEVEL")) ShopButton.WhiskLevel = (uint)PlayerPrefs.GetInt("WHISK_LEVEL");
        else ShopButton.WhiskLevel = 1;

        if (PlayerPrefs.HasKey("RECIPE_LEVEL")) ShopButton.RecipeLevel = (uint)PlayerPrefs.GetInt("RECIPE_LEVEL");
        else ShopButton.RecipeLevel = 1;
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }

    private void OnApplicationQuit()
    {
        SaveData();
        //ResetData();
    }
}
