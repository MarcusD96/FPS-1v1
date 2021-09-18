using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {

    public float fireRate, impactForce, damage, reloadTime, minRange, maxRange;
    public int magazineSize;
    public bool isAuto;

    public ParticleSystem muzzleFlash;
    public BulletCasing bulletCasingPrefab;
    public Transform ejectPort;

    [HideInInspector]
    public int currentAmmo;
    [HideInInspector]
    public Recoil recoil;
    Animator animator;

    [HideInInspector]
    public bool isReloading;

    private void Start() {
        isReloading = false;
        recoil = GetComponent<Recoil>();
        animator = GetComponent<Animator>();
        currentAmmo = magazineSize + 1;
    }

    private void Update() {
        if(!isReloading && currentAmmo != magazineSize + 1) {
            if(Input.GetKeyDown(KeyCode.R)) {
                StartCoroutine(ReloadTime());
            }
        }
    }

    private void OnEnable() {
        if(isReloading)
            StartCoroutine(ReloadTime());
    }

    public void Fire() {
        recoil.StartRecoil();
        currentAmmo--;
        var c = Instantiate(bulletCasingPrefab, ejectPort.position, bulletCasingPrefab.transform.rotation);
        c.Initialize(transform.right.normalized + transform.up.normalized * 3);
        if(animator)
            animator.SetTrigger("Fire");
        if(currentAmmo <= 0) {
            StartCoroutine(ReloadTime());
        }
    }

    public void Reload() {
        if(currentAmmo > 0)
            currentAmmo = magazineSize + 1;
        else
            currentAmmo = magazineSize;
        isReloading = false;
    }

    IEnumerator ReloadTime() {
        isReloading = true;
        if(animator)
            animator.SetTrigger("Reload");
        float t = reloadTime + Time.time;
        while(Time.time < t) {
            yield return null;
        }

        Reload();
    }
}
