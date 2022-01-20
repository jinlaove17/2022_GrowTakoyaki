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
    private Vector3 goldTextPosition = Vector3.zero;

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

        particlePosition = goldTextPosition = GetComponent<RectTransform>().position;
        particlePosition.z -= 50.0f;
        goldTextPosition.y += 180.0f;
    }

    public void OnClickSellButton()
    {
        if (GameManager.instance.Dough >= Sales)
        {
            StartCoroutine(GameManager.instance.Count(GameManager.Goods.GOLD, GameManager.instance.Gold + Income, GameManager.instance.Gold));
            StartCoroutine(GameManager.instance.Count(GameManager.Goods.DOUGH, GameManager.instance.Dough - Sales, GameManager.instance.Dough));

            Instantiate(particlePrefab, particlePosition, Quaternion.identity, GameManager.instance.canvasRectTransform);
            
            GameObject goldText = Instantiate(goldTextPrefab, goldTextPosition, Quaternion.identity, GameManager.instance.canvasRectTransform);
            goldText.GetComponent<Text>().text = "+" + Income.ToString() + "G";

            SoundManager.instance.PlaySFX("SELL");
        }
    }
}
