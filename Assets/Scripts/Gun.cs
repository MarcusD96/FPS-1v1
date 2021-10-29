using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Gun : MonoBehaviour {

    public float fireRate, impactForce, damage, minDamage, reloadTime, minRange, maxRange, adsZoom, adsSpeed, hipFireBaseSpread, hipFireMaxSpread, recoveryTime, switchInSpeed, switchOutSpeed;
    public int magazineSize, penetration, remainingAmmo;
    public bool isAuto, canReload, maxSpread, isShotgun;
    public string soundName;
    public int shots;

    public float headShotMult, torsoShotMult;

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
        currentAmmo = magazineSize;
        if(!isShotgun)
            currentAmmo += 1;
        remainingAmmo = magazineSize * 10;
    }

    private void Update() {
        if(canReload == false)
            return;

        if(!isReloading && currentAmmo < magazineSize + 1) {
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

        if(!isShotgun)
            StartCoroutine(ReloadTime());
        else {
            if(!isReloading)
                StartCoroutine(ReloadShotGun());
        }
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

    IEnumerator ReloadShotGun() {
        isReloading = true;
        animator.SetBool("Reloading", isReloading);
        animator.ResetTrigger("Fire");
        yield return null;

        //yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(1f / 3f);

        if(currentAmmo < 1) {
            while(currentAmmo < magazineSize) {
                yield return new WaitForSeconds(reloadTime);
                if(remainingAmmo > 0) {
                    currentAmmo += 1;
                    remainingAmmo -= 1;
                }
                else
                    break;
            }
        }
        else {
            while(currentAmmo < magazineSize + 1) {
                yield return new WaitForSeconds(reloadTime);
                if(remainingAmmo > 0) {
                    currentAmmo += 1;
                    remainingAmmo -= 1;
                }
                else
                    break;
            }
        }

        isReloading = false;
        animator.SetBool("Reloading", isReloading);
    }

    public float GetSensitivity() {
        return Settings.Sensitivity * (GetZoomFOV() / Settings.FOV_Current);
    }

    public float GetZoomFOV() {
        return Settings.FOV_Current / adsZoom;
    }
}
