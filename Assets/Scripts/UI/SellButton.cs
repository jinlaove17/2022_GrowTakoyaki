using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellButton : MonoBehaviour
{
    // 타코야키 판매시 소모되는 반죽량
    private static uint sales = 0;
    private static Text salesText = null;

    // 타코야키 판매시 획득하는 골드량
    private static uint income = 0;

    private Vector3 particlePosition = Vector3.zero;
    private Vector3 incomeTextPosition = Vector3.zero;

    public GameObject particlePrefab = null;
    public GameObject goldTextPrefab = null;

    // 효과 사운드
    public AudioClip[] audioClips = null;

    public static uint Sales
    {
        set
        {
            sales = value;
            salesText.text = "-" + string.Format("{0:#,0}", sales) + "L";
        }

        get
        {
            return sales;
        }
    }

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

    private void Awake()
    {
        Button sellButton = GetComponent<Button>();
        GameManager.CheckNull(sellButton);

        salesText = sellButton.transform.GetChild(2).GetComponent<Text>();
        GameManager.CheckNull(salesText);

        GameManager.CheckNull(audioClips);
        SoundManager.instance.RegisterAudioclip("SELL", audioClips[0]);

        RectTransform rectTransform = GetComponent<RectTransform>();
        GameManager.CheckNull(rectTransform);

        float halfWidth = 0.5f * rectTransform.rect.width;
        float halfHeight = 0.5f * rectTransform.rect.height;

        particlePosition = incomeTextPosition = new Vector3(rectTransform.position.x + halfWidth, rectTransform.position.y + halfHeight, -50.0f);
        incomeTextPosition.y += 180.0f;
    }

    public void OnClickSellButton()
    {
        if (GameManager.instance.Dough >= Sales)
        {
            StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold + Income, GameManager.instance.Gold));
            StartCoroutine(GameManager.instance.Count(GoodsType.DOUGH, GameManager.instance.Dough - Sales, GameManager.instance.Dough));

            // 판매 파티클 이펙트 출력
            Instantiate(particlePrefab, particlePosition, Quaternion.identity, GameManager.instance.canvasRectTransform);

            // 획득 골드 텍스트 이펙트 출력
            GameObject goldText = Instantiate(goldTextPrefab, incomeTextPosition, Quaternion.identity, GameManager.instance.canvasRectTransform);
            goldText.GetComponent<Text>().text = "+" + Income.ToString() + "G";

            // 판매 사운드 출력
            SoundManager.instance.PlaySFX("SELL");
        }
        else
        {
            GameManager.instance.PrintMessage("반죽량이 모자라요ㅠㅠ");
        }
    }
}
