
using System.Collections;
using UnityEngine;
using HelperFuntions;

public class UpgradeWeapon : Interactable {

    public float interactDistance, upgradeTime, expiryTime;
    public int upgradeCost;
    public Transform weaponPos;

    bool retrivedWeapon;
    Animator animator;
    Player player;
    PlayerShoot playerShoot;
    Gun upgradedGun;
    WallGun ownedWallgun;
    UpgradeState upgradeState;

    private void Start() {
        animator = GetComponent<Animator>();
        upgradeState = UpgradeState.AVAILABLE;
    }

    private void LateUpdate() {
        if(player == null) {
            player = PlayerManager.Instance.player;
            if(player)
                playerShoot = player.GetComponent<PlayerShoot>();
        }
    }

    private void Upgrade() {
        if(player.points < upgradeCost)
            return;

        player.points -= upgradeCost;
        StartCoroutine(UpgradeGun());
    }

    private void RetrieveUpgradedGun() {
        retrivedWeapon = true;
        playerShoot.GiveWeapon(upgradedGun.gameObject);
        upgradedGun.animator.enabled = true;

        if(ownedWallgun != null) {
            ownedWallgun.purchasedGun = playerShoot.currentGun;
        }

        ownedWallgun = null;
        upgradedGun = null;
    }

    //upgrade procedure//
    //(1) take gun, disallow interaction
    //(2) wait for upgrade
    //(3) allow for player to interact again to take weapon
    IEnumerator UpgradeGun() {

        //store upgraded version to give to player
        upgradedGun = Instantiate(playerShoot.currentGun.upgradedGun, weaponPos);
        upgradedGun.animator.enabled = false;

        //find corresponding wallgun to see if it is owned
        var wgs = FindObjectsOfType<WallGun>();
        foreach(var g in wgs) {
            if(g.purchasedGun == null)
                continue;
            if(g.purchasedGun.gunID == playerShoot.currentGun.gunID) {
                ownedWallgun = g;
                g.purchasedGun = upgradedGun;
                break;
            }
        }

        //change upgraded gun layer to default
        int saveLayer = upgradedGun.transform.GetChild(0).gameObject.layer;
        foreach(var g in Helpers.AllChilds(upgradedGun.gameObject)) {
            if(g.layer != saveLayer)
                continue;
            g.layer = 0;
        }

        upgradedGun.gameObject.SetActive(false);

        //station current weapon sto be animated
        Gun takenGun = Instantiate(playerShoot.currentGun, weaponPos);
        takenGun.animator.enabled = false;

        //change taken gun layer to default
        foreach(var g in Helpers.AllChilds(takenGun.gameObject)) {
            g.layer = 0;
        }

        //take gun from player
        playerShoot.RemoveCurrentWeapon();
        animator.SetTrigger("Upgrade");
        upgradeState = UpgradeState.UPGRADING;

        //wait for upgrade
        float upgradeDoneTime = Time.time + upgradeTime;
        float halfway = upgradeDoneTime - (upgradeTime / 2);
        bool switched = false;
        while(Time.time < upgradeDoneTime) {
            if(Time.time >= halfway && !switched) {
                switched = true;
                Destroy(takenGun.gameObject);
                upgradedGun.gameObject.SetActive(true);
            }
            yield return null;
        }

        //restore upgraded gun layers
        foreach(var g in Helpers.AllChilds(upgradedGun.gameObject)) {
            if(g.layer != 0)
                continue;
            g.layer = saveLayer;
        }
        upgradedGun.gameObject.SetActive(true);
        upgradeState = UpgradeState.RETRIEVABLE;

        //allow player to retrieve weapon
        retrivedWeapon = false;
        float endTime = Time.time + expiryTime;
        while(Time.time < endTime) {
            if(retrivedWeapon)
                break;
            yield return null;
        }

        if(!retrivedWeapon)
            Destroy(upgradedGun.gameObject);
        {
            ownedWallgun = null;
            upgradedGun = null;
        }

        upgradeState = UpgradeState.AVAILABLE;
    }

    public override void UpdateInteractText() {
        switch(upgradeState) {
            case UpgradeState.AVAILABLE:
                if(playerShoot.currentGun.upgraded)
                    player.HideInteractionText();
                else
                    player.ShowInteractionText("Upgrade " + playerShoot.currentGun.gunName + " for " + upgradeCost + " " + PlayerManager.Instance.interactKey);
                break;
            case UpgradeState.UPGRADING:
                player.HideInteractionText();
                break;
            case UpgradeState.RETRIEVABLE:
                player.ShowInteractionText("Retrieve " + upgradedGun.gunName + " " + PlayerManager.Instance.interactKey);
                break;
            default:
                break;
        }
    }

    public override void OnInteract() {
        switch(upgradeState) {
            case UpgradeState.AVAILABLE:
                if(playerShoot.currentGun.upgraded)
                    return;
                Upgrade();
                break;
            case UpgradeState.UPGRADING:
                break;
            case UpgradeState.RETRIEVABLE:
                RetrieveUpgradedGun();
                break;
            default:
                break;
        }
    }
}

public enum UpgradeState {
    AVAILABLE,
    UPGRADING,
    RETRIEVABLE
}
