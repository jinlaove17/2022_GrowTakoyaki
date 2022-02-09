using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellButton : MonoBehaviour
{
    // 판매시의 소모 반죽량 관련 데이터
    private static uint sales = 0;
    private static Text salesText = null;

    // 판매시의 획득 골드 관련 데이터
    private static uint income = 0;

    // 이펙트의 위치 데이터
    private Vector3     particlePosition = Vector3.zero;
    private Vector3     incomeTextPosition = Vector3.zero;
    private Vector3     petIncomeTextPosition = Vector3.zero;

    // 프리팝 데이터
    public GameObject   particlePrefab = null;
    public GameObject   goldTextPrefab = null;

    // 사운드 관련 데이터
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
        SystemManager.CheckNull(sellButton);

        salesText = sellButton.transform.GetChild(2).GetComponent<Text>();
        SystemManager.CheckNull(salesText);

        SystemManager.CheckNull(audioClips);
        SoundManager.instance.RegisterAudioclip("SELL", audioClips[0]);

        RectTransform rectTransform = GetComponent<RectTransform>();
        SystemManager.CheckNull(rectTransform);

        float halfWidth = 0.5f * rectTransform.rect.width;
        float halfHeight = 0.5f * rectTransform.rect.height;

        particlePosition = incomeTextPosition = new Vector3(rectTransform.position.x + halfWidth, rectTransform.position.y + halfHeight, -50.0f);
        incomeTextPosition.y += 180.0f;
        petIncomeTextPosition = incomeTextPosition;
        petIncomeTextPosition.y += 50.0f;
    }

    private void GenerateEffect()
    {
        // 판매 파티클 이펙트 출력
        Instantiate(particlePrefab, particlePosition, Quaternion.identity, GameManager.instance.canvasRectTransform);

        // 획득 골드 텍스트 이펙트 출력
        GameObject goldEffect = Instantiate(goldTextPrefab, incomeTextPosition, Quaternion.identity, GameManager.instance.canvasRectTransform);
        goldEffect.GetComponent<Text>().text = "+" + Income.ToString() + "G";

        // 판매 사운드 출력
        SoundManager.instance.PlaySFX("SELL");
    }

    private void GenerateEffectWithPet()
    {
        GenerateEffect();

        // 게으른 냥이 펫을 소유하고 있다면, 추가적인 골드 텍스트 이펙트를 출력한다.
        GameObject goldEffect = Instantiate(goldTextPrefab, petIncomeTextPosition, Quaternion.identity, GameManager.instance.canvasRectTransform);
        Text goldText = goldEffect.GetComponent<Text>();
        goldText.text = "+" + (0.1f * Income).ToString() + "G";
        goldText.fontSize -= 10;
    }

    public void OnClickSellButton()
    {
        if (GameManager.instance.Dough >= Sales)
        {
            StartCoroutine(GameManager.instance.Count(GoodsType.DOUGH, GameManager.instance.Dough - Sales, GameManager.instance.Dough));

            if (GameManager.instance.HasPet(PetType.GREEN_CAT))
            {
                // 게으른 냥이 펫을 소유하고 있다면, 10%의 추가적인 골드를 획득한다.
                StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold + 1.1f * Income, GameManager.instance.Gold));
                GenerateEffectWithPet();
            }
            else
            {
                StartCoroutine(GameManager.instance.Count(GoodsType.GOLD, GameManager.instance.Gold + Income, GameManager.instance.Gold));
                GenerateEffect();
            }
        }
        else
        {
            GameManager.instance.PrintMessage("반죽량이 모자라요ㅠㅠ");
        }
    }
}
