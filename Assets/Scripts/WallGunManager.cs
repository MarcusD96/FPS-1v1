
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
        InvokeRepeating(nameof(FindClosestInteractable), 0, 0.25f);
    }

    void FindClosestInteractable() {
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
            if(!wg.purchased)
                player.buyWeaponText.text = wg.gunName + " - " + wg.gunPrice + " (F)"; 
            else

                player.buyWeaponText.text = wg.gunName + " - " + wg.ammoPrice + " (F)";
        }
    }

    private void Update() {
        if(activeWallGun == null)
            return;

        if(!activeWallGun.purchased)
            BuyGun();
        else
            BuyAmmo();
    }

    void BuyGun() {
        if(Input.GetKeyDown(KeyCode.F) && player.points >= activeWallGun.gunPrice) {
            player.points -= activeWallGun.gunPrice;
            activeWallGun.purchased = true;
            activeWallGun.gunName += " ammo";
            var g = Instantiate(activeWallGun.gunModel, player.handPos);
            activeWallGun.purchasedGun = g.GetComponent<Gun>();
            PlayerShoot shootComp = player.GetComponent<PlayerShoot>();

            if(shootComp.guns.Count > 1) {
                //replace gun
                Destroy(shootComp.currentGun.gameObject);
                shootComp.guns[shootComp.gunIndex] = g.GetComponent<Gun>();
                shootComp.EquipWeapon();
            }
            else {
                //add gun
                shootComp.guns.Add(g.GetComponent<Gun>());
                shootComp.gunIndex = shootComp.guns.Count - 1;
                shootComp.EquipWeapon();
            }
        }
    }

    void BuyAmmo() {
        if(Input.GetKeyDown(KeyCode.F) && player.points >= activeWallGun.ammoPrice) {
            PlayerShoot shootComp = player.GetComponent<PlayerShoot>();
            if(shootComp.currentGun.remainingAmmo >= shootComp.currentGun.magazineSize * 10) {
                return;
            }
            player.points -= activeWallGun.ammoPrice;
            var pShoot = player.GetComponent<PlayerShoot>();
            var gun = pShoot.guns[pShoot.guns.IndexOf(activeWallGun.purchasedGun)];
            gun.remainingAmmo = gun.magazineSize * 10;
        }
    }
}
