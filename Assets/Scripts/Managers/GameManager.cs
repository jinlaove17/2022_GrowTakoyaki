using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ��� �� ����
static class Constants
{
    // ��ų�� ����(����, ����� ����)
    public const int HAS_SKILL1 = 0x00000001;
    public const int HAS_SKILL2 = 0x00000002;
    public const int USING_SKILL1 = 0x00000004;
    public const int USING_SKILL2 = 0x00000008;
}

// ��ų�� ����(�ڵ� �Ǹ�, �ǹ���� �ð� ����)
public enum SkillType
{
    AUTO_SELL,
    PLUS_FEVER_TIME
}

// ��ȭ�� ����(���, ���׷�)
public enum GoodsType
{
    GOLD,
    DOUGH
};

public class GameManager : MonoBehaviour
{
    // �̱��� ����
    public static GameManager instance = null;

    // ������� �ִϸ��̼� ����� ���� �ִϸ�����
    private Animator catAnimator = null;

    // ���� �����ʹ� LoadData() �Լ����� PlayerPrefs�� �̿��Ͽ� �ʱ�ȭ�� �����Ѵ�.
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

    // �ǹ���� ���� ������
    private bool isFeverMode = false;
    private Image backgroundImage = null;

    // ��ų ���� ó���� �ϱ� ���� ������
    private SellButton sellButton = null;
    private Button[] skillButtons = null;
    private int skillStatus = 0x00000000;

    // �÷��̾�� �ȳ��޼����� ����ϱ� ���� ������
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
        // �������� 60���� ������Ų��.
        Application.targetFrameRate = 60;

        // �̱��� ����
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

        // 3�ʸ��� �ڵ����� �����Ѵ�.
        StartCoroutine(AutoSave(3.0f));
        
        // 1�� �� �ȳ� �޼����� ������ش�.
        StartCoroutine(WaitAndCall<string>(PrintMessage, "ȭ���� �������� ��ġ�ؼ� ������ ��ƺ�����!", 1.0f));
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

    // �Ű������� �ڷ����� uint�� ����Ƽ �������� �νĵ��� �ʴ´ٴ� ���� ������
    // ���� �Ű������� �ִ� 1�������� ������ ������
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
        // �� ��ƼŬ ����Ʈ
        position.z = -50.0f;
        Instantiate(starPrefab, position, Quaternion.identity, canvasRectTransform);

        // ȹ�� ���� �ؽ�Ʈ ����Ʈ
        position.y -= 65.0f;
        GameObject scoreEffect = Instantiate(scorePrefab, position, Quaternion.identity, canvasRectTransform);
        scoreEffect.GetComponent<Text>().text = "+" + doughIncrement.ToString() + "L";

        // �޺� �ؽ�Ʈ ����Ʈ
        GameObject comboEffect = Instantiate(comboPrefab, canvasRectTransform);
        Text comboText = comboEffect.GetComponent<Text>();
        Text maxComboText = comboEffect.transform.GetChild(0).GetComponent<Text>();
        comboText.text = currentCombo.ToString() + " COMBO!";
        maxComboText.text = "MAX " + maxCombo.ToString();

        // ������� �ִϸ��̼� Ʈ���� �߻�
        catAnimator.SetTrigger("doTouch");

        // Ŭ������ ���
        SoundManager.instance.PlaySFX("Click", 0.5f);
    }

    public IEnumerator WaitAndCall<T>(Action<T> func, T param, float seconds)
    {
        // seconds�ʵ��� ��ٸ� ��,
        yield return new WaitForSeconds(seconds);

        // �Ű������� �Ѿ�� �Լ��� ȣ���Ѵ�.
        func(param);
    }

    // ���� : https://unitys.tistory.com/7
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

        // 100 �޺� ������ �ǹ���� ����
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

        // �޺��� ������ Ŭ������ 0.65�ʵ��� ����
        yield return new WaitForSeconds(0.65f);

        ++lastCombo;

        // 0.65�� ���Ŀ� ������ �޺����� ���ٸ� �ִ� �޺��� �������� �˻��� �� �޺�ī��Ʈ�� 0���� ����
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
        // �ǹ���忡 �����ߴٰ� ����
        isFeverMode = true;

        GameObject feverEffect = Instantiate(feverPrefab, canvasRectTransform);
        Text feverText = feverEffect.transform.GetChild(0).GetComponent<Text>();
        feverText.text = duration + feverText.text;

        // �ǹ���忡 ���Խ�, 8�ʵ��� ȹ�� ���׷��� ȹ�� ��差 2�� ����
        doughIncrement *= 2;
        SellButton.Income *= 2;

        // �� ������ ���������� ����
        backgroundImage.color = Color.red;

        // �ǹ���� ���� BGM ���� ���
        SoundManager.instance.PlayBGM("FEVER_TIME");

        yield return new WaitForSeconds(duration);

        // ���ӽð� ���Ŀ��� �ٽ� ȹ�� ���׷��� ȹ�� ��差�� ������� ����
        doughIncrement /= 2;
        SellButton.Income /= 2;

        // ���� ���� ������� ���� �Ŀ� false�� ����� ��� ��� ���̿��� ���׷��̵� ����
        isFeverMode = false;

        // �� ������ ������� ����
        backgroundImage.color = Color.white;

        // ��ų ���ð��� ��� �������Ƿ�, ����� ���¸� �����Ѵ�.
        skillStatus = skillStatus & ~Constants.USING_SKILL2;

        // ���� BGM ���� ���
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

        // ��ų ���ð��� ��� �������Ƿ�, ����� ���¸� �����Ѵ�.
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
