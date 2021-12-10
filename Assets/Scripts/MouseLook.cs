
using UnityEngine;

public class MouseLook : MonoBehaviour {

    public float mouseSensitivity;
    public Transform rootBody;

    float xRotation = 0f;

    private void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        if(Settings.Paused)
            return;

        float speed = Time.deltaTime * mouseSensitivity;

        float mouseX = Input.GetAxis("Mouse X") * speed;
        float mouseY = Input.GetAxis("Mouse Y") * speed;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89.99f, 89.99f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        rootBody.Rotate(Vector3.up * mouseX);
    }
}
