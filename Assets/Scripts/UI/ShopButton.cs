using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    // ����â�� ���ȴ����� ���� ����
    private static bool  isOpened = false;

    // ī�װ�/������ ���� ������(0: Whisk, 1: Recipe, 2: Skill, 3: Building, 4: Cat, 5: Pet)
    private Image[]      categoryButtonImages = null;
    private Transform[]  shopContents = null;

    // �� ���� ������
    private Transform    pets = null;

    // ���� ��ư ���� ������(0: Whisk, 1: Recipe_
    private Text[]       buyButtonTexts = null;

    // ������ �ִϸ��̼� ���� ������
    private Animator     shopAnimator = null;

    // ī�װ� ��ư�� Ȱ��/��Ȱ�� ����
    private Color        activeColor = new Color(240.0f / 255, 200.0f / 255, 150.0f / 255);
    private Color        inactiveColor = new Color(220.0f / 255, 220.0f / 255, 220.0f / 255);

    // ������ ������ ���� ������
    private static uint  whiskLevel = 1;
    private const uint   maxWhiskLevel = 5;
    private uint[]       whiskPrice = new uint[] { 0, 50000, 200000, 1000000, 5000000 };

    // ������ ������ ���� ������
    private static uint  recipeLevel = 1;
    private const uint   maxRecipeLevel = 5;
    private uint[]       recipePrice = new uint[] { 0, 50000, 200000, 1000000, 5000000 };

    // ���� ���� ������
    public DictButton    dictionaryInfo = null;

    // ���� ���� ������
    public AudioClip[]   audioClips = null;

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
        Transform shop = GameObject.Find("Shop").transform;
        SystemManager.CheckNull(shop);

        Transform category = shop.GetChild(1);
        SystemManager.CheckNull(category);

        int childCount = category.childCount;
        categoryButtonImages = new Image[childCount];

        for (int i = 0; i < childCount; ++i)
        {
            categoryButtonImages[i] = category.GetChild(i).GetComponent<Image>();
            SystemManager.CheckNull(categoryButtonImages[i]);
        }

        Transform contents = shop.GetChild(2);
        SystemManager.CheckNull(contents);

        shopContents = new Transform[childCount];

        for (int i = 0; i < childCount; ++i)
        {
            shopContents[i] = contents.GetChild(i);
            SystemManager.CheckNull(shopContents[i]);
        }

        pets = GameObject.Find("Canvas").transform.GetChild(0).GetChild(7);
        SystemManager.CheckNull(pets);

        // �������� ������ �������� ������ �ڽ��� ���� ��ư�� �ؽ�Ʈ ��ü�� �����Ѵ�.(0: Whisk, 1: Recipe)
        buyButtonTexts = new Text[2];
        childCount = buyButtonTexts.Length;

        for (int i = 0; i < childCount; ++i)
        {
            buyButtonTexts[i] = shopContents[i].GetChild(shopContents[i].childCount - 1).GetComponentInChildren<Text>();
            SystemManager.CheckNull(buyButtonTexts[i]);
        }

        shopAnimator = shop.GetComponent<Animator>();
        SystemManager.CheckNull(shopAnimator);

        SystemManager.CheckNull(audioClips);
        SoundManager.instance.RegisterAudioclip("BookSlap", audioClips[0]);
        SoundManager.instance.RegisterAudioclip("BookFlip", audioClips[1]);
        SoundManager.instance.RegisterAudioclip("Buy", audioClips[2]);
        SoundManager.instance.RegisterAudioclip("Meow1", audioClips[3]);
        SoundManager.instance.RegisterAudioclip("Meow2", audioClips[4]);
        SoundManager.instance.RegisterAudioclip("Meow3", audioClips[5]);
    }

    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        // �������� �����͸� �ҷ��´�.
        Transform content = shopContents[0];

        if (whiskLevel > 1)
        {
            // ������ ������ 1���� ũ�ٸ�, ����Ʈ�� Ȱ��ȭ�� 0�� �ڽ��� ��Ȱ��ȭ ��Ų��.
           content.GetChild(0).gameObject.SetActive(false);

            if (whiskLevel == maxWhiskLevel)
            {
                // ���� �̹����� ��½�Ų��.
                content.GetChild((int)(maxWhiskLevel - 1)).gameObject.SetActive(true);

                // ���� �ҷ��� �����Ͱ� �ְ� ������ ���, ������ �ڽ��� ��ư�� ��Ȱ��ȭ ��Ų��.
                content.GetChild((int)maxWhiskLevel).gameObject.SetActive(false);
            }
            else
            {
                // �ְ� ������ �ƴ� ���, �ش� ������ �ش��ϴ� ������ ������ Ȱ��ȭ ��Ų��.
                content.GetChild((int)(whiskLevel - 1)).gameObject.SetActive(true);

                // ��ư�� ǥ�õǴ� �������� ���� ���� ���� ������ �°� �����Ѵ�.
                buyButtonTexts[0].text = string.Format("{0:#,0}", whiskPrice[whiskLevel - 1]) + "G";
            }

            // �ش� ���������� ������ ������Ų��.
            for (int i = 1; i < whiskLevel; ++i)
            {
                dictionaryInfo.UnlockWhiskDict((uint)i, content.GetChild(i - 1).GetComponentsInChildren<Text>());
            }
        }

        // �������� �����͸� �ҷ��´�.
        content = shopContents[1];

        if (recipeLevel > 1)
        {
            // ������ ������ 1���� ũ�ٸ�, ����Ʈ�� Ȱ��ȭ�� 0�� �ڽ��� ��Ȱ��ȭ ��Ų��.
            content.GetChild(0).gameObject.SetActive(false);

            if (recipeLevel == maxRecipeLevel)
            {
                // ���� �̹����� ��½�Ų��.
                content.GetChild((int)(maxRecipeLevel - 1)).gameObject.SetActive(true);

                // ���� �ҷ��� �����Ͱ� �ְ� ������ ���, ������ �ڽ��� ��ư�� ��Ȱ��ȭ ��Ų��.
                content.GetChild((int)maxRecipeLevel).gameObject.SetActive(false);
            }
            else
            {
                // �ְ� ������ �ƴ� ���, �ش� ������ �ش��ϴ� ������ ������ Ȱ��ȭ ��Ų��.
                content.GetChild((int)(recipeLevel - 1)).gameObject.SetActive(true);

                // ��ư�� ǥ�õǴ� �������� ���� ���� ���� ������ �°� �����Ѵ�.
                buyButtonTexts[1].text = string.Format("{0:#,0}", recipePrice[recipeLevel - 1]) + "L";
            }

            // �ش� ���������� ������ ������Ų��.
            for (int i = 1; i < recipeLevel; ++i)
            {
                dictionaryInfo.UnlockRecipeDict((uint)i, content.GetChild(i - 1).GetComponentsInChildren<Text>());
            }
        }

        // ���� �����͸� �ҷ��´�.
        content = shopContents[5].GetChild(0);
        bool isSoldOut = true;
        int enumCount = (int)PetType.ENUM_COUNT;

        // ��� ���� ������ �ִ��� �˻��Ͽ� ������ ���� �����Ǿ����� �Ǵ��Ѵ�.
        for (int i = 0; i < enumCount; ++i)
        {
            if (GameManager.instance.HasPet((PetType)i))
            {
                // ������ ���� ȭ�鿡 ������ �ǵ��� Ȱ��ȭ ��Ų��.
                pets.GetChild(i).gameObject.SetActive(true);

                // ����â���� �ش� �꿡 ���� ������ ��Ȱ��ȭ ��Ų��.
                content.GetChild(i).gameObject.SetActive(false);

                // ������ �����Ѵ�.
                dictionaryInfo.UnlockPetDict((PetType)i, content.GetChild(i).GetComponentsInChildren<Text>());
            }
            else
            {
                isSoldOut = false;
            }
        }
        
        if (isSoldOut)
        {
            // ��� ���� �ȷȴٸ� ���� �̹����� ����Ѵ�.
            content.GetChild(enumCount).gameObject.SetActive(true);
        }
    }

    private void UpgradeWhisk()
    {
        Transform whiskContent = shopContents[0];

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
        Transform recipeContent = shopContents[1];

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
        if (GameManager.instance.IsAllClosed())
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

        if (!shopContents[index].gameObject.activeSelf)
        {
            for (int i = 0; i < categoryButtonImages.Length; ++i)
            {
                if (i == index)
                {
                    categoryButtonImages[i].color = activeColor;
                    shopContents[i].gameObject.SetActive(true);
                }
                else
                {
                    categoryButtonImages[i].color = inactiveColor;
                    shopContents[i].gameObject.SetActive(false);
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

    public void OnClickBuyPetButton(int index)
    {
        PetType petType = (PetType)index;

        if (petType < 0 || petType >= PetType.ENUM_COUNT)
        {
            return;
        }

        const int price = 10000000;

        if (GameManager.instance.Gold >= price)
        {
            StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold - price, GameManager.instance.Gold));

            Transform content = shopContents[5].GetChild(0);
            Transform pet = null;

            // �ش� ���� ������ �����Ѵ�.
            dictionaryInfo.UnlockPetDict(petType, content.GetChild((int)petType).GetComponentsInChildren<Text>());

            switch (petType)
            {
                case PetType.RED_CAT:
                    SoundManager.instance.PlaySFX("Meow1");
                    break;
                case PetType.YELLOW_CAT:
                    SoundManager.instance.PlaySFX("Meow2");
                    GameManager.instance.DoughIncrementPerSec = 10;
                    break;
                case PetType.GREEN_CAT:
                    SoundManager.instance.PlaySFX("Meow3");
                    break;
            }

            GameManager.instance.GetPet(petType);

            // ���� ȭ�鿡 ������ �ǵ��� Ȱ��ȭ ��Ų��.
            pet = pets.GetChild(index);
            pet.gameObject.SetActive(true);

            // ����â���� �ش� �꿡 ���� ������ ��Ȱ��ȭ ��Ų��.
            content.GetChild(index).gameObject.SetActive(false);

            // ��� ���� ������ �ִ��� �˻��Ͽ� ������ ���� �����Ǿ����� �Ǵ��Ѵ�.
            bool isSoldOut = true;
            int enumCount = (int)PetType.ENUM_COUNT;

            for (int i = 0; i < enumCount; ++i)
            {
                if (!GameManager.instance.HasPet((PetType)i))
                {
                    isSoldOut = false;
                    break;
                }
            }

            if (isSoldOut)
            {
                // ��� ���� �ȷȴٸ� ���� �̹����� ����Ѵ�.
                content.GetChild(enumCount).gameObject.SetActive(true);
            }

            // ���� ���带 ����Ѵ�.
            SoundManager.instance.PlaySFX("Buy");
        }
        else
        {
            GameManager.instance.PrintMessage("��尡 ���ڶ��Ф�");
        }
    }
}
