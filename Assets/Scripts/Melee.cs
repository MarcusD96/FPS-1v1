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

    public float meleeRate;
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

            CameraShaker.Instance.ShakeOnce(4f, 8f, .1f, .1f);
            nextMeleeTime = 1 / meleeRate;

            var hits = Physics.OverlapSphere(meleePos.position + meleePos.forward, 1, enemyLayer);

            foreach(var h in hits) {
                if(h.CompareTag("Head"))
                    continue;
                Enemy e;
                if(h.transform.parent.TryGetComponent(out e)) {
                    e.Damage(30);
                    var ii = Instantiate(indicator, indicatorPos.position, Quaternion.identity);
                    ii.transform.SetParent(indicatorPos);
                    ii.Initialize(30, false);
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