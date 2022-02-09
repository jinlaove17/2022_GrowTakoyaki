using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴
    public static GameManager instance = null;

    // * 세이브 게임 데이터는 LoadData() 함수에서 PlayerPrefs를 이용하여 초기화한다.
    // 골드 관련 데이터
    private uint              gold = 0;
    private uint              totalGold = 0;
    private Text              goldText = null;

    // 반죽 관련 데이터
    private uint              dough = 0;
    private uint              totalDough = 0;
    private Text              doughText = null;
    private uint              doughIncrement = 0;
    private uint              doughIncrementPerSec = 1;

    // 콤보 관련 데이터
    private uint              currentCombo = 0;
    private uint              totalCombo = 0;
    private uint              lastCombo = 0;
    private uint              maxCombo = 0;
    private uint              totalMaxCombo = 0;

    // 피버모드 관련 데이터
    private bool              isFeverMode = false;
    private Image             backgroundImage = null;

    // 고양이 애니메이션 관련 데이터
    private Animator          catAnimator = null;

    // 스킬 관련 데이터
    private SellButton        sellButton = null;
    private Button[]          skillButtons = null;
    private int               skillStatus = 0x00000000;

    // 안내 메시지 관련 데이터
    private Text              messageText = null;
    private Animator          messageAnimator = null;

    // 펫 관련 데이터
    private bool[]            hasPet = new bool[] { false, false, false };

    // 캔버스 관련 데이터
    public RectTransform      canvasRectTransform = null;

    // 프리팝 데이터
    public GameObject         starPrefab = null;
    public GameObject         scorePrefab = null;
    public GameObject         comboPrefab = null;
    public GameObject         feverPrefab = null;

    // 사운드 관련 데이터
    public AudioClip[]        audioClips = null;

    public uint Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;

            // 3자리마다 쉼표를 찍는다.
            goldText.text = string.Format("{0:#,0}", gold);
        }
    }

    public uint TotalGold
    {
        get
        {
            return totalGold;
        }

        set
        {
            totalGold = value;
        }
    }

    public uint Dough
    {
        get
        {
            return dough;
        }

        set
        {
            dough = value;

            // 3자리마다 쉼표를 찍는다.
            doughText.text = string.Format("{0:#,0}", dough);
        }
    }

    public uint TotalDough
    {
        get
        {
            return totalDough;
        }

        set
        {
            totalDough = value;
        }
    }

    public uint DoughIncrement
    {
        get
        {
            return doughIncrement;
        }

        set
        {
            doughIncrement = value;
        }
    }

    public uint DoughIncrementPerSec
    {
        get
        {
            return doughIncrementPerSec;
        }

        set
        {
            doughIncrementPerSec = value;
        }
    }

    public uint CurrentCombo
    {
        get
        {
            return currentCombo;
        }

        set
        {
            currentCombo = value;
        }
    }

    public uint TotalCombo
    {
        get
        {
            return totalCombo;
        }

        set
        {
            totalCombo = value;
        }
    }

    public uint MaxCombo
    {
        get
        {
            return maxCombo;
        }

        set
        {
            maxCombo = value;
        }
    }

    public uint TotalMaxCombo
    {
        get
        {
            return totalMaxCombo;
        }

        set
        {
            totalMaxCombo = value;
        }
    }

    public bool IsFeverMode
    {
        get
        {
            return isFeverMode;
        }
    }

    public void GetPet(PetType petType)
    {
       if (petType < 0 || petType >= PetType.ENUM_COUNT)
        {
            return;
        }

        hasPet[(int)petType] = true;
    }

    public bool HasPet(PetType petType)
    {
        return (petType < 0 || petType >= PetType.ENUM_COUNT) ? false : hasPet[(int)petType];
    }

    private void Awake()
    {
        // 60FPS
        Application.targetFrameRate = 60;

        if (instance == null)
        {
            instance = this;
        }

        goldText = GameObject.Find("Gold").GetComponentInChildren<Text>();
        SystemManager.CheckNull(goldText);

        doughText = GameObject.Find("Dough").GetComponentInChildren<Text>();
        SystemManager.CheckNull(doughText);

        backgroundImage = canvasRectTransform.GetChild(0).gameObject.GetComponent<Image>();
        SystemManager.CheckNull(backgroundImage);

        catAnimator = GameObject.Find("Cat").GetComponent<Animator>();
        SystemManager.CheckNull(catAnimator);

        sellButton = GameObject.Find("SellButton").GetComponent<SellButton>();
        SystemManager.CheckNull(sellButton);

        skillButtons = new Button[2];
        skillButtons[0] = GameObject.Find("UI").transform.GetChild(3).GetComponent<Button>();
        skillButtons[1] = GameObject.Find("UI").transform.GetChild(4).GetComponent<Button>();
        SystemManager.CheckNull(skillButtons[0]);
        SystemManager.CheckNull(skillButtons[1]);

        GameObject messageObject = GameObject.Find("Message");
        SystemManager.CheckNull(messageObject);

        messageAnimator = messageObject.GetComponent<Animator>();
        SystemManager.CheckNull(messageAnimator);

        messageText = messageObject.GetComponentInChildren<Text>();
        SystemManager.CheckNull(messageText);

        SystemManager.CheckNull(audioClips);
        SoundManager.instance.RegisterAudioclip("Click", audioClips[0]);

        DontDestroyOnLoad(gameObject);
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
        // 한 프레임에 doughIncrementPerSec 만큼씩 반죽을 증가시킨다.
        Dough += doughIncrementPerSec;
        totalDough += doughIncrementPerSec;

        if (IsAllClosed())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

                // 클릭한 위치가 ClickArea와 충돌한 경우에만, 이벤트를 발생시킨다.
                if (hit.collider != null)
                {
                    // 배경 앞쪽에서 렌더링되어야 하므로, 일정 값을 빼준다.
                    position.z = -50.0f;

                    if (HasPet(PetType.RED_CAT))
                    {
                        // 성난 냥이 펫을 보유한다면, 반죽량을 10% 추가적으로 상승시킨다.
                        StartCoroutine(Count(GoodsType.DOUGH, dough + 1.1f * doughIncrement, dough));
                        GenerateEffectWithPet(position);
                    }
                    else
                    {
                        StartCoroutine(Count(GoodsType.DOUGH, dough + doughIncrement, dough));
                        GenerateEffect(position);
                    }

                    StartCoroutine(UpdateCombo());
                }
            }
        }
    }

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
        // 해당 타입의 스킬을 가지고 있는지에 대한 여부를 리턴한다.
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
        // 해당 타입의 스킬이 사용중인지에 대한 여부를 리턴한다.
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
                    StartCoroutine(UpdateFeverMode(13.0f));
                    skillStatus = skillStatus & ~Constants.HAS_SKILL2;
                    skillStatus |= Constants.USING_SKILL2;
                    break;
            }

            // 스킬을 사용했기 때문에, 좌측 상단의 스킬 아이콘을 비활성화 시킨다.
            skillButtons[skillIndex].gameObject.SetActive(false);
        }
    }

    public bool IsAllClosed()
    {
        // 옵션창, 업적창, 상점창, 도감창이 모두 닫혀있는지 검사한다.
        return (!OptionButton.IsOpened) && (!AchievementButton.IsOpened) && (!ShopButton.IsOpened) && (!DictButton.IsOpened);
    }

    public void PrintMessage(string textContent)
    {
        // textContent의 내용으로 안내메세지를 화면에 출력한다.
        messageAnimator.Play("Show", -1, 0.0f);
        messageText.text = textContent;
    }

    private void GenerateEffect(Vector3 position)
    {
        // 별 파티클 이펙트를 출력한다.
        Instantiate(starPrefab, position, Quaternion.identity, canvasRectTransform);

        // 획득 점수 텍스트 이펙트를 출력한다.
        position.y -= 65.0f;
        GameObject scoreEffect = Instantiate(scorePrefab, position, Quaternion.identity, canvasRectTransform);
        scoreEffect.GetComponent<Text>().text = "+" + doughIncrement.ToString() + "L";

        // 콤보 텍스트 이펙트를 출력한다.
        GameObject comboEffect = Instantiate(comboPrefab, canvasRectTransform);
        Text comboText = comboEffect.GetComponent<Text>();
        Text maxComboText = comboEffect.transform.GetChild(0).GetComponent<Text>();
        comboText.text = currentCombo.ToString() + " COMBO!";
        maxComboText.text = "MAX " + maxCombo.ToString();

        // 고양이(요리사)의 애니메이션 트리거를 발생시킨다.
        catAnimator.SetTrigger("doTouch");

        // 클릭 사운드를 출력한다.
        SoundManager.instance.PlaySFX("Click", 0.5f);
    }

    private void GenerateEffectWithPet(Vector3 position)
    {
        GenerateEffect(position);

        // 성난 냥이 펫을 보유하고 있다면, 추가 적인 텍스트 이펙트를 출력한다.
        position.y -= 120.0f;
        GameObject scoreEffect = Instantiate(scorePrefab, position, Quaternion.identity, canvasRectTransform);
        Text scoreText = scoreEffect.GetComponent<Text>();
        scoreText.text = "+" + (0.1f * doughIncrement).ToString() + "L";
        scoreText.fontSize -= 10;
    }

    public IEnumerator WaitAndCall<T>(Action<T> func, T param, float seconds)
    {
        // seconds초동안 기다린 후,
        yield return new WaitForSeconds(seconds);

        // 매개변수로 넘어온 함수에 param을 파라미터로 넘겨주어 호출한다.
        // 이때, Action<>을 사용하기 때문에, 리턴 값을 void여야 한다.
        func(param);
    }

    public IEnumerator Count(GoodsType goods, float target, float current)
    {
        // 참고 : https://unitys.tistory.com/7

        const float duration = 0.15f;
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

                    // 목표 값이 현재 값보다 큰 경우에만, 총 반죽량을 증가시킨다.
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

                    // 목표 값이 현재 값보다 큰 경우에만, 총 골드를 증가시킨다.
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
        currentCombo += 1;
        totalCombo += 1;

        // 콤보가 FEVER_COMBO의 배수일 경우, 피버모드에 진입한다.
        if (currentCombo > 0 && (currentCombo % Constants.FEVER_COMBO) == 0)
        {
            if (HasSkill(SkillType.PLUS_FEVER_TIME))
            {
                // 진입 시, 피버모드 지속시간 증가 스킬을 가지고 있다면 13초동안 피버모드에 진입한다.
                UseSkill((int)SkillType.PLUS_FEVER_TIME);
            }
            else
            {
                // 스킬을 가지고 있지 않다면 8초동안 피버모드에 진입한다.
                StartCoroutine(UpdateFeverMode(8.0f));
            }
        }

        // 콤보는 마지막 클릭이후 COMBO_DURATION초동안 유지된다.
        yield return new WaitForSeconds(Constants.COMBO_DURATION);

        lastCombo += 1;

        // COMBO_DURATION초 이후에도 현재 콤보수가 최근 콤보수와 같다면, 콤보카운트를 0으로 설정한다.
        if (currentCombo == lastCombo)
        {
            // 만약 현재 콤보수가 최대 콤보수를 능가했다면, 최대 콤보수를 갱신한다.
            if (currentCombo > maxCombo)
            {
                totalMaxCombo = maxCombo = currentCombo;
            }

            // 갱신 이후에는 현재 콤보수와 최근 콤보수를 0으로 만든다.
            currentCombo = lastCombo = 0;
        }
    }

    private IEnumerator UpdateFeverMode(float duration)
    {
        isFeverMode = true;

        // 피버모드 텍스트 이펙트 출력
        GameObject feverEffect = Instantiate(feverPrefab, canvasRectTransform);
        Text feverText = feverEffect.transform.GetChild(0).GetComponent<Text>();
        feverText.text = duration + feverText.text;

        // 피버모드에 진입시, 획득 반죽량과 획득 골드량이 2배 증가한다.
        doughIncrement *= 2;
        SellButton.Income *= 2;

        // 뒷 배경색을 붉은색으로 설정한다.
        backgroundImage.color = Color.red;

        // 배경음을 피버모드 진입 BGM으로 변경한다.
        SoundManager.instance.PlayBGM("FEVER_TIME");

        yield return new WaitForSeconds(duration);

        // 지속시간 이후에는 다시 획득 반죽량과 획득 골드량을 원래대로 설정한다.
        doughIncrement /= 2;
        SellButton.Income /= 2;

        // 위의 값을 원래대로 변경 후에 false로 만들어주어야 업그레이드 값이 제대로 적용된다.
        isFeverMode = false;

        // 뒷 배경색을 원래대로 설정한다.
        backgroundImage.color = Color.white;

        // 스킬 지속시간이 모두 지났으므로, 사용중 상태를 제거한다.
        skillStatus = skillStatus & ~Constants.USING_SKILL2;

        // 기존 BGM 사운드 출력한다.
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

        // 스킬 지속시간이 모두 지났으므로, 사용중 상태를 제거한다.
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

        PlayerPrefs.SetInt("HAS_RED_CAT", System.Convert.ToInt16(hasPet[0]));
        PlayerPrefs.SetInt("HAS_YELLOW_CAT", System.Convert.ToInt16(hasPet[0]));
        PlayerPrefs.SetInt("HAS_GREEN_CAT", System.Convert.ToInt16(hasPet[0]));

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
        else Gold = 99999999;

        if (PlayerPrefs.HasKey("TOTAL_GOLD")) TotalGold = (uint)PlayerPrefs.GetInt("TOTAL_GOLD");
        else TotalGold = Gold;

        if (PlayerPrefs.HasKey("DOUGH")) Dough = (uint)PlayerPrefs.GetInt("DOUGH");
        else Dough = 99999999;

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

        if (PlayerPrefs.HasKey("HAS_RED_CAT")) hasPet[0] = System.Convert.ToBoolean(PlayerPrefs.GetInt("HAS_RED_CAT"));
        if (PlayerPrefs.HasKey("HAS_YELLOW_CAT")) hasPet[1] = System.Convert.ToBoolean(PlayerPrefs.GetInt("HAS_YELLOW_CAT"));
        if (PlayerPrefs.HasKey("HAS_GREEN_CAT")) hasPet[2] = System.Convert.ToBoolean(PlayerPrefs.GetInt("HAS_GREEN_CAT"));

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
