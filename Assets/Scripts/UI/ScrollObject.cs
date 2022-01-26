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
        rectTransform = movableObject.GetComponent<RectTransform>();
        GameManager.CheckNull(rectTransform);
    }

    private void Start()
    {
        halfWidth = 0.5f * rectTransform.rect.width;
        canvasHalfWidth = 0.5f * GameManager.instance.canvasRectTransform.rect.width;
    }

    private void Update()
    {
        rectTransform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        if (rectTransform.position.x + halfWidth <= -canvasHalfWidth)
        {
            rectTransform.position = new Vector3(canvasHalfWidth + halfWidth, rectTransform.position.y);
        }
    }
}
