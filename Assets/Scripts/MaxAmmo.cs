

public class MaxAmmo : PowerUp {

    protected override void CollectPowerUp() {
        base.CollectPowerUp();
        foreach(var g in player.GetComponent<PlayerShoot>().guns) {
            g.remainingAmmo = g.maxAmmo;
        }
    }
}
