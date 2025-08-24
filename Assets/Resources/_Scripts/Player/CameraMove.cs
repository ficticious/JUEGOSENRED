using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private new Transform camera;
    public Vector2 sensibility;

    private void Start()
    {
        if (camera == null)
            Debug.LogError("No se asignó la cámara");
        //camera = transform.Find("Camera");
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float hor = Input.GetAxis("Mouse X");
        float ver = Input.GetAxis("Mouse Y");

        if (hor != 0)
        {
            transform.Rotate(Vector3.up * hor * sensibility.x);
        }

        if (ver != 0)
        {
            float angle = (camera.localEulerAngles.x - ver * sensibility.y + 360) % 360;
            if (angle > 180) { angle -= 360; }
            angle = Mathf.Clamp(angle, -60, 80);

            camera.localEulerAngles = Vector3.right * angle;
        }

    }

}
