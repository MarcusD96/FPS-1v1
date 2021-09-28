using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {

    public float fireRate, impactForce, damage, minDamage, reloadTime, minRange, maxRange, adsZoom, adsSpeed, hipFireMaxSpread;
    public int magazineSize;
    public bool isAuto, canReload, maxSpread;

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

    private void Start() {
        isReloading = false;
        canReload = true;
        recoil = GetComponent<Recoil>();
        animator = GetComponent<Animator>();
        currentAmmo = magazineSize + 1;
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
        }
        recoil.StartRecoil();
        currentAmmo--;
        var c = Instantiate(bulletCasingPrefab, ejectPort.position, bulletCasingPrefab.transform.rotation);
        c.Initialize(transform.right.normalized + transform.up.normalized * 3);
        if(animator)
            animator.SetTrigger("Fire");
    }

    public void Reload() {
        StartCoroutine(ReloadTime());
    }

    IEnumerator ReloadTime() {
        isReloading = true;
        if(animator)
            animator.SetTrigger("Reload");
        float t = reloadTime + Time.time;
        while(Time.time < t) {
            yield return null;
        }

        if(currentAmmo > 0)
            currentAmmo = magazineSize + 1;
        else
            currentAmmo = magazineSize;
        isReloading = false;
    }

    public float GetSensitivity() {
        return Settings.Sensitivity * (GetZoomFOV() / Settings.FOV);
    }

    public float GetZoomFOV() {
        return Settings.FOV / adsZoom;
    }
}
