using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZoom : MonoBehaviour {

    public float zoomSpeed;
    public PlayerShoot playerShoot;
    public MouseLook mouseLook;
    public Camera cam, weaponCam;

    float startFOV, fovTracker;
    bool isZoomingIn = false;

    private void Start() {
        startFOV = cam.fieldOfView;
        fovTracker = 0;
    }

    private void Update() {
        Zoom();
    }

    void Zoom() {
        if(playerShoot.currentGun.isReloading) {
            isZoomingIn = false;
            mouseLook.mouseSensitivity = 300;
            cam.fieldOfView = weaponCam.fieldOfView = startFOV;
            return;
        }

        if(Input.GetMouseButton(1)) {
            if(!isZoomingIn) {
                isZoomingIn = true;
                mouseLook.mouseSensitivity = playerShoot.currentGun.adsSensitivity;
                fovTracker = 0;
            }
            fovTracker += zoomSpeed * Time.deltaTime;
            cam.fieldOfView = weaponCam.fieldOfView = Mathf.Lerp(cam.fieldOfView, playerShoot.currentGun.adsMinFOV, fovTracker);

        }
        else if(Input.GetMouseButtonUp(1)) {
            if(isZoomingIn) {
                isZoomingIn = false;
                mouseLook.mouseSensitivity = 300;
            }
            StartCoroutine(ZoomOut());
        }
        Mathf.Clamp(cam.fieldOfView, playerShoot.currentGun.adsMinFOV, startFOV);
        Mathf.Clamp(weaponCam.fieldOfView, playerShoot.currentGun.adsMinFOV, startFOV);
    }

    IEnumerator ZoomOut() {
        fovTracker = 0;

        while(cam.fieldOfView <= startFOV) {
            fovTracker += zoomSpeed * Time.deltaTime;
            cam.fieldOfView = weaponCam.fieldOfView = Mathf.Lerp(cam.fieldOfView, startFOV, fovTracker);
            if(isZoomingIn)
                break;
            yield return null;
        }
    }
}
