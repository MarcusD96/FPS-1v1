
using UnityEngine;

public class WallGun : Interactable {

    public string gunName;
    public int gunPrice;
    public int ammoPrice;
    public bool purchased;

    [HideInInspector]
    public string purchasedGunName;
    public GameObject gunModel;

    public Gun purchasedGun;

    private void Awake() {
        purchased = false;
        purchasedGunName = gunName + " ammo";
    }

    void BuyGun() {
        Player p = PlayerManager.Instance.player;
        if(p.points < gunPrice)
            return;
        p.points -= gunPrice;
        AudioManager.instance.PlaySound("$$$", AudioManager.instance.effects);
        purchased = true;
        var g = Instantiate(gunModel);
        purchasedGun = g.GetComponent<Gun>();

        p.GetComponent<PlayerShoot>().GiveWeapon(g);
    }

    void BuyAmmo(bool upgraded_) {
        Player p = PlayerManager.Instance.player;
        PlayerShoot pShoot = p.GetComponent<PlayerShoot>();

        if(purchasedGun != pShoot.currentGun) {
            return;
        }
        if(purchasedGun.remainingAmmo >= purchasedGun.maxAmmo) {
            return;
        }

        if(!upgraded_) {
            if(p.points < ammoPrice)
                return;
            p.points -= ammoPrice;
            AudioManager.instance.PlaySound("$$$", AudioManager.instance.effects);
            purchasedGun.MaxAmmo();

         }
        else {
            if(p.points < ammoPrice * 3)
                return;
            p.points -= ammoPrice * 3;
            AudioManager.instance.PlaySound("$$$", AudioManager.instance.effects);
            purchasedGun.MaxAmmo();
        }
    }

    public override void UpdateInteractText() {
        if(!purchased) {
            PlayerManager.Instance.player.ShowInteractionText("Purchase " + gunName + " for " + gunPrice + " " + PlayerManager.Instance.interactKey);
        }
        else {
            if(!purchasedGun.upgraded)
                PlayerManager.Instance.player.ShowInteractionText("Purchase " + purchasedGun.gunName + " ammo for " + ammoPrice + " " + PlayerManager.Instance.interactKey);
            else
                PlayerManager.Instance.player.ShowInteractionText("Purchase " + purchasedGun.gunName + " ammo for " + ammoPrice * 3 + " " + PlayerManager.Instance.interactKey);
        }
    }

    public override void OnInteract() {
        if(!purchased)
            BuyGun();
        else
            BuyAmmo(purchasedGun.upgraded);
    }
}
