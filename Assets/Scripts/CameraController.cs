using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float minDistance = 3;
    public float maxDistance = 30;
    public float wheelSpeed = 500;
    public float xMoveSpeed = 500;
    public float yMoveSpeed = 250;
    float yMinLimit = 5;
    float yMaxLimit = 80;
    float x, y;
    float distance;

    void Awake()
    {
        distance = Vector3.Distance(transform.position, target.position);
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        x += Input.GetAxis("Mouse X") * xMoveSpeed * Time.deltaTime;
        y -= Input.GetAxis("Mouse Y") * yMoveSpeed * Time.deltaTime;

        y = ClampAngle(y, yMinLimit, yMaxLimit);
        transform.rotation = Quaternion.Euler(y, x, 0);

        distance -= Input.GetAxis("Mouse ScrollWheel") * wheelSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = transform.rotation * new Vector3(0, 0, -distance) + target.position;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
