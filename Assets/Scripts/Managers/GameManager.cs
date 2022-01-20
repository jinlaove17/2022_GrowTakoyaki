using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // 재화의 종류
    public enum Goods
    {
        GOLD,
        DOUGH
    };

    // 싱글톤 패턴
    public static GameManager instance = null;

    // 고양이의 애니메이션 출력을 위한 애니메이터
    private Animator catAnimator = null;

    // 게임 데이터는 LoadData() 함수에서 PlayerPrefs를 이용하여 초기화를 수행한다.
    private uint gold = 0;
    private Text goldText = null;
    //private int goldLevel = 0;
    //private string[] goldUnitArr = new string[] { "", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    private uint dough = 0;
    private Text doughText = null;
    private uint doughIncrement = 0;

    private uint lastCombo = 0;
    private uint currentCombo = 0;
    private uint maxCombo = 0;

    public RectTransform canvasRectTransform = null;

    public GameObject starPrefab = null;
    public GameObject scorePrefab = null;
    public GameObject comboPrefab = null;
    public GameObject feverPrefab = null;

    public uint Gold
    {
        set
        {
            gold = value;
            goldText.text = string.Format("{0:#,0}", gold);
        }

        get
        { 
            return gold;
        }
    }

    public uint Dough
    {
        set
        { 
            dough = value;
            doughText.text = string.Format("{0:#,0}", dough);
        }

        get
        { 
            return dough;
        }
    }

    public uint DoughIncrement
    {
        set
        { 
            doughIncrement = value;
        }

        get
        { 
            return doughIncrement;
        }
    }

    private void Awake()
    {
        // Fix 60FPS
        Application.targetFrameRate = 60;

        instance = this;

        catAnimator = GameObject.Find("Cat").GetComponent<Animator>();

        if (catAnimator == null)
        {
            Debug.LogError("catAnimator is null!");
        }

        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("audioSource is null!");
        }

        SoundManager.instance.RegisterAudioclip("Click", audioSource.clip);

        goldText = GameObject.Find("Gold").GetComponentInChildren<Text>();

        if (goldText == null)
        {
            Debug.LogError("goldText is null!");
        }

        doughText = GameObject.Find("Dough").GetComponentInChildren<Text>();

        if (doughText == null)
        {
            Debug.LogError("doughText is null!");
        }

        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("canvas is null!");
        }

        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    private void Start()
    {
        LoadData();

        // Auto Save
        InvokeRepeating("SaveData", 0.0f, 3.0f);
    }

    private void Update()
    {
        Dough += 1;

        if (!DictButton.IsOpened && !OptionButton.IsOpened)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);

                if (hit.collider != null)
                {
                    StartCoroutine(UpdateCombo());
                    StartCoroutine(Count(Goods.DOUGH, dough + doughIncrement, dough));
                    GenerateEffect(position);
                }
            }
        }
    }

    private void GenerateEffect(Vector3 position)
    {
        // 별 파티클 이펙트
        position.z = -50.0f;
        Instantiate(starPrefab, position, Quaternion.identity, canvasRectTransform);

        // 획득 점수 텍스트 이펙트
        position.y -= 65.0f;
        GameObject scoreEffect = Instantiate(scorePrefab, position, Quaternion.identity, canvasRectTransform);
        scoreEffect.GetComponent<Text>().text = "+" + doughIncrement.ToString() + "L";

        // 콤보 텍스트 이펙트
        GameObject comboEffect = Instantiate(comboPrefab, canvasRectTransform);
        comboEffect.GetComponent<Text>().text = currentCombo.ToString() + comboEffect.GetComponent<Text>().text;
        comboEffect.transform.GetChild(0).GetComponent<Text>().text += maxCombo.ToString();

        // 고양이의 애니메이션 트리거 발생
        catAnimator.SetTrigger("doTouch");

        // 클릭사운드 출력
        SoundManager.instance.PlaySFX("Click", 0.5f);
    }

    // 참고 : https://unitys.tistory.com/7
    public IEnumerator Count(Goods goods, float target, float current)
    {
        float duration = 0.08f;
        float offset;

        switch (goods)
        {
            case Goods.DOUGH:
                offset = (target - dough) / duration;

                if (offset >= 0.0f)
                {
                    while (current < target)
                    {
                        current += offset * Time.deltaTime;
                        doughText.text = string.Format("{0:#,0}", current);

                        yield return null;
                    }
                }
                else
                {
                    while (current > target)
                    {
                        current += offset * Time.deltaTime;
                        doughText.text = string.Format("{0:#,0}", current);

                        yield return null;
                    }
                }

                Dough = (uint)target;
                break;
            case Goods.GOLD:
                offset = (target - gold) / duration;

                if (offset >= 0.0f)
                {
                    while (current < target)
                    {
                        current += offset * Time.deltaTime;
                        goldText.text = string.Format("{0:#,0}", current);

                        yield return null;
                    }
                }
                else
                {
                    while (current > target)
                    {
                        current += offset * Time.deltaTime;
                        goldText.text = string.Format("{0:#,0}", current);

                        yield return null;
                    }
                }

                Gold = (uint)target;
                break;
        }
    }

    private IEnumerator UpdateCombo()
    {
        ++currentCombo;

        if (currentCombo > 0 && (currentCombo % 100) == 0)
        {
            Instantiate(feverPrefab, canvasRectTransform);
            StartCoroutine(UpdateFeverMode());
        }

        yield return new WaitForSeconds(0.65f);

        ++lastCombo;

        if (currentCombo == lastCombo)
        {
            if (currentCombo > maxCombo)
            {
                maxCombo = currentCombo;
            }

            currentCombo = lastCombo = 0;
        }
    }

    private IEnumerator UpdateFeverMode()
    {
        Image background = canvasRectTransform.GetChild(0).gameObject.GetComponent<Image>();

        doughIncrement *= 2;
        SellButton.Income *= 2;
        background.color = Color.red;
        SoundManager.instance.PlayBGM("FEVER_TIME");

        yield return new WaitForSeconds(8.0f);

        doughIncrement /= 2;
        SellButton.Income /= 2;
        background.color = Color.white;
        SoundManager.instance.PlayBGM("BGM");
    }

    public void SaveData()
    {
        // GameManager Data
        PlayerPrefs.SetInt("GOLD", (int)gold);
        PlayerPrefs.SetInt("DOUGH", (int)dough);
        PlayerPrefs.SetInt("DOUGH_INCREMENT", (int)doughIncrement);
        PlayerPrefs.SetInt("MAX_COMBO", (int)maxCombo);

        // Sell Button Data
        PlayerPrefs.SetInt("INCOME", (int)SellButton.Income);
        PlayerPrefs.SetInt("SALES", (int)SellButton.Sales);

        // Whisk Button Data
        PlayerPrefs.SetInt("WHISK_LEVEL", (int)WhiskButton.Level);

        // Recipe Button Data
        PlayerPrefs.SetInt("RECIPE_LEVEL", (int)RecipeButton.Level);
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("GOLD"))
        {
            Gold = (uint)PlayerPrefs.GetInt("GOLD");
        }
        else
        {
            Gold = 10000000;
        }

        if (PlayerPrefs.HasKey("DOUGH"))
        {
            Dough = (uint)PlayerPrefs.GetInt("DOUGH");
        }
        else
        {
            Dough = 10000000;
        }

        if (PlayerPrefs.HasKey("DOUGH_INCREMENT"))
        {
            DoughIncrement = (uint)PlayerPrefs.GetInt("DOUGH_INCREMENT");
        }
        else
        {
            DoughIncrement = 500;
        }

        if (PlayerPrefs.HasKey("MAX_COMBO"))
        {
            maxCombo = (uint)PlayerPrefs.GetInt("MAX_COMBO");
        }
        else
        {
            maxCombo = 0;
        }

        if (PlayerPrefs.HasKey("INCOME"))
        {
            SellButton.Income = (uint)PlayerPrefs.GetInt("INCOME");
        }
        else
        {
            SellButton.Income = 100;
        }

        if (PlayerPrefs.HasKey("SALES"))
        {
            SellButton.Sales = (uint)PlayerPrefs.GetInt("SALES");
        }
        else
        {
            SellButton.Sales = 100;
        }

        if (PlayerPrefs.HasKey("WHISK_LEVEL"))
        {
            WhiskButton.Level = (uint)PlayerPrefs.GetInt("WHISK_LEVEL");
        }
        else
        {
            WhiskButton.Level = 1;
        }

        if (PlayerPrefs.HasKey("RECIPE_LEVEL"))
        {
            RecipeButton.Level = (uint)PlayerPrefs.GetInt("RECIPE_LEVEL");
        }
        else
        {
            RecipeButton.Level = 1;
        }
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }

    private void OnApplicationQuit()
    {
        SaveData();
        ResetData();
    }
}
