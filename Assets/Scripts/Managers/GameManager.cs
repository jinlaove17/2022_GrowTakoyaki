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
    private Text goldText = null;

    private uint dough = 0;
    private Text doughText = null;
    private uint doughIncrement = 0;

    private uint currentCombo = 0;
    private uint lastCombo = 0;
    private uint maxCombo = 0;

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

        // 1ȸ �ȳ� �޼����� ������ش�.
        PrintMessage("ȭ���� �������� ��ġ�ؼ� ������ ��ƺ�����!");

        // Auto Save
        //InvokeRepeating("SaveData", 0.0f, 3.0f);
    }

    private void Update()
    {
        Dough += 1;

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
                }
            }
        }
    }

    public static void CheckNull(Object obj)
    {
        if (!obj)
        {
            Debug.LogError(obj.name + "is null!");
        }
    }

    public static void CheckNull(Object[] objs)
    {
        foreach (Object obj in objs)
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

    // ���� : https://unitys.tistory.com/7
    public IEnumerator Count(GoodsType goods, float target, float current)
    {
        float duration = 0.08f;
        float offset;

        switch (goods)
        {
            case GoodsType.DOUGH:
                offset = (target - dough) / duration;

                if (offset >= 0.0f)
                {
                    while (current < target)
                    {
                        current += offset * Time.deltaTime;
                        doughText.text = string.Format("{0:#,0}", current);

                        yield return null;
                    }
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
                offset = (target - gold) / duration;

                if (offset >= 0.0f)
                {
                    while (current < target)
                    {
                        current += offset * Time.deltaTime;
                        goldText.text = string.Format("{0:#,0}", current);

                        yield return null;
                    }
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
                maxCombo = currentCombo;
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

    public void SaveData()
    {
        // GameManager Data
        PlayerPrefs.SetInt("GOLD", (int)gold);
        PlayerPrefs.SetInt("DOUGH", (int)dough);
        PlayerPrefs.SetInt("DOUGH_INCREMENT", (int)doughIncrement);
        PlayerPrefs.SetInt("MAX_COMBO", (int)maxCombo);

        // Sell Button Data
        PlayerPrefs.SetInt("INCOME", (int)SellButton.Income);
        PlayerPrefs.SetInt("SALES", (int)SellButton.Sales);

        // Whisk Button Data
        //PlayerPrefs.SetInt("WHISK_LEVEL", (int)WhiskButton.Level);

        // Recipe Button Data
        //PlayerPrefs.SetInt("RECIPE_LEVEL", (int)RecipeButton.Level);
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("GOLD"))
        {
            Gold = (uint)PlayerPrefs.GetInt("GOLD");
        }
        else
        {
            Gold = 10000000;
        }

        if (PlayerPrefs.HasKey("DOUGH"))
        {
            Dough = (uint)PlayerPrefs.GetInt("DOUGH");
        }
        else
        {
            Dough = 10000000;
        }

        if (PlayerPrefs.HasKey("DOUGH_INCREMENT"))
        {
            DoughIncrement = (uint)PlayerPrefs.GetInt("DOUGH_INCREMENT");
        }
        else
        {
            DoughIncrement = 500;
        }

        if (PlayerPrefs.HasKey("MAX_COMBO"))
        {
            maxCombo = (uint)PlayerPrefs.GetInt("MAX_COMBO");
        }
        else
        {
            maxCombo = 0;
        }

        if (PlayerPrefs.HasKey("INCOME"))
        {
            SellButton.Income = (uint)PlayerPrefs.GetInt("INCOME");
        }
        else
        {
            SellButton.Income = 100;
        }

        if (PlayerPrefs.HasKey("SALES"))
        {
            SellButton.Sales = (uint)PlayerPrefs.GetInt("SALES");
        }
        else
        {
            SellButton.Sales = 100;
        }

        if (PlayerPrefs.HasKey("WHISK_LEVEL"))
        {
            //WhiskButton.Level = (uint)PlayerPrefs.GetInt("WHISK_LEVEL");
        }
        else
        {
            //WhiskButton.Level = 1;
        }

        if (PlayerPrefs.HasKey("RECIPE_LEVEL"))
        {
            //RecipeButton.Level = (uint)PlayerPrefs.GetInt("RECIPE_LEVEL");
        }
        else
        {
            //RecipeButton.Level = 1;
        }
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }

    private void OnApplicationQuit()
    {
        //SaveData();
        //ResetData();
    }
}
