using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerZoom : MonoBehaviour {

    public bool maxZoom;
    public Transform hand;
    public PlayerShoot playerShoot;
    public MouseLook mouseLook;
    public Camera cam, weaponCam;
    public GameObject sniperScope, crossHair;

    float fovTracker = 0;
    public bool isZoomingIn = false;

    private Vector3 handRestPos, handAimPos;

    void Start() {
        handRestPos = hand.localPosition;
        handAimPos = handRestPos + (Vector3.back + Vector3.up + Vector3.left) / 4.0f;
    }

    private void Update() {
        if(Settings.Paused)
            return;

        Zoom();

        fovTracker = Mathf.Clamp01(fovTracker);
    }

    public void ResetZoom() {
        mouseLook.mouseSensitivity = Settings.Sensitivity;
        cam.fieldOfView = Settings.FOV_Current;
        fovTracker = 0;
        playerShoot.currentGun.model.SetActive(true);
        crossHair.SetActive(true);
        sniperScope.SetActive(false);
    }

    void Zoom() {
        if(playerShoot.currentGun.isReloading || playerShoot.meleeComp.isMeleeing || playerShoot.moveComp.isRunning) {
            hand.localPosition = handRestPos;
            playerShoot.currentGun.model.SetActive(true);
            crossHair.SetActive(true);
            sniperScope.SetActive(false);
            isZoomingIn = false;
            ResetZoom();
            return;
        }

        if(Input.GetMouseButton(1)) {
            isZoomingIn = true;

            fovTracker += playerShoot.currentGun.adsSpeed * Time.deltaTime;

            mouseLook.mouseSensitivity = Mathf.Lerp(Settings.Sensitivity, playerShoot.currentGun.GetSensitivity(), fovTracker * 2);
            cam.fieldOfView = Mathf.Lerp(Settings.FOV_Current, playerShoot.currentGun.GetZoomFOV(), fovTracker);
            hand.localPosition = Vector3.Lerp(handRestPos, handAimPos, fovTracker);
        }
        else if(Input.GetMouseButtonUp(1)) {
            isZoomingIn = false;
            StartCoroutine(ZoomOut());
        }
        else {
            if(zoomedOut) {
                mouseLook.mouseSensitivity = Settings.Sensitivity;
                cam.fieldOfView = Settings.FOV_Current;
            }
        }

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, playerShoot.currentGun.GetZoomFOV(), Settings.FOV_Current);

        if(cam.fieldOfView <= Settings.FOV_Current - ((Settings.FOV_Current - playerShoot.currentGun.GetZoomFOV()) * 0.75f)) {
            maxZoom = true;
            if(playerShoot.currentGun.maxSpread && !playerShoot.currentGun.isShotgun) {
                playerShoot.currentGun.model.SetActive(false);
                crossHair.SetActive(false);
                sniperScope.SetActive(true);
            }
        }
        else {
            maxZoom = false;
        }
    }

    bool zoomedOut = false;
    IEnumerator ZoomOut() {
        zoomedOut = false;
        while(fovTracker > 0f) {

            Time.maximumDeltaTime = 1f / 60f;
            fovTracker -= playerShoot.currentGun.adsSpeed * 2f * Time.deltaTime;

            mouseLook.mouseSensitivity = Mathf.Lerp(Settings.Sensitivity, playerShoot.currentGun.GetSensitivity(), fovTracker);
            cam.fieldOfView = Mathf.Lerp(Settings.FOV_Current, playerShoot.currentGun.GetZoomFOV(), fovTracker);
            hand.localPosition = Vector3.Lerp(handRestPos, handAimPos, fovTracker);

            if(cam.fieldOfView <= Settings.FOV_Current - ((Settings.FOV_Current - playerShoot.currentGun.GetZoomFOV()) * 0.25f)) {
                if(playerShoot.currentGun.maxSpread) {
                    playerShoot.currentGun.model.SetActive(true);
                    crossHair.SetActive(true);
                    sniperScope.SetActive(false);
                }
            }

            if(isZoomingIn) {
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
        zoomedOut = true;
    }
}