
using UnityEngine;

public class WallGunManager : MonoBehaviour {

    public float interactDistance;

    WallGun[] wallGuns;
    WallGun activeWallGun;
    Player player;
    PlayerShoot ps;

    private void Start() {
        wallGuns = new WallGun[transform.childCount];
        player = PlayerManager.Instance.player;
        if(player) {
            ps = player.GetComponent<PlayerShoot>(); 
        }
        for(int i = 0; i < wallGuns.Length; i++) {
            wallGuns[i] = transform.GetChild(i).GetComponent<WallGun>();
        }
        InvokeRepeating(nameof(FindClosestInteractable), 0, 0.25f);
    }

    void FindClosestInteractable() {
        if(player == null) {
            player = PlayerManager.Instance.player;
            if(player) {
                ps = player.GetComponent<PlayerShoot>();
            }
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
            activeWallGun = null;
        }
        else {
            activeWallGun = wg;
        }
    }

    private void Update() {
        if(activeWallGun == null)
            return;

        if(!activeWallGun.purchased)
            BuyGun();
        else {
            if(activeWallGun.purchasedGun.upgraded)
                BuyAmmo(true);
            else
                BuyAmmo(false);
        }
    }

    void BuyGun() {
        if(Input.GetKeyDown(KeyCode.F) && player.points >= activeWallGun.gunPrice) {
            player.points -= activeWallGun.gunPrice;
            AudioManager.instance.PlaySound("$$$", AudioManager.instance.effects);
            activeWallGun.purchased = true;
            var g = Instantiate(activeWallGun.gunModel);
            activeWallGun.purchasedGun = g.GetComponent<Gun>();

            ps.GiveWeapon(g);
        }
    }

    void BuyAmmo(bool upgraded) {
        if(!upgraded) {
            if(Input.GetKeyDown(KeyCode.F) && player.points >= activeWallGun.ammoPrice && activeWallGun.purchasedGun == ps.currentGun) {
                if(ps.currentGun.remainingAmmo >= ps.currentGun.magazineSize * 10) {
                    return;
                }
                player.points -= activeWallGun.ammoPrice;
                AudioManager.instance.PlaySound("$$$", AudioManager.instance.effects);
                var pShoot = player.GetComponent<PlayerShoot>();
                var gun = pShoot.guns[pShoot.guns.IndexOf(activeWallGun.purchasedGun)];
                gun.remainingAmmo = gun.magazineSize * 10;
            }
        }
        else {
            if(Input.GetKeyDown(KeyCode.F) && player.points >= activeWallGun.ammoPrice * 3 && activeWallGun.purchasedGun == ps.currentGun) {
                if(ps.currentGun.remainingAmmo >= ps.currentGun.maxAmmo) {
                    return;
                }
                player.points -= activeWallGun.ammoPrice * 3;
                AudioManager.instance.PlaySound("$$$", AudioManager.instance.effects);
                var pShoot = player.GetComponent<PlayerShoot>();
                var gun = pShoot.guns[pShoot.guns.IndexOf(activeWallGun.purchasedGun)];
                gun.remainingAmmo = gun.magazineSize * 10;
            }
        }
    }

    public WallGun[] GetWallGuns() {
        return wallGuns;
    }
}
