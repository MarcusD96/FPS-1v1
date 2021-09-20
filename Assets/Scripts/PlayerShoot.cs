﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

    public Transform shootPos, indicatorPos;
    public GameObject hitEffect;
    public DamageIndicator indicator;
    public LayerMask shootableLayerMask;
    public Gun currentGun;
    public Gun[] guns;
    public TextMeshProUGUI ammoText;
    public CameraShake cameraShake;
    public Melee meleeComp;

    float nextFire = 0;
    int gunIndex = 0;

    private void Start() {
        EquipWeapon();
    }

    private void Update() {
        SwitchWeapon();
        Shoot();
    }

    void Shoot() {
        ammoText.text = currentGun.currentAmmo.ToString() + "/" + currentGun.magazineSize;

        if(meleeComp.isMeleeing)
            return;

        if(currentGun.isReloading)
            return;

        if(currentGun.isAuto) {
            if(Input.GetButton("Shoot")) {
                if(nextFire <= 0) {
                    Fire();
                    nextFire = 1 / currentGun.fireRate;
                    return;
                }
            }
        }
        else {
            if(Input.GetButtonDown("Shoot")) {
                if(nextFire <= 0) {
                    Fire();
                    nextFire = 1 / currentGun.fireRate;
                    return;
                }
            }
        }
        nextFire -= Time.deltaTime;
    }

    void Fire() {
        shootPos.GetComponent<MouseLook>().recoil += currentGun.recoil.recoilPower;
        StartCoroutine(cameraShake.Shake(0.08f, 0.05f));

        currentGun.Fire();

        currentGun.muzzleFlash.Play();

        RaycastHit hit;
        Ray ray = new Ray(shootPos.position, shootPos.forward);
        if(Physics.Raycast(ray, out hit, 1000, shootableLayerMask)) {
            CharacterStats cs;
            if(hit.collider.transform.parent.TryGetComponent(out cs)) {
                float d = Vector3.Distance(transform.position, hit.point);
                float damageActual = CalculateDamage(d);

                if(hit.collider == cs.body) {
                    cs.Damage(damageActual);
                    var i = Instantiate(indicator, indicatorPos.position, Quaternion.identity);
                    i.transform.SetParent(indicatorPos);
                    i.Initialize(damageActual, false);
                }
                else if(hit.collider == cs.head) {
                    cs.Damage(damageActual * 2);
                    var i = Instantiate(indicator, indicatorPos.position, Quaternion.identity);
                    i.transform.SetParent(indicatorPos);
                    i.Initialize(damageActual * 2, true);
                }

            }
            if(hit.rigidbody != null) {
                hit.rigidbody.AddForce(-hit.normal * currentGun.impactForce);
            }
            var e = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(e, 1f);
        }
    }

    void SwitchWeapon() {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.Q)) {
            currentGun.isReloading = false;
            gunIndex--;
            if(gunIndex < 0)
                gunIndex = guns.Length - 1;

            EquipWeapon();
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0) {
            currentGun.isReloading = false;
            gunIndex++;
            if(gunIndex >= guns.Length)
                gunIndex = 0;
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
            return Mathf.CeilToInt(currentGun.damage * 0.3f);

        //within falloff range
        float range = currentGun.maxRange - currentGun.minRange;
        float rangeNorm = ((distToTarget_ - currentGun.minRange) / range);
        //print("Distance: " + distToTarget_ + "\nRange: " + range + "\nNorm: " + rangeNorm);
        return Mathf.CeilToInt(Mathf.Lerp(currentGun.damage, currentGun.damage * 0.3f, rangeNorm));
    }

    public Gun GetCurrentGun() {
        return currentGun;
    }

}
