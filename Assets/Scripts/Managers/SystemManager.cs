using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ų�� ����
public enum SkillType { AUTO_SELL, PLUS_FEVER_TIME, ENUM_COUNT }

// ��ȭ�� ����
public enum GoodsType { GOLD, DOUGH, ENUM_COUNT };

// ������ ����
public enum SoundType { BGM, SFX, ENUM_COUNT }

// ���� ����
public enum PetType { RED_CAT, YELLOW_CAT, GREEN_CAT, ENUM_COUNT }

// * ��� �� ����
static class Constants
{
    // ��ų�� ����(����/����� ����)
    public const int   HAS_SKILL1     = 0x00000001;
    public const int   HAS_SKILL2     = 0x00000002;
    public const int   USING_SKILL1   = 0x00000004;
    public const int   USING_SKILL2   = 0x00000008;

    // �ǹ���� ���� �޺�
    public const int   FEVER_COMBO    = 100;

    // �޺� ���� �ð�
    public const float COMBO_DURATION = 0.65f;
}

public static class SystemManager
{
    public static void CheckNull(Object obj)
    {
        if (obj == null)
        {
            Debug.LogError(obj.name + "is null!");
        }
    }

    public static void CheckNull(Object[] objs)
    {
        foreach (Object obj in objs)
        {
            if (obj == null)
            {
                Debug.LogError(obj.name + "is null!");
                return;
            }
        }
    }
}
