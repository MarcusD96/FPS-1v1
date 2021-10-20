using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Gun : MonoBehaviour {

    public float fireRate, impactForce, damage, minDamage, reloadTime, minRange, maxRange, adsZoom, adsSpeed, hipFireBaseSpread, hipFireMaxSpread, recoveryTime;
    public int magazineSize, penetration, remainingAmmo;
    public bool isAuto, canReload, maxSpread;
    public string soundName;

    public GameObject model;
    public ParticleSystem muzzleFlash;
    public BulletCasing bulletCasingPrefab;
    public Transform ejectPort;
    public Animator animator;

    [HideInInspector]
    public int currentAmmo;
    [HideInInspector]
    public Recoil recoil;
    [HideInInspector]
    public bool isReloading;

    private void Awake() {
        isReloading = false;
        canReload = true;
        recoil = GetComponent<Recoil>();
        currentAmmo = magazineSize + 1;
        remainingAmmo = magazineSize * 5;
    }

    private void Update() {
        if(canReload == false)
            return;

        if(!isReloading && currentAmmo != magazineSize + 1) {
            if(Input.GetKeyDown(KeyCode.R)) {
                Reload();
            }
        }
        if(!isReloading && currentAmmo <= 0)
            Reload();
    }

    private void OnEnable() {
        if(isReloading)
            Reload();
    }

    public void Fire() {
        if(currentAmmo <= 0) {
            Reload();
            return;
        }
        recoil.StartRecoil();
        currentAmmo--;
        var c = Instantiate(bulletCasingPrefab, ejectPort.position, bulletCasingPrefab.transform.rotation);
        c.Initialize(transform.right.normalized + transform.up.normalized * 3);
        if(animator)
            animator.SetTrigger("Fire");
    }

    public void Reload() {
        if(remainingAmmo <= 0)
            return;
        StartCoroutine(ReloadTime());
    }

    IEnumerator ReloadTime() {
        isReloading = true;

        animator.SetTrigger("Reload");
        animator.ResetTrigger("Fire");

        float t = reloadTime + Time.time;
        while(Time.time < t) {
            yield return null;
        }

        if(remainingAmmo < magazineSize) {
            if(currentAmmo - magazineSize + 1 < remainingAmmo && currentAmmo + remainingAmmo <= magazineSize + 1) {
                currentAmmo += remainingAmmo;
                remainingAmmo = 0;
                isReloading = false;
                yield break;
            }
        }


        if(currentAmmo > 0) {
            remainingAmmo -= magazineSize + 1 - currentAmmo;
            currentAmmo = magazineSize + 1;

        }
        else {
            remainingAmmo -= magazineSize - currentAmmo;
            currentAmmo = magazineSize;
        }

        if(remainingAmmo < 0)
            remainingAmmo = 0;

        isReloading = false;
    }

    public float GetSensitivity() {
        return Settings.Sensitivity * (GetZoomFOV() / Settings.FOV_Current);
    }

    public float GetZoomFOV() {
        return Settings.FOV_Current / adsZoom;
    }
}
