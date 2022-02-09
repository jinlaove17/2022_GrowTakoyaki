using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬의 종류
public enum SkillType { AUTO_SELL, PLUS_FEVER_TIME, ENUM_COUNT }

// 재화의 종류
public enum GoodsType { GOLD, DOUGH, ENUM_COUNT };

// 사운드의 종류
public enum SoundType { BGM, SFX, ENUM_COUNT }

// 펫의 종류
public enum PetType { RED_CAT, YELLOW_CAT, GREEN_CAT, ENUM_COUNT }

// * 상수 값 정의
static class Constants
{
    // 스킬의 상태(보유/사용중 여부)
    public const int   HAS_SKILL1     = 0x00000001;
    public const int   HAS_SKILL2     = 0x00000002;
    public const int   USING_SKILL1   = 0x00000004;
    public const int   USING_SKILL2   = 0x00000008;

    // 피버모드 진입 콤보
    public const int   FEVER_COMBO    = 100;

    // 콤보 지속 시간
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
