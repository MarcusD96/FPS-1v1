
using UnityEngine;

public class WallGunManager : MonoBehaviour {

    public float interactDistance;

    WallGun[] wallGuns;
    WallGun activeWallGun;
    Player player;

    private void Start() {
        wallGuns = new WallGun[transform.childCount];
        player = PlayerManager.Instance.player;
        for(int i = 0; i < wallGuns.Length; i++) {
            wallGuns[i] = transform.GetChild(i).GetComponent<WallGun>();
        }
        InvokeRepeating(nameof(FindClosestWallGun), 0, 0.25f);
    }

    void FindClosestWallGun() {
        if(player == null) {
            player = PlayerManager.Instance.player;
            return;
        }

        var closestDist = float.MaxValue;
        WallGun wg = null;
        foreach(var g in wallGuns) {
            float d = Vector3.Distance(g.transform.position, player.transform.position);
            if(d < closestDist) {
                closestDist = d;
                wg = g;
            }
        }

        if(closestDist > interactDistance) {
            player.buyWeapon.SetActive(false);
            activeWallGun = null;
        }
        else {
            activeWallGun = wg;
            player.buyWeapon.SetActive(true);
            player.buyWeaponText.text = wg.gunName + " - " + wg.gunPrice;
        }
    }

    private void Update() {
        if(activeWallGun == null)
            return;

        if(!activeWallGun.purchased)
            BuyGun(player);
        else
            BuyAmmo(player);
    }

    void BuyGun(Player p) {
        if(Input.GetKeyDown(KeyCode.F) && p.points >= activeWallGun.gunPrice) {
            p.points -= activeWallGun.gunPrice;
            activeWallGun.purchased = true;
            activeWallGun.gunPrice /= 5;
            activeWallGun.gunPrice = Mathf.RoundToInt(activeWallGun.gunPrice * 5f) / 5;
            activeWallGun.gunName += " ammo";
            var g = Instantiate(activeWallGun.gunModel, p.handPos);
            activeWallGun.purchasedGun = g.GetComponent<Gun>();
            PlayerShoot shootComp = p.GetComponent<PlayerShoot>();
            shootComp.guns.Add(g.GetComponent<Gun>());
            shootComp.gunIndex = shootComp.guns.Count - 1;
            shootComp.EquipWeapon();
        }
    }

    void BuyAmmo(Player p) {
        if(Input.GetKeyDown(KeyCode.F) && p.points >= activeWallGun.gunPrice) {
            PlayerShoot shootComp = p.GetComponent<PlayerShoot>();
            if(shootComp.currentGun.remainingAmmo >= shootComp.currentGun.magazineSize * 5) {
                return;
            }
            p.points -= activeWallGun.gunPrice;
            var pShoot = p.GetComponent<PlayerShoot>();
            var gun = pShoot.guns[pShoot.guns.IndexOf(activeWallGun.purchasedGun)];
            gun.remainingAmmo = gun.magazineSize * 5;
        }
    }
}
