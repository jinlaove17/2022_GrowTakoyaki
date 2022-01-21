using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhiskButton : MonoBehaviour
{
    private static uint level = 1;
    private const uint maxLevel = 10;

    private static uint price = 1000;
    private static Text priceText = null;

    private Button whiskButton = null;

    private List<GameObject> whiskDictInfo = null;
    private string[] whiskName = new string[] { 
        "≥™π´ »÷¡£∞≥", "∞°¡§øÎ »÷¡£∞≥", "±›π⁄¿Ã »÷¡£∞≥", "4¥‹∞Ë »÷¡£∞≥", "5¥‹∞Ë »÷¡£∞≥",
        "6¥‹∞Ë »÷¡£∞≥", "7¥‹∞Ë »÷¡£∞≥", "8¥‹∞Ë »÷¡£∞≥", "9¥‹∞Ë »÷¡£∞≥", "10¥‹∞Ë »÷¡£∞≥",
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
        whiskButton = GetComponent<Button>();

        if (whiskButton == null)
        {
            Debug.LogError("whiskButton is null!");
        }

        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("audioSource is null!");
        }

        SoundManager.instance.RegisterAudioclip("LEVEL_UP", audioSource.clip);

        priceText = whiskButton.transform.GetChild(2).GetComponent<Text>();

        if (priceText == null)
        {
            Debug.LogError("priceText is null!");
        }

        GameObject contents = GameObject.Find("Dictionary").transform.GetChild(0).transform.GetChild(0).gameObject;

        if (contents == null)
        {
            Debug.LogError("contents is null!");
        }

        whiskDictInfo = new List<GameObject>();

        for (int i = 0; i < contents.transform.childCount; ++i)
        {
            whiskDictInfo.Add(contents.transform.GetChild(i).transform.gameObject);
        }

        RectTransform rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("rectTransform is null!");
        }

        float halfWidth = 0.5f * rectTransform.rect.width;
        float halfHeight = 0.5f * rectTransform.rect.height;

        particlePosition = new Vector3(rectTransform.position.x + halfWidth, rectTransform.position.y + halfHeight, -50.0f);
    }

    public void Start()
    {
        for (int i = 0; i <= Level; ++i)
        {
            UnlockWhiskDict(i);
        }
    }

    public void OnClickWhiskButton()
    {
        if (GameManager.instance.Gold >= price && level < maxLevel)
        {
            UpgradeWhisk();
            GenerateEffect();
        }
    }

    private void UpgradeWhisk()
    {
        StartCoroutine(GameManager.instance.Count(GameManager.Goods.GOLD, GameManager.instance.Gold - price, GameManager.instance.Gold));

        GameManager.instance.DoughIncrement = (uint)(1.5f * GameManager.instance.DoughIncrement);

        UnlockWhiskDict((int)++Level);

        if (Level == maxLevel)
        {
            whiskButton.enabled = false;
            whiskButton.GetComponent<Image>().color = Color.gray;
            priceText.text = "MAX";
            priceText.color = Color.red;
        }
    }

    private void GenerateEffect()
    {
        // ∆ƒ∆º≈¨ ¿Ã∆Â∆Æ √‚∑¬
        Instantiate(particlePrefab, particlePosition, Quaternion.identity, GameManager.instance.canvasRectTransform);

        // ≈ÿΩ∫∆Æ ¿Ã∆Â∆Æ √‚∑¬
        GameObject levelUpEffect = Instantiate(levelUpPrefab, GameManager.instance.canvasRectTransform);
        levelUpEffect.GetComponent<Text>().text += " " + Level.ToString();

        // ªÁøÓµÂ √‚∑¬
        SoundManager.instance.PlaySFX("LEVEL_UP");
    }

    private void UnlockWhiskDict(int lv)
    {
        if (lv > 1 && lv <= maxLevel)
        {
            whiskDictInfo[lv - 1].transform.GetChild(0).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            whiskDictInfo[lv - 1].transform.GetChild(1).GetComponent<Text>().text = whiskName[lv - 1];
            whiskDictInfo[lv - 1].transform.GetChild(2).GetComponent<Text>().text = "≈¨∏ØΩ√ »πµÊ π›¡◊∑Æ " + (1.5f * (lv - 1)).ToString() + "πË ¡ı∞°";
        }
    }
}
