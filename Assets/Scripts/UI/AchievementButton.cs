using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementButton : MonoBehaviour
{
    // ����â�� ���ȴ����� ���� ����
    private static bool isOpened = false;

    // ����â�� �ִϸ��̼� ����� ���� �ִϸ�����
    private Animator achievementAnimator = null;

    private void Awake()
    {
        GameObject achievement = GameObject.Find("Achievement");
        GameManager.CheckNull(achievement);

        achievementAnimator = achievement.GetComponent<Animator>();
        GameManager.CheckNull(achievementAnimator);
    }

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

    public void OnClickAchievementButton()
    {
        if (GameManager.instance.IsClosed())
        {
            isOpened = true;
            achievementAnimator.SetTrigger("doShow");
            SoundManager.instance.PlaySFX("BookSlap");
        }
    }

    public void OnClickExitDictButton()
    {
        if (isOpened)
        {
            IsOpened = false;
            achievementAnimator.SetTrigger("doHide");
        }
    }
}
