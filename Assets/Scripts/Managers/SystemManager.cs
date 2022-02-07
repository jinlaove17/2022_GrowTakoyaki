using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// * 상수 값 정의
static class Constants
{
    // 스킬의 상태(보유/사용중 여부)
    public const int HAS_SKILL1 = 0x00000001;
    public const int HAS_SKILL2 = 0x00000002;
    public const int USING_SKILL1 = 0x00000004;
    public const int USING_SKILL2 = 0x00000008;
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
