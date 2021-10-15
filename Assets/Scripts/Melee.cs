using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Melee : MonoBehaviour {

    public LayerMask enemyLayer;
    public DamageIndicator indicator;
    public Transform meleePos, indicatorPos;
    public Gun currentGun;
    public PlayerShoot shootComp;

    public float meleeRate, meleeDist;
    public float impactForce;

    [HideInInspector]
    public bool isMeleeing;

    float nextMeleeTime = 0;

    private void Start() {
        isMeleeing = false;
    }

    private void Update() {
        if(nextMeleeTime <= 0) {
            DoMelee();
            return;
        }
        nextMeleeTime -= Time.deltaTime;
    }

    void DoMelee() {
        if(Input.GetKeyDown(KeyCode.C)) {

            StartCoroutine(StopForMelee());

            GetComponent<PlayerShoot>().currentGun.GetComponent<Animator>().SetTrigger("Melee");

            CameraShaker.Instance.ShakeOnce(6f, 8f, .15f, .15f);
            nextMeleeTime = 1 / meleeRate;

            var hits = Physics.OverlapSphere(meleePos.position + (meleePos.forward * meleeDist), 1, enemyLayer);

            Enemy e;
            for(int i = 0; i < hits.Length; i++) {
                if(i > 0) {
                    if(hits[i].transform.root == hits[i - 1].transform.root)
                        continue;
                }
                if(hits[i].transform.root.TryGetComponent(out e)) {
                    var ii = Instantiate(indicator, indicatorPos.position, Quaternion.identity);
                    ii.transform.SetParent(indicatorPos);
                    if(e.hp - 30 <= 0)
                        ii.InitializeMelee(30, true);
                    else
                        ii.InitializeMelee(30, false);
                    e.Damage(30);
                }
            }
        }
    }

    IEnumerator StopForMelee() {
        isMeleeing = true;
        if(currentGun.isReloading) {
            currentGun.StopAllCoroutines();
            currentGun.isReloading = false;
        }
        currentGun.canReload = false;
        yield return new WaitForSeconds(1 / meleeRate);
        currentGun.canReload = true;
        isMeleeing = false;
    }

    bool CanMelee() {
        //Conditions:
        //not switching weapon

        //Stop Actions:
        //Shoot, Run

        return true;
    }
}