using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    // ����â�� ���ȴ����� ���� ����
    private static bool isOpened = false;

    // 0: Whisk, 1: Recipe, 2: Skill, 3: Building, 4: Cat, 5: Pet
    private Image[] categoryButtonImages = null;
    private GameObject[] shopContents = null;

    // 0: Whisk, 1: Recipe
    private Text[] buyButtonTexts = null;

    // ���� ����/�ݱ��� �ִϸ�����
    private Animator shopAnimator = null;

    // ī�װ��� ��ư�� Ȱ��/��Ȱ�� ����
    private Color activeColor = new Color(240.0f / 255, 200.0f / 255, 150.0f / 255);
    private Color inactiveColor = new Color(220.0f / 255, 220.0f / 255, 220.0f / 255);

    // ������ ������ ���� ����
    private static uint whiskLevel = 1;
    private const uint maxWhiskLevel = 5;
    private uint[] whiskPrice = new uint[] { 0, 50000, 200000, 1000000, 5000000 };

    // ������ ������ ���� ����
    private static uint recipeLevel = 1;
    private const uint maxRecipeLevel = 5;
    private uint[] recipePrice = new uint[] { 0, 50000, 200000, 1000000, 5000000 };

    // ���� ��ü
    public DictButton dictionaryInfo = null;

    // ȿ�� ����
    public AudioClip[] audioClips = null;

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

    public static uint WhiskLevel
    {
        set
        {
            whiskLevel = value;
        }

        get
        {
            return whiskLevel;
        }
    }

    public static uint RecipeLevel
    {
        set
        {
            recipeLevel = value;
        }

        get
        {
            return recipeLevel;
        }
    }

    private void Awake()
    {
        GameObject shop = GameObject.Find("Shop");
        GameManager.CheckNull(shop);

        GameObject category = shop.transform.GetChild(1).gameObject;
        GameManager.CheckNull(category);

        int childCount = category.transform.childCount;
        categoryButtonImages = new Image[childCount];

        for (int i = 0; i < childCount; ++i)
        {
            categoryButtonImages[i] = category.transform.GetChild(i).GetComponent<Image>();
            GameManager.CheckNull(categoryButtonImages[i]);
        }

        GameObject contents = shop.transform.GetChild(2).gameObject;
        GameManager.CheckNull(contents);

        shopContents = new GameObject[childCount];

        for (int i = 0; i < childCount; ++i)
        {
            shopContents[i] = contents.transform.GetChild(i).gameObject;
            GameManager.CheckNull(shopContents[i]);
        }

        // �������� ������ �������� ������ �ڽ��� ���� ��ư�� �ؽ�Ʈ ��ü�� �����Ѵ�.
        // 0: Whisk, 1: Recipe
        buyButtonTexts = new Text[2];

        childCount = buyButtonTexts.Length;
        for (int i = 0; i < childCount; ++i)
        {
            buyButtonTexts[i] = shopContents[i].transform.GetChild(shopContents[i].transform.childCount - 1).GetComponentInChildren<Text>();
            GameManager.CheckNull(buyButtonTexts[i]);
        }

        shopAnimator = shop.GetComponent<Animator>();
        GameManager.CheckNull(shopAnimator);

        GameManager.CheckNull(audioClips);
        SoundManager.instance.RegisterAudioclip("BookSlap", audioClips[0]);
        SoundManager.instance.RegisterAudioclip("BookFlip", audioClips[1]);
        SoundManager.instance.RegisterAudioclip("Buy", audioClips[2]);
    }

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        Transform whiskContent = shopContents[0].transform;

        if (whiskLevel > 1)
        {
            // ������ ������ 1���� ũ�ٸ�, ����Ʈ�� Ȱ��ȭ�� 0�� �ڽ��� ��Ȱ��ȭ ��Ų��.
            whiskContent.GetChild(0).gameObject.SetActive(false);

            if (whiskLevel == maxWhiskLevel)
            {
                // ���� �̹����� ��½�Ų��.
                whiskContent.GetChild((int)(maxWhiskLevel - 1)).gameObject.SetActive(true);

                // ���� �ҷ��� �����Ͱ� �ְ� ������ ���, ������ �ڽ��� ��ư�� ��Ȱ��ȭ ��Ų��.
                whiskContent.GetChild((int)maxWhiskLevel).gameObject.SetActive(false);
            }
            else
            {
                // �ְ� ������ �ƴ� ���, �ش� ������ �ش��ϴ� ������ ������ Ȱ��ȭ ��Ų��.
                whiskContent.GetChild((int)(whiskLevel - 1)).gameObject.SetActive(true);

                // ��ư�� ǥ�õǴ� �������� ���� ���� ���� ������ �°� �����Ѵ�.
                buyButtonTexts[0].text = string.Format("{0:#,0}", whiskPrice[whiskLevel - 1]) + "G";
            }

            // �ش� ���������� ������ ������Ų��.
            for (int i = 1; i < whiskLevel; ++i)
            {
                dictionaryInfo.UnlockWhiskDict((uint)i, whiskContent.GetChild(i - 1).GetComponentsInChildren<Text>());
            }
        }

        Transform recipeContent = shopContents[1].transform;

        if (recipeLevel > 1)
        {
            // ������ ������ 1���� ũ�ٸ�, ����Ʈ�� Ȱ��ȭ�� 0�� �ڽ��� ��Ȱ��ȭ ��Ų��.
            recipeContent.GetChild(0).gameObject.SetActive(false);

            if (recipeLevel == maxRecipeLevel)
            {
                // ���� �̹����� ��½�Ų��.
                recipeContent.GetChild((int)(maxRecipeLevel - 1)).gameObject.SetActive(true);

                // ���� �ҷ��� �����Ͱ� �ְ� ������ ���, ������ �ڽ��� ��ư�� ��Ȱ��ȭ ��Ų��.
                recipeContent.GetChild((int)maxRecipeLevel).gameObject.SetActive(false);
            }
            else
            {
                // �ְ� ������ �ƴ� ���, �ش� ������ �ش��ϴ� ������ ������ Ȱ��ȭ ��Ų��.
                recipeContent.GetChild((int)(recipeLevel - 1)).gameObject.SetActive(true);

                // ��ư�� ǥ�õǴ� �������� ���� ���� ���� ������ �°� �����Ѵ�.
                buyButtonTexts[1].text = string.Format("{0:#,0}", recipePrice[recipeLevel - 1]) + "L";
            }

            // �ش� ���������� ������ ������Ų��.
            for (int i = 1; i < recipeLevel; ++i)
            {
                dictionaryInfo.UnlockRecipeDict((uint)i, recipeContent.GetChild(i - 1).GetComponentsInChildren<Text>());
            }
        }
    }

    private void UpgradeWhisk()
    {
        Transform whiskContent = shopContents[0].transform;

        if (whiskLevel < maxWhiskLevel)
        {
            // ������ �������� ������ �����Ѵ�.
            dictionaryInfo.UnlockWhiskDict(whiskLevel, whiskContent.GetChild((int)(whiskLevel - 1)).GetComponentsInChildren<Text>());

            // ���� ������ �������� �����ش�.
            whiskContent.GetChild((int)(whiskLevel - 1)).gameObject.SetActive(false);
            whiskContent.GetChild((int)whiskLevel).gameObject.SetActive(true);

            // �ش� ���ݸ�ŭ ��带 ������ ��, Ŭ���� ���׷��� ������Ų��.
            StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold - whiskPrice[whiskLevel], GameManager.instance.Gold));
            GameManager.instance.DoughIncrement = (uint)(500 * Mathf.Pow(2.0f, whiskLevel));

            // �ְ� ������ �޼��ϰ� �Ǹ�, ��ư�� �����ش�.
            if (++whiskLevel == maxWhiskLevel)
            {
                // ���� �̹����� ����Ѵ�.
                whiskContent.GetChild((int)(maxWhiskLevel - 1)).gameObject.SetActive(true);

                // ������ �ڽ��� ��ư�� �ҷ��� ��Ȱ��ȭ �����ش�.
                whiskContent.GetChild((int)(maxWhiskLevel)).gameObject.SetActive(false);
            }
            else
            {
                // �� ���� ��쿡�� ��ư ���� �ؽ�Ʈ(����)�� �����Ų��.
                buyButtonTexts[0].text = string.Format("{0:#,0}", whiskPrice[whiskLevel]) + "G";
            }

            // �����ϴ� ���带 ����Ѵ�.
            SoundManager.instance.PlaySFX("Buy");
        }
    }

    private void UpgradeRecipe()
    {
        Transform recipeContent = shopContents[1].transform;

        if (recipeLevel < maxRecipeLevel)
        {
            // ������ �������� ������ �����Ѵ�.
            dictionaryInfo.UnlockRecipeDict(recipeLevel, recipeContent.GetChild((int)(recipeLevel - 1)).GetComponentsInChildren<Text>());

            // ���� ������ �����Ǹ� �����ش�.
            recipeContent.GetChild((int)(recipeLevel - 1)).gameObject.SetActive(false);
            recipeContent.GetChild((int)recipeLevel).gameObject.SetActive(true);

            // �ش� ���ݸ�ŭ ��带 ������ ��, Ŭ���� ���׷��� ������Ų��.
            StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold - recipePrice[recipeLevel], GameManager.instance.Gold));

            string[] texts = recipeContent.GetChild((int)recipeLevel - 1).transform.GetChild(2).GetComponent<Text>().text.Split(' ');
            SellButton.Sales = uint.Parse(texts[3].Replace("L\nȹ��", "").Replace(",", ""));
            SellButton.Income = uint.Parse(texts[texts.Length - 1].Replace("G", "").Replace(",", ""));

            // �ְ� ������ �޼��ϰ� �Ǹ�, ��ư�� �����ش�.
            if (++recipeLevel == maxRecipeLevel)
            {
                // ���� �̹����� ����Ѵ�.
                recipeContent.GetChild((int)(maxRecipeLevel - 1)).gameObject.SetActive(true);

                // ������ �ڽ��� ��ư�� �ҷ��� ��Ȱ��ȭ �����ش�.
                recipeContent.GetChild((int)(maxRecipeLevel)).gameObject.SetActive(false);
            }
            else
            {
                // �� ���� ��쿡�� ��ư ���� �ؽ�Ʈ(����)�� �����Ų��.
                buyButtonTexts[1].text = string.Format("{0:#,0}", recipePrice[recipeLevel]) + "G";
            }

            // �����ϴ� ���带 ����Ѵ�.
            SoundManager.instance.PlaySFX("Buy");
        }
    }

    public void OnClickShopButton()
    {
        if (GameManager.instance.IsClosed())
        {
            isOpened = true;
            shopAnimator.SetTrigger("doShow");
            SoundManager.instance.PlaySFX("BookSlap");
        }
    }

    public void OnClickExitShopButton()
    {
        if (isOpened)
        {
            IsOpened = false;
            shopAnimator.SetTrigger("doHide");
        }
    }

    public void OnClickCategoryButton(int index)
    {
        if (index < 0 || index > categoryButtonImages.Length)
        {
            return;
        }

        if (!shopContents[index].activeSelf)
        {
            for (int i = 0; i < categoryButtonImages.Length; ++i)
            {
                if (i == index)
                {
                    categoryButtonImages[i].color = activeColor;
                    shopContents[i].SetActive(true);
                }
                else
                {
                    categoryButtonImages[i].color = inactiveColor;
                    shopContents[i].SetActive(false);
                }
            }

            SoundManager.instance.PlaySFX("BookFlip");
        }
    }

    public void OnClickBuyWhiskButton()
    {
        if (whiskLevel < maxWhiskLevel)
        {
            if (GameManager.instance.Gold >= whiskPrice[whiskLevel])
            {
                if (GameManager.instance.IsFeverMode)
                {
                    GameManager.instance.PrintMessage("�ǹ���� �߿��� ���׷��̵��� �� �����.");
                }
                else
                {
                    UpgradeWhisk();
                }
            }
            else
            {
                GameManager.instance.PrintMessage("��尡 ���ڶ��Ф�");
            }
        }
    }

    public void OnClickBuyRecipeButton()
    {
        if (recipeLevel < maxRecipeLevel)
        {
            if (GameManager.instance.Gold >= recipePrice[recipeLevel])
            {
                if (GameManager.instance.IsFeverMode)
                {
                    GameManager.instance.PrintMessage("�ǹ���� �߿��� ���׷��̵��� �� �����.");
                }
                else
                {
                    UpgradeRecipe();
                }
            }
            else
            {
                GameManager.instance.PrintMessage("��尡 ���ڶ��Ф�");
            }
        }
    }

    public void OnClickBuySkill1Button(Button skillButton)
    {
        // �ڵ� �Ǹ� ��ų
        if (GameManager.instance.MaxCombo >= 30)
        {
            // ��ų�� �������� ���� ��쿡�� ������ �� �ֵ��� �ϰ�, ��ų�� ���ӽð��� ������ �� false�� �ٲ��ش�.
            if (!GameManager.instance.HasSkill(SkillType.AUTO_SELL))
            {
                skillButton.gameObject.SetActive(true);

                // �ش� ����ŭ �ִ� �޺����� ���ҽ�Ű��, ��ų�� �����ߴٰ� ���� �Ŵ����� �˷��ش�.
                GameManager.instance.MaxCombo -= 30;
                GameManager.instance.GetSkill(SkillType.AUTO_SELL);

                // �����ϴ� ���带 ����Ѵ�.
                SoundManager.instance.PlaySFX("Buy");
            }
            else
            {
                GameManager.instance.PrintMessage("�̹� ��ų�� ������ ��ñ���!");
            }
        }
        else
        {
            GameManager.instance.PrintMessage("�ִ� �޺��� ���ڶ��Ф�(���� : " + GameManager.instance.MaxCombo + ")");
        }
    }

    public void OnClickBuySkill2Button(Button skillButton)
    {
        // �ǹ���� �ð� ���� ��ų
        if (GameManager.instance.MaxCombo >= 10)
        {
            // ��ų�� �������� ���� ��쿡�� ������ �� �ֵ��� �ϰ�, ��ų�� ���ӽð��� ������ �� false�� �ٲ��ش�.
            if (!GameManager.instance.HasSkill(SkillType.PLUS_FEVER_TIME))
            {
                skillButton.gameObject.SetActive(true);

                // �ش� ����ŭ �ִ� �޺����� ���ҽ�Ű��, ��ų�� �����ߴٰ� ���� �Ŵ����� �˷��ش�.
                GameManager.instance.MaxCombo -= 10;
                GameManager.instance.GetSkill(SkillType.PLUS_FEVER_TIME);

                // �����ϴ� ���带 ����Ѵ�.
                SoundManager.instance.PlaySFX("Buy");
            }
            else
            {
                GameManager.instance.PrintMessage("�̹� ��ų�� ������ ��ñ���!");
            }
        }
        else
        {
            GameManager.instance.PrintMessage("�ִ� �޺��� ���ڶ��Ф�(���� : " + GameManager.instance.MaxCombo + ")");
        }
    }
}