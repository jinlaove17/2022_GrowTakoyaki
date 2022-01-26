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

    // ī�װ� ��ư�� Ȱ��/��Ȱ�� ����
    private Color activeColor = new Color(240.0f / 255, 200.0f / 255, 150.0f / 255);
    private Color inactiveColor = new Color(220.0f / 255, 220.0f / 255, 220.0f / 255);

    // ������ ������ ���� ����
    public static uint whiskLevel = 0;
    private uint[] whiskPrice = new uint[] { 0, 5000, 30000, 180000, 500000, 1000000 };

    // ������ ������ ���� ����
    public static uint recipeLevel = 0;
    private uint[] recipePrice = new uint[] { 0, 5000, 30000, 180000, 500000, 1000000 };

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

    private void Awake()
    {
        GameObject shop = GameObject.Find("Shop");
        GameManager.CheckNull(shop);

        GameObject category = shop.transform.GetChild(1).gameObject;
        GameManager.CheckNull(category);

        categoryButtonImages = new Image[category.transform.childCount];

        for (int i = 0; i < category.transform.childCount; ++i)
        {
            categoryButtonImages[i] = category.transform.GetChild(i).GetComponent<Image>();
            GameManager.CheckNull(categoryButtonImages[i]);
        }

        GameObject contents = shop.transform.GetChild(2).gameObject;
        GameManager.CheckNull(contents);

        shopContents = new GameObject[categoryButtonImages.Length];

        for (int i = 0; i < categoryButtonImages.Length; ++i)
        {
            shopContents[i] = contents.transform.GetChild(i).gameObject;
            GameManager.CheckNull(shopContents[i]);
        }

        buyButtonTexts = new Text[2];

        // �� �������� ������ �ڽ��� ���� ��ư�� �ؽ�Ʈ ��ü�� �����´�.
        for (int i = 0; i < buyButtonTexts.Length; ++i)
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

    private void UpgradeWhisk()
    {
        Transform whiskContent = shopContents[0].transform;
        uint maxWhiskLevel = (uint)whiskContent.childCount - 1;

        // ������ �ڽ��� ��ư�̹Ƿ� < �����ڸ� �̿��Ѵ�.
        if (whiskLevel < maxWhiskLevel)
        {
            // ������ �������� ������ �����Ѵ�.
            dictionaryInfo.UnlockWhiskDict(whiskLevel, whiskContent.GetChild((int)whiskLevel).GetComponentsInChildren<Text>());

            // ���� ������ �������� �����ش�.
            whiskContent.GetChild((int)whiskLevel).gameObject.SetActive(false);
            whiskContent.GetChild((int)++whiskLevel).gameObject.SetActive(true);

            // �ش� ���ݸ�ŭ ��带 ������ ��, Ŭ���� ���׷��� ������Ų��.
            StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold - whiskPrice[whiskLevel], GameManager.instance.Gold));
            GameManager.instance.DoughIncrement = (uint)(600.0f * Mathf.Pow(1.5f, whiskLevel));

            // �ְ� ������ �޼��ϰ� �Ǹ�, ��ư�� �����ش�.
            if (whiskLevel == maxWhiskLevel)
            {
                // ������ �ڽ��� ��ư�� �ҷ��� ��Ȱ��ȭ �����ش�.
                whiskContent.GetChild((int)maxWhiskLevel).gameObject.SetActive(false);
            }
            else
            {
                // �� ���� ��쿡�� ��ư ���� �ؽ�Ʈ(����)�� �����Ų��.
                buyButtonTexts[0].text = string.Format("{0:#,0}", whiskPrice[whiskLevel + 1]) + "G";
            }

            // �����ϴ� ���带 ����Ѵ�.
            SoundManager.instance.PlaySFX("Buy");
        }
    }

    private void UpgradeRecipe()
    {
        Transform recipeContent = shopContents[1].transform;
        uint maxWhiskLevel = (uint)recipeContent.childCount - 1;

        // ������ �ڽ��� ��ư�̹Ƿ� < �����ڸ� �̿��Ѵ�.
        if (recipeLevel < maxWhiskLevel)
        {
            // ������ �������� ������ �����Ѵ�.
            dictionaryInfo.UnlockRecipeDict(recipeLevel, recipeContent.GetChild((int)recipeLevel).GetComponentsInChildren<Text>());

            // ���� ������ �������� �����ش�.
            recipeContent.GetChild((int)recipeLevel).gameObject.SetActive(false);
            recipeContent.GetChild((int)++recipeLevel).gameObject.SetActive(true);

            // �ش� ���ݸ�ŭ ��带 ������ ��, Ŭ���� ���׷��� ������Ų��.
            StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold - recipePrice[recipeLevel], GameManager.instance.Gold));
            SellButton.Sales = (uint)(100.0f * Mathf.Pow(2.25f, recipeLevel));
            SellButton.Income = (uint)(100.0f * Mathf.Pow(2.5f, recipeLevel));

            // �ְ� ������ �޼��ϰ� �Ǹ�, ��ư�� �����ش�.
            if (recipeLevel == maxWhiskLevel)
            {
                // ������ �ڽ��� ��ư�� �ҷ��� ��Ȱ��ȭ �����ش�.
                recipeContent.GetChild((int)maxWhiskLevel).gameObject.SetActive(false);
            }
            else
            {
                // �� ���� ��쿡�� ��ư ���� �ؽ�Ʈ(����)�� �����Ų��.
                buyButtonTexts[1].text = string.Format("{0:#,0}", recipePrice[recipeLevel + 1]) + "G";
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
        if (GameManager.instance.Gold >= whiskPrice[whiskLevel + 1])
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
    }

    public void OnClickBuyRecipeButton()
    {
        if (GameManager.instance.Gold >= recipePrice[recipeLevel + 1])
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
        }
    }
}
