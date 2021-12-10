
using UnityEngine;

public class WeaponSway : MonoBehaviour {

    [SerializeField]
    float smoothing, swayMultiplier;

    private void Update() {
        //get mouse input
        float mX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        //calculate target rotation
        Quaternion rotX = Quaternion.AngleAxis(-mY, Vector3.right);
        Quaternion rotY = Quaternion.AngleAxis(mX, Vector3.up);

        Quaternion targetRot = rotX * rotY;

        //rotate
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, smoothing * Time.deltaTime);
    }

}
