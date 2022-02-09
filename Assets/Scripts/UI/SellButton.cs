using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellButton : MonoBehaviour
{
    // �ǸŽ��� �Ҹ� ���׷� ���� ������
    private static uint sales = 0;
    private static Text salesText = null;

    // �ǸŽ��� ȹ�� ��� ���� ������
    private static uint income = 0;

    // ����Ʈ�� ��ġ ������
    private Vector3     particlePosition = Vector3.zero;
    private Vector3     incomeTextPosition = Vector3.zero;
    private Vector3     petIncomeTextPosition = Vector3.zero;

    // ������ ������
    public GameObject   particlePrefab = null;
    public GameObject   goldTextPrefab = null;

    // ���� ���� ������
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
        // �Ǹ� ��ƼŬ ����Ʈ ���
        Instantiate(particlePrefab, particlePosition, Quaternion.identity, GameManager.instance.canvasRectTransform);

        // ȹ�� ��� �ؽ�Ʈ ����Ʈ ���
        GameObject goldEffect = Instantiate(goldTextPrefab, incomeTextPosition, Quaternion.identity, GameManager.instance.canvasRectTransform);
        goldEffect.GetComponent<Text>().text = "+" + Income.ToString() + "G";

        // �Ǹ� ���� ���
        SoundManager.instance.PlaySFX("SELL");
    }

    private void GenerateEffectWithPet()
    {
        GenerateEffect();

        // ������ ���� ���� �����ϰ� �ִٸ�, �߰����� ��� �ؽ�Ʈ ����Ʈ�� ����Ѵ�.
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
                // ������ ���� ���� �����ϰ� �ִٸ�, 10%�� �߰����� ��带 ȹ���Ѵ�.
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
            GameManager.instance.PrintMessage("���׷��� ���ڶ��Ф�");
        }
    }
}
