using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public Transform target;
    public float cameraSpeed = 5f;
    public Vector2 cameraOffset;
    public float cameraViewSize; // �ν����Ϳ��� ������ �� �ִ� ī�޶� �þ� ũ��

    void Start()
    {
        Camera.main.orthographicSize = cameraViewSize;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector2 desiredPosition = (Vector2)target.position + cameraOffset;
            Vector2 smoothedPosition = Vector2.Lerp(transform.position, desiredPosition, cameraSpeed);
            transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
        }
    }
}

