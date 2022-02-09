using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    // 상점창이 열렸는지에 대한 여부
    private static bool  isOpened = false;

    // 카테고리/컨텐츠 관련 데이터(0: Whisk, 1: Recipe, 2: Skill, 3: Building, 4: Cat, 5: Pet)
    private Image[]      categoryButtonImages = null;
    private Transform[]  shopContents = null;

    // 펫 관련 데이터
    private Transform    pets = null;

    // 구매 버튼 관련 데이터(0: Whisk, 1: Recipe_
    private Text[]       buyButtonTexts = null;

    // 상점의 애니메이션 관련 데이터
    private Animator     shopAnimator = null;

    // 카테고리 버튼의 활성/비활성 색상
    private Color        activeColor = new Color(240.0f / 255, 200.0f / 255, 150.0f / 255);
    private Color        inactiveColor = new Color(220.0f / 255, 220.0f / 255, 220.0f / 255);

    // 휘젓개 아이템 관련 데이터
    private static uint  whiskLevel = 1;
    private const uint   maxWhiskLevel = 5;
    private uint[]       whiskPrice = new uint[] { 0, 50000, 200000, 1000000, 5000000 };

    // 레시피 아이템 관련 데이터
    private static uint  recipeLevel = 1;
    private const uint   maxRecipeLevel = 5;
    private uint[]       recipePrice = new uint[] { 0, 50000, 200000, 1000000, 5000000 };

    // 도감 관련 데이터
    public DictButton    dictionaryInfo = null;

    // 사운드 관련 데이터
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

        // 휘젓개와 레시피 컨텐츠의 마지막 자식인 구매 버튼의 텍스트 객체를 저장한다.(0: Whisk, 1: Recipe)
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
        // 휘젓개의 데이터를 불러온다.
        Transform content = shopContents[0];

        if (whiskLevel > 1)
        {
            // 휘젓개 레벨이 1보다 크다면, 디폴트로 활성화된 0번 자식을 비활성화 시킨다.
           content.GetChild(0).gameObject.SetActive(false);

            if (whiskLevel == maxWhiskLevel)
            {
                // 매진 이미지를 출력시킨다.
                content.GetChild((int)(maxWhiskLevel - 1)).gameObject.SetActive(true);

                // 만약 불러온 데이터가 최고 레벨일 경우, 마지막 자식인 버튼도 비활성화 시킨다.
                content.GetChild((int)maxWhiskLevel).gameObject.SetActive(false);
            }
            else
            {
                // 최고 레벨이 아닌 경우, 해당 레벨에 해당하는 휘젓개 정보를 활성화 시킨다.
                content.GetChild((int)(whiskLevel - 1)).gameObject.SetActive(true);

                // 버튼에 표시되는 아이템의 가격 또한 현재 레벨에 맞게 설정한다.
                buyButtonTexts[0].text = string.Format("{0:#,0}", whiskPrice[whiskLevel - 1]) + "G";
            }

            // 해당 레벨전까지 도감을 해제시킨다.
            for (int i = 1; i < whiskLevel; ++i)
            {
                dictionaryInfo.UnlockWhiskDict((uint)i, content.GetChild(i - 1).GetComponentsInChildren<Text>());
            }
        }

        // 레시피의 데이터를 불러온다.
        content = shopContents[1];

        if (recipeLevel > 1)
        {
            // 휘젓개 레벨이 1보다 크다면, 디폴트로 활성화된 0번 자식을 비활성화 시킨다.
            content.GetChild(0).gameObject.SetActive(false);

            if (recipeLevel == maxRecipeLevel)
            {
                // 매진 이미지를 출력시킨다.
                content.GetChild((int)(maxRecipeLevel - 1)).gameObject.SetActive(true);

                // 만약 불러온 데이터가 최고 레벨일 경우, 마지막 자식인 버튼도 비활성화 시킨다.
                content.GetChild((int)maxRecipeLevel).gameObject.SetActive(false);
            }
            else
            {
                // 최고 레벨이 아닌 경우, 해당 레벨에 해당하는 레시피 정보를 활성화 시킨다.
                content.GetChild((int)(recipeLevel - 1)).gameObject.SetActive(true);

                // 버튼에 표시되는 아이템의 가격 또한 현재 레벨에 맞게 설정한다.
                buyButtonTexts[1].text = string.Format("{0:#,0}", recipePrice[recipeLevel - 1]) + "L";
            }

            // 해당 레벨전까지 도감을 해제시킨다.
            for (int i = 1; i < recipeLevel; ++i)
            {
                dictionaryInfo.UnlockRecipeDict((uint)i, content.GetChild(i - 1).GetComponentsInChildren<Text>());
            }
        }

        // 펫의 데이터를 불러온다.
        content = shopContents[5].GetChild(0);
        bool isSoldOut = true;
        int enumCount = (int)PetType.ENUM_COUNT;

        // 모든 펫을 가지고 있는지 검사하여 상점에 펫이 매진되었는지 판단한다.
        for (int i = 0; i < enumCount; ++i)
        {
            if (GameManager.instance.HasPet((PetType)i))
            {
                // 구매한 펫이 화면에 렌더링 되도록 활성화 시킨다.
                pets.GetChild(i).gameObject.SetActive(true);

                // 상점창에서 해당 펫에 대한 내용을 비활성화 시킨다.
                content.GetChild(i).gameObject.SetActive(false);

                // 도감을 해제한다.
                dictionaryInfo.UnlockPetDict((PetType)i, content.GetChild(i).GetComponentsInChildren<Text>());
            }
            else
            {
                isSoldOut = false;
            }
        }
        
        if (isSoldOut)
        {
            // 모든 펫이 팔렸다면 매진 이미지를 출력한다.
            content.GetChild(enumCount).gameObject.SetActive(true);
        }
    }

    private void UpgradeWhisk()
    {
        Transform whiskContent = shopContents[0];

        if (whiskLevel < maxWhiskLevel)
        {
            // 구매한 아이템의 도감을 해제한다.
            dictionaryInfo.UnlockWhiskDict(whiskLevel, whiskContent.GetChild((int)(whiskLevel - 1)).GetComponentsInChildren<Text>());

            // 다음 레벨의 휘젓개를 보여준다.
            whiskContent.GetChild((int)(whiskLevel - 1)).gameObject.SetActive(false);
            whiskContent.GetChild((int)whiskLevel).gameObject.SetActive(true);

            // 해당 가격만큼 골드를 차감한 후, 클릭시 반죽량을 증가시킨다.
            StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold - whiskPrice[whiskLevel], GameManager.instance.Gold));
            GameManager.instance.DoughIncrement = (uint)(500 * Mathf.Pow(2.0f, whiskLevel));

            // 최고 레벨에 달성하게 되면, 버튼도 지워준다.
            if (++whiskLevel == maxWhiskLevel)
            {
                // 매진 이미지를 출력한다.
                whiskContent.GetChild((int)(maxWhiskLevel - 1)).gameObject.SetActive(true);

                // 마지막 자식인 버튼을 불러와 비활성화 시켜준다.
                whiskContent.GetChild((int)(maxWhiskLevel)).gameObject.SetActive(false);
            }
            else
            {
                // 그 외의 경우에는 버튼 내의 텍스트(가격)만 변경시킨다.
                buyButtonTexts[0].text = string.Format("{0:#,0}", whiskPrice[whiskLevel]) + "G";
            }

            // 구매하는 사운드를 출력한다.
            SoundManager.instance.PlaySFX("Buy");
        }
    }

    private void UpgradeRecipe()
    {
        Transform recipeContent = shopContents[1];

        if (recipeLevel < maxRecipeLevel)
        {
            // 구매한 아이템의 도감을 해제한다.
            dictionaryInfo.UnlockRecipeDict(recipeLevel, recipeContent.GetChild((int)(recipeLevel - 1)).GetComponentsInChildren<Text>());

            // 다음 레벨의 레시피를 보여준다.
            recipeContent.GetChild((int)(recipeLevel - 1)).gameObject.SetActive(false);
            recipeContent.GetChild((int)recipeLevel).gameObject.SetActive(true);

            // 해당 가격만큼 골드를 차감한 후, 클릭시 반죽량을 증가시킨다.
            StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold - recipePrice[recipeLevel], GameManager.instance.Gold));

            string[] texts = recipeContent.GetChild((int)recipeLevel - 1).transform.GetChild(2).GetComponent<Text>().text.Split(' ');
            SellButton.Sales = uint.Parse(texts[3].Replace("L\n획득", "").Replace(",", ""));
            SellButton.Income = uint.Parse(texts[texts.Length - 1].Replace("G", "").Replace(",", ""));

            // 최고 레벨에 달성하게 되면, 버튼도 지워준다.
            if (++recipeLevel == maxRecipeLevel)
            {
                // 매진 이미지를 출력한다.
                recipeContent.GetChild((int)(maxRecipeLevel - 1)).gameObject.SetActive(true);

                // 마지막 자식인 버튼을 불러와 비활성화 시켜준다.
                recipeContent.GetChild((int)(maxRecipeLevel)).gameObject.SetActive(false);
            }
            else
            {
                // 그 외의 경우에는 버튼 내의 텍스트(가격)만 변경시킨다.
                buyButtonTexts[1].text = string.Format("{0:#,0}", recipePrice[recipeLevel]) + "G";
            }

            // 구매하는 사운드를 출력한다.
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
                    GameManager.instance.PrintMessage("피버모드 중에는 업그레이드할 수 없어요.");
                }
                else
                {
                    UpgradeWhisk();
                }
            }
            else
            {
                GameManager.instance.PrintMessage("골드가 모자라요ㅠㅠ");
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
                    GameManager.instance.PrintMessage("피버모드 중에는 업그레이드할 수 없어요.");
                }
                else
                {
                    UpgradeRecipe();
                }
            }
            else
            {
                GameManager.instance.PrintMessage("골드가 모자라요ㅠㅠ");
            }
        }
    }

    public void OnClickBuySkill1Button(Button skillButton)
    {
        // 자동 판매 스킬
        if (GameManager.instance.MaxCombo >= 30)
        {
            // 스킬을 구매하지 않은 경우에만 구매할 수 있도록 하고, 스킬의 지속시간이 끝났을 때 false로 바꿔준다.
            if (!GameManager.instance.HasSkill(SkillType.AUTO_SELL))
            {
                skillButton.gameObject.SetActive(true);

                // 해당 값만큼 최대 콤보수를 감소시키고, 스킬을 구매했다고 게임 매니저에 알려준다.
                GameManager.instance.MaxCombo -= 30;
                GameManager.instance.GetSkill(SkillType.AUTO_SELL);

                // 구매하는 사운드를 출력한다.
                SoundManager.instance.PlaySFX("Buy");
            }
            else
            {
                GameManager.instance.PrintMessage("이미 스킬을 가지고 계시군요!");
            }
        }
        else
        {
            GameManager.instance.PrintMessage("최대 콤보가 모자라요ㅠㅠ(현재 : " + GameManager.instance.MaxCombo + ")");
        }
    }

    public void OnClickBuySkill2Button(Button skillButton)
    {
        // 피버모드 시간 증가 스킬
        if (GameManager.instance.MaxCombo >= 10)
        {
            // 스킬을 구매하지 않은 경우에만 구매할 수 있도록 하고, 스킬의 지속시간이 끝났을 때 false로 바꿔준다.
            if (!GameManager.instance.HasSkill(SkillType.PLUS_FEVER_TIME))
            {
                skillButton.gameObject.SetActive(true);

                // 해당 값만큼 최대 콤보수를 감소시키고, 스킬을 구매했다고 게임 매니저에 알려준다.
                GameManager.instance.MaxCombo -= 10;
                GameManager.instance.GetSkill(SkillType.PLUS_FEVER_TIME);

                // 구매하는 사운드를 출력한다.
                SoundManager.instance.PlaySFX("Buy");
            }
            else
            {
                GameManager.instance.PrintMessage("이미 스킬을 가지고 계시군요!");
            }
        }
        else
        {
            GameManager.instance.PrintMessage("최대 콤보가 모자라요ㅠㅠ(현재 : " + GameManager.instance.MaxCombo + ")");
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

            // 해당 펫의 도감을 해제한다.
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

            // 펫이 화면에 렌더링 되도록 활성화 시킨다.
            pet = pets.GetChild(index);
            pet.gameObject.SetActive(true);

            // 상점창에서 해당 펫에 대한 내용을 비활성화 시킨다.
            content.GetChild(index).gameObject.SetActive(false);

            // 모든 펫을 가지고 있는지 검사하여 상점에 펫이 매진되었는지 판단한다.
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
                // 모든 펫이 팔렸다면 매진 이미지를 출력한다.
                content.GetChild(enumCount).gameObject.SetActive(true);
            }

            // 구매 사운드를 출력한다.
            SoundManager.instance.PlaySFX("Buy");
        }
        else
        {
            GameManager.instance.PrintMessage("골드가 모자라요ㅠㅠ");
        }
    }
}
