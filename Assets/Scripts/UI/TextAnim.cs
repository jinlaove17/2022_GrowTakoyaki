using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnim : MonoBehaviour
{
    public void RemoveThisObject()
    {
        // �� ��ũ��Ʈ�� ������ ��ü�� �����ϴ� �Լ��̴�.
        // ���� �� �������� Instantiate() �Լ��� �����ϰ�, �ִϸ��̼��� ��� ����� ��ü���� ȣ���Ѵ�.
        Destroy(gameObject);
    }
}
