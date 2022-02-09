using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnim : MonoBehaviour
{
    public void RemoveThisObject()
    {
        // 이 스크립트를 소유한 객체를 제거하는 함수이다.
        // 게임 중 동적으로 Instantiate() 함수로 생성하고, 애니메이션을 모두 출력한 객체에서 호출한다.
        Destroy(gameObject);
    }
}
