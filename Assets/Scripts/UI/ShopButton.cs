using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    // 상점창이 열렸는지에 대한 여부
    private static bool isOpened = false;

    // 0: Whisk, 1: Recipe, 2: Skill, 3: Building, 4: Cat, 5: Pet
    private Image[] categoryButtonImages = null;
    private GameObject[] shopContents = null;

    // 0: Whisk, 1: Recipe
    private Text[] buyButtonTexts = null;

    // 상점 열기/닫기의 애니메이터
    private Animator shopAnimator = null;

    // 카테고리 버튼의 활성/비활성 색상
    private Color activeColor = new Color(240.0f / 255, 200.0f / 255, 150.0f / 255);
    private Color inactiveColor = new Color(220.0f / 255, 220.0f / 255, 220.0f / 255);

    // 휘젓개 아이템 관련 정보
    public static uint whiskLevel = 0;
    private uint[] whiskPrice = new uint[] { 0, 5000, 30000, 180000, 500000, 1000000 };

    // 레시피 아이템 관련 정보
    public static uint recipeLevel = 0;
    private uint[] recipePrice = new uint[] { 0, 5000, 30000, 180000, 500000, 1000000 };

    // 도감 객체
    public DictButton dictionaryInfo = null;

    // 효과 사운드
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

        // 각 컨텐츠의 마지막 자식인 구매 버튼의 텍스트 객체를 가져온다.
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

        // 마지막 자식은 버튼이므로 < 연산자를 이용한다.
        if (whiskLevel < maxWhiskLevel)
        {
            // 구매한 아이템의 도감을 해제한다.
            dictionaryInfo.UnlockWhiskDict(whiskLevel, whiskContent.GetChild((int)whiskLevel).GetComponentsInChildren<Text>());

            // 다음 레벨의 휘젓개를 보여준다.
            whiskContent.GetChild((int)whiskLevel).gameObject.SetActive(false);
            whiskContent.GetChild((int)++whiskLevel).gameObject.SetActive(true);

            // 해당 가격만큼 골드를 차감한 후, 클릭시 반죽량을 증가시킨다.
            StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold - whiskPrice[whiskLevel], GameManager.instance.Gold));
            GameManager.instance.DoughIncrement = (uint)(600.0f * Mathf.Pow(1.5f, whiskLevel));

            // 최고 레벨에 달성하게 되면, 버튼도 지워준다.
            if (whiskLevel == maxWhiskLevel)
            {
                // 마지막 자식인 버튼을 불러와 비활성화 시켜준다.
                whiskContent.GetChild((int)maxWhiskLevel).gameObject.SetActive(false);
            }
            else
            {
                // 그 외의 경우에는 버튼 내의 텍스트(가격)만 변경시킨다.
                buyButtonTexts[0].text = string.Format("{0:#,0}", whiskPrice[whiskLevel + 1]) + "G";
            }

            // 구매하는 사운드를 출력한다.
            SoundManager.instance.PlaySFX("Buy");
        }
    }

    private void UpgradeRecipe()
    {
        Transform recipeContent = shopContents[1].transform;
        uint maxWhiskLevel = (uint)recipeContent.childCount - 1;

        // 마지막 자식은 버튼이므로 < 연산자를 이용한다.
        if (recipeLevel < maxWhiskLevel)
        {
            // 구매한 아이템의 도감을 해제한다.
            dictionaryInfo.UnlockRecipeDict(recipeLevel, recipeContent.GetChild((int)recipeLevel).GetComponentsInChildren<Text>());

            // 다음 레벨의 휘젓개를 보여준다.
            recipeContent.GetChild((int)recipeLevel).gameObject.SetActive(false);
            recipeContent.GetChild((int)++recipeLevel).gameObject.SetActive(true);

            // 해당 가격만큼 골드를 차감한 후, 클릭시 반죽량을 증가시킨다.
            StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold - recipePrice[recipeLevel], GameManager.instance.Gold));
            SellButton.Sales = (uint)(100.0f * Mathf.Pow(2.25f, recipeLevel));
            SellButton.Income = (uint)(100.0f * Mathf.Pow(2.5f, recipeLevel));

            // 최고 레벨에 달성하게 되면, 버튼도 지워준다.
            if (recipeLevel == maxWhiskLevel)
            {
                // 마지막 자식인 버튼을 불러와 비활성화 시켜준다.
                recipeContent.GetChild((int)maxWhiskLevel).gameObject.SetActive(false);
            }
            else
            {
                // 그 외의 경우에는 버튼 내의 텍스트(가격)만 변경시킨다.
                buyButtonTexts[1].text = string.Format("{0:#,0}", recipePrice[recipeLevel + 1]) + "G";
            }

            // 구매하는 사운드를 출력한다.
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
                GameManager.instance.PrintMessage("피버모드 중에는 업그레이드할 수 없어요.");
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
                GameManager.instance.PrintMessage("피버모드 중에는 업그레이드할 수 없어요.");
            }
            else
            {
                UpgradeRecipe();
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
        }
    }
}
