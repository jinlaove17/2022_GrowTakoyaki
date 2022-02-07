using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnim : MonoBehaviour
{
    public void RemoveThisObject()
    {
        // 이 스크립트를 소유한 객체를 제거한다.
        Destroy(gameObject);
    }
}
