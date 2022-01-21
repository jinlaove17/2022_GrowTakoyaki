using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellButton : MonoBehaviour
{
    private static uint income = 100;
    private static uint sales = 100;
    private static Text salesText = null;

    private Button sellButton = null;

    private Vector3 particlePosition = Vector3.zero;
    private Vector3 incomeTextPosition = Vector3.zero;

    public GameObject particlePrefab = null;
    public GameObject goldTextPrefab = null;

    public static uint Income
    {
        set
        {
            income = value;
        }

        get
        {
            return income;
        }
    }

    public static uint Sales
    {
        set
        {
            sales = value;
            salesText.text = "-" + sales.ToString() + "L";
        }

        get
        {
            return sales;
        }
    }

    private void Awake()
    {
        sellButton = GetComponent<Button>();

        if (sellButton == null)
        {
            Debug.LogError("sellButton is null!");
        }

        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("audioSource is null!");
        }

        SoundManager.instance.RegisterAudioclip("SELL", audioSource.clip);

        salesText = sellButton.transform.GetChild(2).GetComponent<Text>();

        if (salesText == null)
        {
            Debug.LogError("salesText is null!");
        }

        RectTransform rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("rectTransform is null!");
        }

        float halfWidth = 0.5f * rectTransform.rect.width;
        float halfHeight = 0.5f * rectTransform.rect.height;

        particlePosition = incomeTextPosition = new Vector3(rectTransform.position.x + halfWidth, rectTransform.position.y + halfHeight, -50.0f);
        incomeTextPosition.y += 180.0f;
    }

    public void OnClickSellButton()
    {
        if (GameManager.instance.Dough >= Sales)
        {
            StartCoroutine(GameManager.instance.Count(GameManager.Goods.GOLD, GameManager.instance.Gold + Income, GameManager.instance.Gold));
            StartCoroutine(GameManager.instance.Count(GameManager.Goods.DOUGH, GameManager.instance.Dough - Sales, GameManager.instance.Dough));

            Instantiate(particlePrefab, particlePosition, Quaternion.identity, GameManager.instance.canvasRectTransform);
            
            GameObject goldText = Instantiate(goldTextPrefab, incomeTextPosition, Quaternion.identity, GameManager.instance.canvasRectTransform);
            goldText.GetComponent<Text>().text = "+" + Income.ToString() + "G";

            SoundManager.instance.PlaySFX("SELL");
        }
    }
}
