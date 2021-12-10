using System.Collections;
using UnityEngine;
using EZCameraShake;

public class Melee : MonoBehaviour {

    public float meleeRate, meleeDist, meleeDamage;
    public float impactForce;

    public LayerMask enemyLayer;
    public Transform meleePos;
    public Gun currentGun;
    public PlayerShoot shootComp;

    [HideInInspector]
    public bool isMeleeing;
    [HideInInspector]
    public bool instaKill;

    float nextMeleeTime = 0;

    private void Start() {
        isMeleeing = false;
    }

    private void Update() {
        if(nextMeleeTime <= 0 && !shootComp.isSwitching) {
            DoMelee();
            return;
        }
        nextMeleeTime -= Time.deltaTime;
    }

    void DoMelee() {
        if(Input.GetKeyDown(KeyCode.Mouse4)) {


            AudioManager.instance.PlaySound("Grunt_" + Random.Range(0, 5), AudioManager.instance.meleeGrunts);

            StartCoroutine(StopForMelee());

            GetComponent<PlayerShoot>().currentGun.animator.SetTrigger("Melee");

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
                    if(instaKill)
                        e.DamageMelee(float.MaxValue);
                    else
                        e.DamageMelee(meleeDamage);
                        break;
                }
            }
        }
    }

    IEnumerator StopForMelee() {
        isMeleeing = true;
        if(currentGun.isReloading) {
            currentGun.StopAllCoroutines();
            currentGun.isReloading = false;
            if(currentGun.isShotgun) {
                currentGun.animator.SetBool("Reloading", currentGun.isReloading);
            }
        }
        currentGun.canReload = false;
        yield return new WaitForSeconds(1 / meleeRate);
        currentGun.canReload = true;
        isMeleeing = false;
    }
}