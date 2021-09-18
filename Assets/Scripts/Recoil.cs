using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour {

    public float recoil;
    public float maxRecoil_X;
    public float recoilSpeed;
    public float recoilPower;

    private float recoilTime;
    private float maxRecoil_Y;

    public void StartRecoil() {
        maxRecoil_Y = Random.Range(-maxRecoil_X / 2, maxRecoil_X / 2);
        recoilTime = recoil;
    }

    void Recoiling() {
        if(recoilTime > 0f) {
            Quaternion maxRecoil = Quaternion.Euler(maxRecoil_X, maxRecoil_Y, 0f);
            //dampen towards target rot
            transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
        }
        else {
            recoilTime = 0f;
            //dampen towards target rot
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * recoilSpeed / 2);
        }
        recoilTime -= Time.deltaTime;
    }

    private void Update() {
        Recoiling();
    }

}
