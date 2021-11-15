using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Gun : MonoBehaviour {

    public float fireRate, impactForce, damage, minDamage, reloadTime, minRange, maxRange, adsZoom, adsSpeed, hipFireBaseSpread, hipFireMaxSpread, recoveryTime, switchInSpeed;
    public int magazineSize, penetration, remainingAmmo, numReloadShells;
    public bool isAuto, canReload, maxSpread, isShotgun, upgraded, primaryShot = true;
    public string soundName;
    public int shots;

    public float headShotMult, torsoShotMult;

    public GameObject model;
    public GunNameID gunID;
    public ParticleSystem muzzleFlash, secondaryMuzzleFlash;
    public BulletCasing bulletCasingPrefab;
    public Transform[] ejectPorts;
    public Animator animator;
    public Gun upgradedGun;

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
        animator.SetTrigger("Fire");
        recoil.StartRecoil();
        currentAmmo--;
        EjectShell();
    }

    int ejectIndex = 0;
    void EjectShell() {
        var c = Instantiate(bulletCasingPrefab, ejectPorts[ejectIndex].position, bulletCasingPrefab.transform.rotation);
        c.Initialize(transform.right.normalized + transform.up.normalized * 3);
        ejectIndex++;
        if(ejectIndex >= ejectPorts.Length)
            ejectIndex = 0;
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

        #region old reload
        while(currentAmmo < magazineSize) { //under mag size
            yield return new WaitForSeconds(reloadTime);
            if(remainingAmmo > 0) { //has ammo still
                currentAmmo += numReloadShells;
                if(currentAmmo > magazineSize)
                    currentAmmo = magazineSize;
                remainingAmmo -= numReloadShells;
                if(remainingAmmo < 0)
                    remainingAmmo = 0;
            }
            else
                break;
        }
        #endregion

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
