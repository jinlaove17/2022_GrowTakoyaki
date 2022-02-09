using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // �̱��� ����
    public static GameManager instance = null;

    // * ���̺� ���� �����ʹ� LoadData() �Լ����� PlayerPrefs�� �̿��Ͽ� �ʱ�ȭ�Ѵ�.
    // ��� ���� ������
    private uint              gold = 0;
    private uint              totalGold = 0;
    private Text              goldText = null;

    // ���� ���� ������
    private uint              dough = 0;
    private uint              totalDough = 0;
    private Text              doughText = null;
    private uint              doughIncrement = 0;
    private uint              doughIncrementPerSec = 1;

    // �޺� ���� ������
    private uint              currentCombo = 0;
    private uint              totalCombo = 0;
    private uint              lastCombo = 0;
    private uint              maxCombo = 0;
    private uint              totalMaxCombo = 0;

    // �ǹ���� ���� ������
    private bool              isFeverMode = false;
    private Image             backgroundImage = null;

    // ����� �ִϸ��̼� ���� ������
    private Animator          catAnimator = null;

    // ��ų ���� ������
    private SellButton        sellButton = null;
    private Button[]          skillButtons = null;
    private int               skillStatus = 0x00000000;

    // �ȳ� �޽��� ���� ������
    private Text              messageText = null;
    private Animator          messageAnimator = null;

    // �� ���� ������
    private bool[]            hasPet = new bool[] { false, false, false };

    // ĵ���� ���� ������
    public RectTransform      canvasRectTransform = null;

    // ������ ������
    public GameObject         starPrefab = null;
    public GameObject         scorePrefab = null;
    public GameObject         comboPrefab = null;
    public GameObject         feverPrefab = null;

    // ���� ���� ������
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

            // 3�ڸ����� ��ǥ�� ��´�.
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

            // 3�ڸ����� ��ǥ�� ��´�.
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

        // 3�ʸ��� �ڵ����� �����Ѵ�.
        StartCoroutine(AutoSave(3.0f));
        
        // 1�� �� �ȳ� �޼����� ������ش�.
        StartCoroutine(WaitAndCall<string>(PrintMessage, "ȭ���� �������� ��ġ�ؼ� ������ ��ƺ�����!", 1.0f));
    }

    private void Update()
    {
        // �� �����ӿ� doughIncrementPerSec ��ŭ�� ������ ������Ų��.
        Dough += doughIncrementPerSec;
        totalDough += doughIncrementPerSec;

        if (IsAllClosed())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

                // Ŭ���� ��ġ�� ClickArea�� �浹�� ��쿡��, �̺�Ʈ�� �߻���Ų��.
                if (hit.collider != null)
                {
                    // ��� ���ʿ��� �������Ǿ�� �ϹǷ�, ���� ���� ���ش�.
                    position.z = -50.0f;

                    if (HasPet(PetType.RED_CAT))
                    {
                        // ���� ���� ���� �����Ѵٸ�, ���׷��� 10% �߰������� ��½�Ų��.
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
        // �ش� Ÿ���� ��ų�� ������ �ִ����� ���� ���θ� �����Ѵ�.
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
        // �ش� Ÿ���� ��ų�� ����������� ���� ���θ� �����Ѵ�.
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

        // ��ų�� �����ϰ� �ְ�, ��������� ���� ��쿡 ����� �� �ֵ��� �Ѵ�.
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

            // ��ų�� ����߱� ������, ���� ����� ��ų �������� ��Ȱ��ȭ ��Ų��.
            skillButtons[skillIndex].gameObject.SetActive(false);
        }
    }

    public bool IsAllClosed()
    {
        // �ɼ�â, ����â, ����â, ����â�� ��� �����ִ��� �˻��Ѵ�.
        return (!OptionButton.IsOpened) && (!AchievementButton.IsOpened) && (!ShopButton.IsOpened) && (!DictButton.IsOpened);
    }

    public void PrintMessage(string textContent)
    {
        // textContent�� �������� �ȳ��޼����� ȭ�鿡 ����Ѵ�.
        messageAnimator.Play("Show", -1, 0.0f);
        messageText.text = textContent;
    }

    private void GenerateEffect(Vector3 position)
    {
        // �� ��ƼŬ ����Ʈ�� ����Ѵ�.
        Instantiate(starPrefab, position, Quaternion.identity, canvasRectTransform);

        // ȹ�� ���� �ؽ�Ʈ ����Ʈ�� ����Ѵ�.
        position.y -= 65.0f;
        GameObject scoreEffect = Instantiate(scorePrefab, position, Quaternion.identity, canvasRectTransform);
        scoreEffect.GetComponent<Text>().text = "+" + doughIncrement.ToString() + "L";

        // �޺� �ؽ�Ʈ ����Ʈ�� ����Ѵ�.
        GameObject comboEffect = Instantiate(comboPrefab, canvasRectTransform);
        Text comboText = comboEffect.GetComponent<Text>();
        Text maxComboText = comboEffect.transform.GetChild(0).GetComponent<Text>();
        comboText.text = currentCombo.ToString() + " COMBO!";
        maxComboText.text = "MAX " + maxCombo.ToString();

        // �����(�丮��)�� �ִϸ��̼� Ʈ���Ÿ� �߻���Ų��.
        catAnimator.SetTrigger("doTouch");

        // Ŭ�� ���带 ����Ѵ�.
        SoundManager.instance.PlaySFX("Click", 0.5f);
    }

    private void GenerateEffectWithPet(Vector3 position)
    {
        GenerateEffect(position);

        // ���� ���� ���� �����ϰ� �ִٸ�, �߰� ���� �ؽ�Ʈ ����Ʈ�� ����Ѵ�.
        position.y -= 120.0f;
        GameObject scoreEffect = Instantiate(scorePrefab, position, Quaternion.identity, canvasRectTransform);
        Text scoreText = scoreEffect.GetComponent<Text>();
        scoreText.text = "+" + (0.1f * doughIncrement).ToString() + "L";
        scoreText.fontSize -= 10;
    }

    public IEnumerator WaitAndCall<T>(Action<T> func, T param, float seconds)
    {
        // seconds�ʵ��� ��ٸ� ��,
        yield return new WaitForSeconds(seconds);

        // �Ű������� �Ѿ�� �Լ��� param�� �Ķ���ͷ� �Ѱ��־� ȣ���Ѵ�.
        // �̶�, Action<>�� ����ϱ� ������, ���� ���� void���� �Ѵ�.
        func(param);
    }

    public IEnumerator Count(GoodsType goods, float target, float current)
    {
        // ���� : https://unitys.tistory.com/7

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

                    // ��ǥ ���� ���� ������ ū ��쿡��, �� ���׷��� ������Ų��.
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

                    // ��ǥ ���� ���� ������ ū ��쿡��, �� ��带 ������Ų��.
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

        // �޺��� FEVER_COMBO�� ����� ���, �ǹ���忡 �����Ѵ�.
        if (currentCombo > 0 && (currentCombo % Constants.FEVER_COMBO) == 0)
        {
            if (HasSkill(SkillType.PLUS_FEVER_TIME))
            {
                // ���� ��, �ǹ���� ���ӽð� ���� ��ų�� ������ �ִٸ� 13�ʵ��� �ǹ���忡 �����Ѵ�.
                UseSkill((int)SkillType.PLUS_FEVER_TIME);
            }
            else
            {
                // ��ų�� ������ ���� �ʴٸ� 8�ʵ��� �ǹ���忡 �����Ѵ�.
                StartCoroutine(UpdateFeverMode(8.0f));
            }
        }

        // �޺��� ������ Ŭ������ COMBO_DURATION�ʵ��� �����ȴ�.
        yield return new WaitForSeconds(Constants.COMBO_DURATION);

        lastCombo += 1;

        // COMBO_DURATION�� ���Ŀ��� ���� �޺����� �ֱ� �޺����� ���ٸ�, �޺�ī��Ʈ�� 0���� �����Ѵ�.
        if (currentCombo == lastCombo)
        {
            // ���� ���� �޺����� �ִ� �޺����� �ɰ��ߴٸ�, �ִ� �޺����� �����Ѵ�.
            if (currentCombo > maxCombo)
            {
                totalMaxCombo = maxCombo = currentCombo;
            }

            // ���� ���Ŀ��� ���� �޺����� �ֱ� �޺����� 0���� �����.
            currentCombo = lastCombo = 0;
        }
    }

    private IEnumerator UpdateFeverMode(float duration)
    {
        isFeverMode = true;

        // �ǹ���� �ؽ�Ʈ ����Ʈ ���
        GameObject feverEffect = Instantiate(feverPrefab, canvasRectTransform);
        Text feverText = feverEffect.transform.GetChild(0).GetComponent<Text>();
        feverText.text = duration + feverText.text;

        // �ǹ���忡 ���Խ�, ȹ�� ���׷��� ȹ�� ��差�� 2�� �����Ѵ�.
        doughIncrement *= 2;
        SellButton.Income *= 2;

        // �� ������ ���������� �����Ѵ�.
        backgroundImage.color = Color.red;

        // ������� �ǹ���� ���� BGM���� �����Ѵ�.
        SoundManager.instance.PlayBGM("FEVER_TIME");

        yield return new WaitForSeconds(duration);

        // ���ӽð� ���Ŀ��� �ٽ� ȹ�� ���׷��� ȹ�� ��差�� ������� �����Ѵ�.
        doughIncrement /= 2;
        SellButton.Income /= 2;

        // ���� ���� ������� ���� �Ŀ� false�� ������־�� ���׷��̵� ���� ����� ����ȴ�.
        isFeverMode = false;

        // �� ������ ������� �����Ѵ�.
        backgroundImage.color = Color.white;

        // ��ų ���ӽð��� ��� �������Ƿ�, ����� ���¸� �����Ѵ�.
        skillStatus = skillStatus & ~Constants.USING_SKILL2;

        // ���� BGM ���� ����Ѵ�.
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

        // ��ų ���ӽð��� ��� �������Ƿ�, ����� ���¸� �����Ѵ�.
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
