using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EZCameraShake;

public class PlayerShoot : MonoBehaviour {

    public Transform shootPos, indicatorPos;
    public GameObject objectImpactEffect, bloodHitEffect;
    public BulletHole bulletHole, bloodHole;
    public DamageIndicator indicator;
    public LayerMask shootableLayerMask;
    public Gun currentGun;
    public Gun[] guns;
    public TextMeshProUGUI ammoText;
    public Melee meleeComp;
    public PlayerMovement moveComp;
    public PlayerZoom zoom;
    public float firingSpreadRadius;

    float nextFire = 0f;
    int gunIndex = 2;

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

        ammoText.text = currentGun.currentAmmo.ToString() + "/" + currentGun.magazineSize;

        if(meleeComp.isMeleeing || currentGun.isReloading || moveComp.isRunning) {
            firingSpreadRadius = 0;
            return;
        }

        if(nextFire <= -0.25f || currentGun.maxSpread)
            firingSpreadRadius = 0;

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
                    CalculateSpread();
                    Fire();
                    nextFire = 1 / currentGun.fireRate;
                    return;
                }
            }
        }
    }

    void Fire() {
        shootPos.GetComponent<MouseLook>().recoil += currentGun.recoil.recoilPower;
        CameraShaker.Instance.ShakeOnce(currentGun.recoil.recoilPower * 10, 8f, .1f, .1f);

        currentGun.Fire();

        currentGun.muzzleFlash.Play();

        Ray ray = new Ray(shootPos.position, GetRandomForward());
        //single
        {
            /*RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000, shootableLayerMask)) {
                bool hitBody = false;
                Enemy cs;
                if(hit.collider.transform.parent) {
                    if(hit.collider.transform.parent.TryGetComponent(out cs)) {
                        float d = Vector3.Distance(transform.position, hit.point);
                        float damageActual = CalculateDamage(d);

                        if(hit.collider == cs.body) {
                            cs.Damage(damageActual);
                            var i = Instantiate(indicator, indicatorPos.position, Quaternion.identity);
                            i.transform.SetParent(indicatorPos);
                            i.Initialize(damageActual, false);
                            hitBody = true;
                        }
                        else if(hit.collider == cs.head) {
                            cs.Damage(damageActual * 2);
                            var i = Instantiate(indicator, indicatorPos.position, Quaternion.identity);
                            i.transform.SetParent(indicatorPos);
                            i.Initialize(damageActual * 2, true);
                            hitBody = true;
                        }
                    }
                }

                if(hit.rigidbody != null) {
                    hit.rigidbody.AddForce(-hit.normal * currentGun.impactForce);
                }

                //effects
                if(hitBody) {
                    var effect = Instantiate(bloodHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(effect, 1f);
                    var hole = Instantiate(bloodHole, hit.point, Quaternion.LookRotation(hit.normal));
                    hole.SetHitObject(hit.collider.gameObject);
                    Destroy(hole.gameObject, hole.lifeTime);
                }
                else {
                    var effect = Instantiate(objectImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(effect, 1f);
                    var hole = Instantiate(bulletHole, hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
                    hole.SetHitObject(hit.collider.gameObject);
                    Destroy(hole.gameObject, hole.lifeTime);
                }
            }
            */
        }

        RaycastHit[] hits = Physics.RaycastAll(ray, 1000, shootableLayerMask);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        int p;
        if(hits.Length <= 0)
            return;
        if(hits.Length <= currentGun.penetration)
            p = hits.Length;
        else
            p = currentGun.penetration;

        for(int i = 0; i < p; i++) {
            if(hits[i].collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                break;

            if(i > 0 && hits[i].collider.transform.parent == hits[i - 1].collider.transform.parent) {
                p++;
                continue;
            }

            bool hitBody = false;
            Enemy cs;
            if(hits[i].collider.transform.parent) {
                if(hits[i].collider.transform.parent.TryGetComponent(out cs)) {
                    float d = Vector3.Distance(transform.position, hits[i].point);
                    float damageActual = CalculateDamage(d);
                    damageActual /= i + 1;

                    if(hits[i].collider == cs.body) {
                        cs.Damage(damageActual);
                        var ind = Instantiate(indicator, indicatorPos.position, Quaternion.identity);
                        ind.transform.SetParent(indicatorPos);
                        ind.Initialize(damageActual, false);
                        hitBody = true;
                    }
                    else if(hits[i].collider == cs.head) {
                        cs.Damage(damageActual * 2);
                        var ind = Instantiate(indicator, indicatorPos.position, Quaternion.identity);
                        ind.transform.SetParent(indicatorPos);
                        ind.Initialize(damageActual * 2, true);
                        hitBody = true;
                    }
                }
            }

            if(hits[i].rigidbody != null) {
                hits[i].rigidbody.AddForce(-hits[i].normal * currentGun.impactForce);
            }

            //effects
            if(hitBody) {
                var effect = Instantiate(bloodHitEffect, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                Destroy(effect, 1f);
                var hole = Instantiate(bloodHole, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                hole.SetHitObject(hits[i].collider.gameObject);
                Destroy(hole.gameObject, hole.lifeTime);
            }
            else {
                var effect = Instantiate(objectImpactEffect, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                Destroy(effect, 1f);
                var hole = Instantiate(bulletHole, hits[i].point + (hits[i].normal * 0.01f), Quaternion.LookRotation(hits[i].normal));
                hole.SetHitObject(hits[i].collider.gameObject);
                Destroy(hole.gameObject, hole.lifeTime);
            }
        }
    }

    void SwitchWeapon() {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.Q)) {
            currentGun.isReloading = false;
            gunIndex--;
            if(gunIndex < 0)
                gunIndex = guns.Length - 1;

            zoom.ResetZoom();
            EquipWeapon();
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0) {
            currentGun.isReloading = false;
            gunIndex++;
            if(gunIndex >= guns.Length)
                gunIndex = 0;

            zoom.ResetZoom();
            EquipWeapon();
        }
    }

    void EquipWeapon() {
        if(meleeComp.isMeleeing)
            return;


        meleeComp.currentGun = currentGun = guns[gunIndex];
        for(int i = 0; i < guns.Length; i++) {
            if(i == gunIndex)
                guns[i].gameObject.SetActive(true);
            else
                guns[i].gameObject.SetActive(false);
        }
        shootPos.GetComponent<MouseLook>().maxRecoil = currentGun.recoil.recoilPower * 3f;
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
        if(zoom.maxZoom) {
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
        Vector3 randomForward = shootPos.forward;
        randomForward.x += Random.Range(-firingSpreadRadius, firingSpreadRadius);
        randomForward.y += Random.Range(-firingSpreadRadius, firingSpreadRadius);
        randomForward.z += Random.Range(-firingSpreadRadius, firingSpreadRadius);
        return randomForward.normalized;
    }

    bool CanReload() {
        //conditions:
        //not meleeing
        //not running

        //cancels:
        //shoot
        //run

        return true;
    }

    bool CanShoot() {
        //conditions:
        //not running
        //not meleeing
        //not reloading

        //cancels
        //running (with delay)

        return true;
    }
}
