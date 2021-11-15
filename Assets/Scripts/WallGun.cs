﻿
using UnityEngine;

public class WallGun : MonoBehaviour {

    public string gunName;
    public int gunPrice;
    public int ammoPrice;
    public bool purchased;

    [HideInInspector]
    public string purchasedGunName;
    public GameObject gunModel;
    [HideInInspector]
    public Gun purchasedGun;

    private void Awake() {
        purchased = false;
        purchasedGunName = gunName + " ammo";
    }

    private void LateUpdate() {
        if(purchased)
            if(purchasedGun == null)
                purchased = false;
    }

}
