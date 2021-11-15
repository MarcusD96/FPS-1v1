
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EZCameraShake;

public class PlayerShoot : MonoBehaviour {

    public Transform shootPos;
    public GameObject objectImpactEffect, bloodHitEffect;
    public BulletHole bulletHole;
    public LayerMask shootableLayerMask;
    public Gun currentGun;
    public List<Gun> guns;
    public TextMeshProUGUI ammoText;
    public Melee meleeComp;
    public PlayerMovement moveComp;
    public PlayerZoom zoom;
    public float firingSpreadRadius;

    float nextFire = 0f;

    [HideInInspector]
    public int gunIndex = 0;
    [HideInInspector]
    public bool isSwitching = false;

    private void Awake() {
        zoom = GetComponent<PlayerZoom>();
    }

    private void Start() {
        EquipWeapon();
    }

    private void Update() {
        if(Settings.Paused)
            return;

        SwitchWeapon();
        Shoot();
    }

    void Shoot() {

        nextFire -= Time.deltaTime;

        ammoText.text = currentGun.currentAmmo.ToString() + "/" + currentGun.remainingAmmo;

        if(currentGun.currentAmmo <= 0 && currentGun.remainingAmmo <= 0)
            return;

        if(nextFire <= -0.1f && !currentGun.maxSpread)
            firingSpreadRadius = Mathf.Lerp(firingSpreadRadius, 0, currentGun.recoveryTime * Time.deltaTime);

        if(currentGun.maxSpread)
            firingSpreadRadius = 0;

        if(meleeComp.isMeleeing || currentGun.isReloading || moveComp.isRunning || isSwitching) {
            return;
        }

        if(currentGun.isAuto) {
            if(Input.GetButton("Shoot")) {
                if(nextFire <= 0) {
                    Fire();
                    CalculateSpread();
                    nextFire = 1 / currentGun.fireRate;
                    return;
                }
            }
        }
        else {
            if(Input.GetButtonDown("Shoot")) {
                if(nextFire <= 0) {
                    Fire();
                    CalculateSpread();
                    nextFire = 1 / currentGun.fireRate;
                    return;
                }
            }
        }
    }

    RaycastHit[] hits;
    void Fire() {
        AudioManager.instance.Play(currentGun.soundName);
        if(currentGun.upgraded) {
            AudioManager.instance.Play("Upgraded Shot");
        }

        CameraShaker.Instance.ShakeOnce(currentGun.recoil.recoilPower * 10, 8f, .1f, currentGun.recoil.recoilPower);

        currentGun.Fire();

        if(currentGun.primaryShot) {
            currentGun.muzzleFlash.Play();
            if(currentGun.secondaryMuzzleFlash != null) {
                currentGun.primaryShot = !currentGun.primaryShot;
                currentGun.animator.SetBool("PrimFire", currentGun.primaryShot);
            }
        }
        else {
            currentGun.secondaryMuzzleFlash.Play();
            if(currentGun.secondaryMuzzleFlash != null) {
                currentGun.primaryShot = !currentGun.primaryShot;
                currentGun.animator.SetBool("PrimFire", currentGun.primaryShot);
            }
        }

        for(int i = 0; i < currentGun.shots; i++) {
            Ray ray = new Ray(shootPos.position, GetRandomForward());
            hits = Physics.RaycastAll(ray, 100, shootableLayerMask);
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

            int p;
            if(hits.Length <= 0)
                return;
            if(hits.Length <= currentGun.penetration)
                p = hits.Length;
            else
                p = currentGun.penetration;

            List<Enemy> hitList = new List<Enemy>();
            for(int h = 0; h < p; h++) {
                if(hits[h].collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                    var hitEffect = Instantiate(objectImpactEffect, hits[h].point, Quaternion.LookRotation(hits[h].normal));
                    Destroy(hitEffect, 1f);
                    var hitHole = Instantiate(bulletHole, hits[h].point + (hits[h].normal * 0.01f), Quaternion.LookRotation(hits[h].normal));
                    hitHole.SetHitObject(hits[h].collider.gameObject);
                    Destroy(hitHole.gameObject, hitHole.lifeTime);
                    break;
                }
                if(hitList.Contains(hits[h].collider.transform.root.GetComponent<Enemy>())) {
                    p++;
                    continue;
                }

                Enemy e;
                if(hits[h].collider.transform.root.TryGetComponent(out e)) {
                    hitList.Add(e);
                    float d = Vector3.Distance(transform.position, hits[h].point);
                    float damageActual = CalculateDamage(d);
                    for(int ii = 0; ii < h; ii++) {
                        damageActual -= damageActual * 0.05f;
                    }

                    if(hits[h].collider == e.head) {
                        damageActual *= currentGun.headShotMult;
                        e.DamageBody(damageActual, true);
                    }
                    else {
                        foreach(var b in e.body) {
                            if(hits[h].collider == b) {
                                damageActual *= currentGun.torsoShotMult;
                                e.DamageBody(damageActual, false);
                                break;
                            }
                        }
                    }
                }


                if(hits[h].rigidbody != null) {
                    hits[h].rigidbody.AddForce(-hits[h].normal * currentGun.impactForce);
                }

                //effects
                var effect = Instantiate(bloodHitEffect, hits[h].point, Quaternion.LookRotation(hits[h].normal));
                Destroy(effect, 1f);
            }
        }
    }

