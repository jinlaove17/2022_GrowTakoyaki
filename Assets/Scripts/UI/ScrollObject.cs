using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObject : MonoBehaviour
{
    private RectTransform rectTransform = null;
    private float halfWidth = 0.0f;
    private float canvasHalfWidth = 0.0f;

    public GameObject movableObject = null;
    public float speed = 20.0f;

    private void Awake()
    {
        if (movableObject == null)
        {
            Debug.LogError("movableObject is null!");
        }

        rectTransform = movableObject.GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("rectTransform is null!");
        }
    }

    private void Start()
    {
        halfWidth = 0.5f * rectTransform.rect.width;
        canvasHalfWidth = 0.5f * GameManager.instance.canvasRectTransform.rect.width;
    }

    private void Update()
    {
        rectTransform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        // 현재 객체의 무한 스크롤링(이동시, y좌표 값을 임의의 값으로 설정)
        if (rectTransform.position.x + halfWidth <= -canvasHalfWidth)
        {
            rectTransform.position = new Vector3(canvasHalfWidth + halfWidth, Random.Range(515.0f, 650.0f));
        }
    }
}
