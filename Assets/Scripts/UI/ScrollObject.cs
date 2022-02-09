using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObject : MonoBehaviour
{
    private RectTransform rectTransform = null;

    private float         halfWidth = 0.0f;
    private float         canvasHalfWidth = 0.0f;
    private float         speed = 15.0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        SystemManager.CheckNull(rectTransform);
    }

    private void Start()
    {
        halfWidth = 0.5f * rectTransform.rect.width;
        canvasHalfWidth = 0.5f * GameManager.instance.canvasRectTransform.rect.width;
    }

    private void Update()
    {
        // �� �����Ӹ��� ���� �Ÿ��� �̵��Ѵ�.
        rectTransform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        // ���� ȭ�� �������� �������, �ٽ� �������� ���۵ǵ��� �Ѵ�.
        if (rectTransform.position.x + halfWidth <= -canvasHalfWidth)
        {
            rectTransform.position = new Vector3(canvasHalfWidth + halfWidth, rectTransform.position.y);
        }
    }
}
