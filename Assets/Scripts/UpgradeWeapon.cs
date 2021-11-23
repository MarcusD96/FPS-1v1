
using System.Collections;
using UnityEngine;

public class UpgradeWeapon : MonoBehaviour {

    public float interactDistance, upgradeTime, expiryTime;
    public int upgradeCost;
    public Transform weaponPos;

    bool isUpgrading = false;
    Animator animator;
    Player player;
    PlayerShoot playerShoot;
    Gun upgradedGun;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void LateUpdate() {
        if(player == null) {
            player = PlayerManager.Instance.player;
            if(player)
                playerShoot = player.GetComponent<PlayerShoot>();
            return;
        }

        if(isUpgrading)
            return;

        Vector3 pPos = player.transform.position;
        if(Vector3.Distance(transform.position, pPos) <= interactDistance && !playerShoot.currentGun.upgraded) {
            //enable gui
            player.buyUpgrade.SetActive(true);
            player.buyUpgradeText.text = "Upgrade gun for " + upgradeCost;
            if(Input.GetKeyDown(KeyCode.F) && player.points >= upgradeCost) {
                Upgrade();
            }
        }
        else
            player.buyUpgrade.SetActive(false);
    }

    private void Upgrade() {
        player.points -= upgradeCost;
        StartCoroutine(UpgradeGun());
    }

    //upgrade procedure//
    //(1) take gun, disallow interaction
    //(2) wait for upgrade
    //(3) allow for player to interact again to take weapon
    IEnumerator UpgradeGun() {
        yield return null;
        isUpgrading = true;

        //store upgraded version to give to player
        Gun weaponToGive = upgradedGun = Instantiate(playerShoot.currentGun.upgradedGun, weaponPos);
        weaponToGive.animator.enabled = false;
        weaponToGive.gameObject.SetActive(false);

        //station current weapon to be animated
        Gun weaponToTake = Instantiate(playerShoot.currentGun, weaponPos);
        weaponToTake.animator.enabled = false;

        //take gun from player
        playerShoot.RemoveCurrentWeapon();
        animator.SetTrigger("Upgrade");

        //wait for upgrade
        float upgradeDoneTime = Time.time + upgradeTime;
        float halfway = upgradeDoneTime - (upgradeTime / 2);
        bool switched = false;
        while(Time.time < upgradeDoneTime) {
            player.buyUpgrade.SetActive(false);
            if(Time.time >= halfway && !switched) {
                switched = true;
                Destroy(weaponToTake.gameObject);
                weaponToGive.gameObject.SetActive(true);
            }
            yield return null;
        }

        weaponToGive.gameObject.SetActive(true);

        //allow player to retrieve weapon
        float endTime = Time.time + expiryTime;
        bool retrieved = false;
        while(Time.time < endTime) {
            if(Input.GetKeyDown(KeyCode.F)) {
                retrieved = true;
                playerShoot.GiveWeapon(weaponToGive.gameObject);
                weaponToGive.animator.enabled = true;

                //check if wallgun
                var wgm = FindObjectOfType<WallGunManager>();
                WallGun wg = null;
                foreach(var g in wgm.GetWallGuns()) {
                    if(g.purchasedGun == playerShoot.currentGun) {
                        wg = g;
                        break;
                    }
                }

                if(wg) {
                    wg.purchasedGun = playerShoot.currentGun;
                }
                break;
            }
            yield return null;
        }

        if(!retrieved) {
            Destroy(weaponToGive.gameObject);
        }



        isUpgrading = false;
    }
}
