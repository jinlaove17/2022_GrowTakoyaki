using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{
    private static uint level = 1;
    private const uint maxLevel = 10;

    private static uint price = 1000;
    private static Text priceText = null;

    private Button recipeButton = null;

    private List<GameObject> recipeDictInfo = null;
    private string[] recipeName = new string[] {
        "세발 문어", "다섯발 문어", "열발 문어", "매콤알싸한 문어", "먹물 문어",
        "자연산 문어", "단짠단짠 문어", "남극갔다 온 문어", "최고급 대왕 문어", "크라켄 문어"
    };

    private Vector3 particlePosition = Vector3.zero;

    public GameObject particlePrefab = null;
    public GameObject levelUpPrefab = null;

    public static uint Level
    {
        set
        {
            level = value;
            price = 1000 * (uint)Mathf.Pow(2.5f, Level - 1);
            priceText.text = "-" + price.ToString() + "G";
        }

        get
        {
            return level;
        }
    }

    private void Awake()
    {
        recipeButton = GetComponent<Button>();

        if (recipeButton == null)
        {
            Debug.LogError("recipeButton is null!");
        }

        priceText = recipeButton.transform.GetChild(2).GetComponent<Text>();

        if (priceText == null)
        {
            Debug.LogError("recipePriceText is null!");
        }

        GameObject contents = GameObject.Find("Dictionary").transform.GetChild(1).transform.GetChild(0).gameObject;

        if (contents == null)
        {
            Debug.LogError("contents is null!");
        }

        recipeDictInfo = new List<GameObject>();

        for (int i = 0; i < contents.transform.childCount; ++i)
        {
            recipeDictInfo.Add(contents.transform.GetChild(i).transform.gameObject);
        }

        particlePosition = GetComponent<RectTransform>().position;
        particlePosition.z -= 50.0f;
    }

    public void Start()
    {
        for (int i = 0; i <= Level; ++i)
        {
            UnlockRecipeDict(i);
        }
    }

    public void OnClickRecipeButton()
    {
        if (GameManager.instance.Gold >= price && level < maxLevel)
        {
            UpgradeRecipe();
            GenerateEffect();
        }
    }

    private void UpgradeRecipe()
    {
        StartCoroutine(GameManager.instance.Count(GameManager.Goods.GOLD, GameManager.instance.Gold - price, GameManager.instance.Gold));

        SellButton.Income = (uint)(1.75f * SellButton.Income);
        SellButton.Sales = (uint)(2.0f * SellButton.Sales);

        UnlockRecipeDict((int)++Level);

        if (Level == maxLevel)
        {
            recipeButton.enabled = false;
            recipeButton.GetComponent<Image>().color = Color.gray;
            priceText.text = "MAX";
            priceText.color = Color.red;
        }
    }

    private void GenerateEffect()
    {
        // 파티클 이펙트 출력
        Instantiate(particlePrefab, particlePosition, Quaternion.identity, GameManager.instance.canvasRectTransform);

        // 텍스트 이펙트 출력
        GameObject levelUp = Instantiate(levelUpPrefab, transform.position, Quaternion.identity, GameManager.instance.canvasRectTransform);
        levelUp.GetComponent<Text>().text += " " + Level.ToString();

        // 사운드 출력
        SoundManager.instance.PlaySFX("LEVEL_UP");
    }

    private void UnlockRecipeDict(int lv)
    {
        if (lv > 1 && lv <= maxLevel)
        {
            recipeDictInfo[lv - 1].transform.GetChild(0).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            recipeDictInfo[lv - 1].transform.GetChild(1).GetComponent<Text>().text = recipeName[lv - 1];
            recipeDictInfo[lv - 1].transform.GetChild(2).GetComponent<Text>().text = "소요 반죽량 : " + SellButton.Sales + "L\n획득 골드량 : " + SellButton.Income + "G";
        }
    }
}
