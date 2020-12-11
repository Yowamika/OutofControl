using UnityEngine;

/// <summary>
/// GameビューにてSceneビューのようなカメラの動きをマウス操作によって実現する
/// https://gist.github.com/EsProgram/0fd35669c28fd13594c8
/// 改変：浅野
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10f)]
    private float wheelSpeed = 2f;

    [SerializeField, Range(0.1f, 10f)]
    private float moveSpeed = 0.3f;

    [SerializeField, Range(0.1f, 10f)]
    private float rotateSpeed = 0.3f;

    private Vector3 preMousePos;
    [SerializeField]
    private Transform target = null;

    [SerializeField]
    private KeyCode upKey = KeyCode.W;
    [SerializeField]
    private KeyCode downKey = KeyCode.S;
    [SerializeField]
    private KeyCode rightKey = KeyCode.D;
    [SerializeField]
    private KeyCode leftKey = KeyCode.A;

    private void Update()
    {
        MouseUpdate();
        KeyMove();
        return;
    }

    private void MouseUpdate()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0.0f)
            MouseWheel(scrollWheel);

        if (Input.GetMouseButtonDown(0) ||
           Input.GetMouseButtonDown(1) ||
           Input.GetMouseButtonDown(2))
            preMousePos = Input.mousePosition;

        MouseDrag(Input.mousePosition);
    }

    private void MouseWheel(float delta)
    {
        target.position += transform.forward * delta * wheelSpeed;
        transform.position += transform.forward * delta * wheelSpeed;
        return;
    }

    private void MouseDrag(Vector3 mousePos)
    {
        Vector3 diff = mousePos - preMousePos;

        if (diff.magnitude < Vector3.kEpsilon)
            return;

        //if (Input.GetMouseButton(2))
        //    transform.Translate(-diff * Time.deltaTime * moveSpeed);
        //else if (Input.GetMouseButton(1))
        //    CameraRotate(new Vector2(-diff.y, diff.x) * rotateSpeed);
        if (Input.GetMouseButton(2))
            CameraRotate(new Vector2(-diff.y, diff.x) * rotateSpeed);

        preMousePos = mousePos;
    }

    public void CameraRotate(Vector2 angle)
    {
        transform.RotateAround(target.position, transform.right, angle.x);
        transform.RotateAround(target.position, Vector3.up, angle.y);
        target.RotateAround(target.position, transform.right, angle.x);
        target.RotateAround(target.position, Vector3.up, angle.y);
    }

    private void KeyMove()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(upKey))
        {
            transform.Translate(Vector3.up * moveSpeed);
            target.Translate(Vector3.up * moveSpeed);
        }
        if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(downKey))
        {
            transform.Translate(Vector3.down * moveSpeed);
            target.Translate(Vector3.down * moveSpeed);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(rightKey))
        {
            transform.Translate(Vector3.right * moveSpeed);
            target.Translate(Vector3.right * moveSpeed);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(leftKey))
        {
            transform.Translate(Vector3.left * moveSpeed);
            target.Translate(Vector3.left * moveSpeed);
        }
    }
}