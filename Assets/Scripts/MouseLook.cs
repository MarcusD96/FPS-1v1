
using UnityEngine;

public class MouseLook : MonoBehaviour {

    public float mouseSensitivity;
    public Transform body;
    public float recoil;
    public float maxRecoil;

    float xRotation = 0f;

    private void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        if(Settings.Paused)
            return;
        recoil = Mathf.Clamp(recoil, 0, 0.5f);
        recoil -= 1f / 60f;
        if(recoil < 0)
            recoil = 0;

        float speed = Time.deltaTime * mouseSensitivity;

        float mouseX = Input.GetAxisRaw("Mouse X") * speed;
        float mouseY = Input.GetAxisRaw("Mouse Y") * speed;

        xRotation -= mouseY + recoil;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        body.Rotate(Vector3.up * mouseX);
    }
}
