using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZoom : MonoBehaviour {

    public bool maxZoom;
    public PlayerShoot playerShoot;
    public MouseLook mouseLook;
    public Camera cam, weaponCam;

    float fovTracker = 0;
    public bool isZoomingIn = false;

    private void Update() {
        if(Settings.Paused)
            return;

        Zoom();

        fovTracker = Mathf.Clamp01(fovTracker);
    }

    public void ResetZoom() {
        mouseLook.mouseSensitivity = Settings.Sensitivity;
        cam.fieldOfView = weaponCam.fieldOfView = Settings.FOV;
        fovTracker = 0;
    }

    void Zoom() {
        if(playerShoot.currentGun.isReloading || playerShoot.meleeComp.isMeleeing || playerShoot.moveComp.isRunning) {
            isZoomingIn = false;
            ResetZoom();
            return;
        }

        if(Input.GetMouseButton(1)) {
            isZoomingIn = true;

            fovTracker += playerShoot.currentGun.adsSpeed * Time.deltaTime;

            mouseLook.mouseSensitivity = Mathf.Lerp(Settings.Sensitivity, playerShoot.currentGun.GetSensitivity(), fovTracker * 2);
            cam.fieldOfView = weaponCam.fieldOfView = Mathf.Lerp(Settings.FOV, playerShoot.currentGun.GetZoomFOV(), fovTracker);
        }
        else if(Input.GetMouseButtonUp(1)) {
            isZoomingIn = false;
            StartCoroutine(ZoomOut());
        }

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, playerShoot.currentGun.GetZoomFOV(), Settings.FOV);
        weaponCam.fieldOfView = Mathf.Clamp(weaponCam.fieldOfView, playerShoot.currentGun.GetZoomFOV(), Settings.FOV);

        if(cam.fieldOfView <= Settings.FOV - ((Settings.FOV - playerShoot.currentGun.GetZoomFOV()) * 0.75f))
            maxZoom = true;
        else
            maxZoom = false;
    }

    IEnumerator ZoomOut() {
        while(cam.fieldOfView <= Settings.FOV) {

            fovTracker -= playerShoot.currentGun.adsSpeed * 2 * Time.deltaTime;

            mouseLook.mouseSensitivity = Mathf.Lerp(Settings.Sensitivity, playerShoot.currentGun.GetSensitivity(), fovTracker);
            cam.fieldOfView = weaponCam.fieldOfView = Mathf.Lerp(Settings.FOV, playerShoot.currentGun.GetZoomFOV(), fovTracker);

            if(isZoomingIn)
                yield break;

            yield return null;
        }
    }
}
