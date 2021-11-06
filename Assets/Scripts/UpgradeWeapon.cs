
using UnityEngine;

public class UpgradeWeapon : MonoBehaviour {

    public float interactDistance, upgradeTime;
    public int upgradeCost;

    bool isUpgrading = false, finishedUpgrading = false;
    Player player;
    Gun upgradedGun;

    private void Start() {
        player = PlayerManager.Instance.player;
    }

    private void LateUpdate() {
        if(isUpgrading)
            return;

        if(player == null) {
            player = PlayerManager.Instance.player;
            return;
        }

        Vector3 pPos = player.transform.position;
        if(Vector3.Distance(transform.position, pPos) <= interactDistance && !player.GetComponent<PlayerShoot>().currentGun.upgraded) {
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

    //upgrade procedure//
    //(1) take gun, disallow interaction
    //(2) wait for upgrade
    //(3) allow for player to interact again to take weapon

    private void Upgrade() {
        player.points -= upgradeCost;
        var pShoot = player.GetComponent<PlayerShoot>();
        upgradedGun = pShoot.currentGun.upgradedGun;
        SwapWallGunPurchase(pShoot);
    }

    void SwapWallGunPurchase(PlayerShoot ps) {
        var wgm = FindObjectOfType<WallGunManager>();
        WallGun wg = null;
        foreach(var g in wgm.GetWallGuns()) {
            if(g.purchasedGun == ps.currentGun) {
                wg = g;
                break;
            }
        }
        Destroy(ps.currentGun.gameObject);
        ps.currentGun = Instantiate(upgradedGun, player.handPos);
        ps.guns[ps.gunIndex] = ps.currentGun;
        if(wg != null) {
            wg.purchasedGun = ps.currentGun; 
        }

    }
}
