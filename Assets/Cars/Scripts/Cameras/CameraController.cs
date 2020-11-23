using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public Transform target;

    private const float distance = 5.2f;
    private Vector3 offset = new Vector3(0f, 0f, -distance);
    private Vector3 lookDown = new Vector3(10f, 0f, 0f);
    private const float followRate = 0.1f;

    void Start()
    {
        transform.position = target.TransformPoint(offset);
        transform.LookAt(target, Vector3.up);
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.TransformPoint(offset);
        Vector3 lerp = Vector3.Lerp(transform.position, desiredPosition, followRate);
        Vector3 toTarget = target.position - lerp;
        toTarget.Normalize();
        toTarget *= distance;
        transform.position = target.position - toTarget;
        transform.LookAt(target, Vector3.up);
        transform.Rotate(lookDown);
    }
}