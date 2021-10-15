using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EZCameraShake;

public class PlayerShoot : MonoBehaviour {

    public Transform shootPos, indicatorPos;
    public GameObject objectImpactEffect, bloodHitEffect;
    public BulletHole bulletHole;
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


        if(nextFire <= -0.1f && !currentGun.maxSpread)
            firingSpreadRadius = Mathf.Lerp(firingSpreadRadius, 0, currentGun.recoveryTime * Time.deltaTime);

        if(currentGun.maxSpread)
            firingSpreadRadius = 0;

        if(meleeComp.isMeleeing || currentGun.isReloading || moveComp.isRunning) {
            return;
        }

        if(currentGun.isAuto) {
            if(Input.GetButton("Shoot")) {
                if(nextFire <= 0) {
                    Fire();
                    AudioManager.instance.PlayEffect(AudioManager.instance.sounds, currentGun.soundName, transform.position);
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
                    AudioManager.instance.PlayEffect(AudioManager.instance.sounds, currentGun.soundName, transform.position);
                    CalculateSpread();
                    nextFire = 1 / currentGun.fireRate;
                    return;
                }
            }
        }
    }

    void Fire() {
        CameraShaker.Instance.ShakeOnce(currentGun.recoil.recoilPower * 10, 8f, .1f, currentGun.recoil.recoilPower);

        currentGun.Fire();

        currentGun.muzzleFlash.Play();

        Ray ray = new Ray(shootPos.position, GetRandomForward());
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
            if(hits[i].collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                var hitEffect = Instantiate(objectImpactEffect, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                Destroy(hitEffect, 1f);
                var hitHole = Instantiate(bulletHole, hits[i].point + (hits[i].normal * 0.01f), Quaternion.LookRotation(hits[i].normal));
                hitHole.SetHitObject(hits[i].collider.gameObject);
                Destroy(hitHole.gameObject, hitHole.lifeTime);
                break;
            }

            if(i > 0 && hits[i].collider.transform.parent == hits[i - 1].collider.transform.parent) {
                p++;
                continue;
            }

            Enemy e;
            if(hits[i].collider.transform.root.TryGetComponent(out e)) {
                float d = Vector3.Distance(transform.position, hits[i].point);
                float damageActual = CalculateDamage(d);
                damageActual /= i + 1;

                if(hits[i].collider == e.head) {
                    damageActual *= 2.0f;
                    var ind = Instantiate(indicator, indicatorPos.position, Quaternion.identity);
                    ind.transform.SetParent(indicatorPos);
                    if(e.hp - damageActual <= 0)
                        ind.Initialize(damageActual, true, true);
                    else
                        ind.Initialize(damageActual, true, false);
                    e.Damage(damageActual);
                }
                else {
                    foreach(var b in e.body) {
                        if(hits[i].collider == b) {
                            var ind = Instantiate(indicator, indicatorPos.position, Quaternion.identity);
                            ind.transform.SetParent(indicatorPos);
                            if(e.hp - damageActual <= 0)
                                ind.Initialize(damageActual, false, true);
                            else
                                ind.Initialize(damageActual, false, false);
                            e.Damage(damageActual);
                            break;
                        }
                    }
                }
            }


            if(hits[i].rigidbody != null) {
                hits[i].rigidbody.AddForce(-hits[i].normal * currentGun.impactForce);
            }

            //effects
            var effect = Instantiate(bloodHitEffect, hits[i].point, Quaternion.LookRotation(hits[i].normal));
            Destroy(effect, 1f);
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
        if(!currentGun.maxSpread) {
            if(firingSpreadRadius <= 0.001f || zoom.maxZoom)
                return randomForward;
        }
        else
            if(zoom.maxZoom)
            return randomForward;

        var min = -firingSpreadRadius - currentGun.hipFireBaseSpread;
        var max = firingSpreadRadius + currentGun.hipFireBaseSpread;
        randomForward.x += Random.Range(min, max);
        randomForward.y += Random.Range(min, max);
        randomForward.z += Random.Range(min, max);
        return randomForward.normalized;
    }
}