    void SwitchWeapon() {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.Q)) {
            currentGun.isReloading = false;
            gunIndex--;
            if(gunIndex < 0)
                gunIndex = guns.Count - 1;

            zoom.ResetZoom();
            if(guns.Count > 1) {
                StopAllCoroutines();
                StartCoroutine(SwitchWeaponDelay());
            }
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0) {
            currentGun.isReloading = false;
            gunIndex++;
            if(gunIndex >= guns.Count)
                gunIndex = 0;

            zoom.ResetZoom();
            if(guns.Count > 1) {
                StopAllCoroutines();
                StartCoroutine(SwitchWeaponDelay());
            }
        }
    }

    public void EquipWeapon() {
        if(meleeComp.isMeleeing)
            return;


        meleeComp.currentGun = currentGun = guns[gunIndex];
        for(int i = 0; i < guns.Count; i++) {
            if(i == gunIndex)
                guns[i].gameObject.SetActive(true);
            else
                guns[i].gameObject.SetActive(false);
        }
    }

    public void GiveWeapon(GameObject g) {
        Gun gun = g.GetComponent<Gun>();
        if(guns.Count > 1) {
            Destroy(currentGun.gameObject);
            guns[gunIndex] = gun;
            EquipWeapon();
        }
        else {
            guns.Add(gun);
            gunIndex = 1;
            EquipWeapon();
        }
    }

    float CalculateDamage(float distToTarget_) {
        //within min range
        if(distToTarget_ <= currentGun.minRange)
            return currentGun.damage;

        //past max range
        if(distToTarget_ >= currentGun.maxRange)
            return Mathf.CeilToInt(currentGun.minDamage);

        //within falloff range
        float range = currentGun.maxRange - currentGun.minRange;
        float rangeNorm = ((distToTarget_ - currentGun.minRange) / range);
        return Mathf.CeilToInt(Mathf.Lerp(currentGun.damage, currentGun.minDamage, rangeNorm));
    }

    public Gun GetCurrentGun() {
        return currentGun;
    }

    void CalculateSpread() {
        if(currentGun.isShotgun) { }
        else if(zoom.maxZoom) {
            firingSpreadRadius = 0;
            return;
        }

        firingSpreadRadius += currentGun.hipFireMaxSpread / (currentGun.magazineSize / 2);

        if(firingSpreadRadius > currentGun.hipFireMaxSpread)
            firingSpreadRadius = currentGun.hipFireMaxSpread;

        if(currentGun.maxSpread)
            firingSpreadRadius = currentGun.hipFireMaxSpread;
    }

    Vector3 GetRandomForward() {
        if(!currentGun.maxSpread) {
            if(firingSpreadRadius <= 0.001f || zoom.maxZoom)
                return shootPos.forward;
        }
        else if(zoom.maxZoom && !currentGun.isShotgun) //middle
            return shootPos.forward;

        Vector3 randomForward = shootPos.forward;
        var min = -firingSpreadRadius - currentGun.hipFireBaseSpread;
        var max = firingSpreadRadius + currentGun.hipFireBaseSpread;
        randomForward.x += Random.Range(min, max);
        randomForward.y += Random.Range(min, max);
        randomForward.z += Random.Range(min, max);
        return randomForward.normalized;
    }

    IEnumerator SwitchWeaponDelay() {
        isSwitching = true;

        currentGun.animator.SetTrigger("Switch");
        yield return new WaitForSeconds(0.5f);
        EquipWeapon();
        yield return new WaitForSeconds(currentGun.switchInSpeed);

        isSwitching = false;
    }


}
