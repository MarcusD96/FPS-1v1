using System.Collections;
using UnityEngine;

public class InstaKill : PowerUp {
    protected override void CollectPowerUp() {
        base.CollectPowerUp();
        StartCoroutine(StartInstaKill());
    }

    IEnumerator StartInstaKill() {
        player.GetComponent<PlayerShoot>().instaKill = true;
        player.GetComponent<Melee>().instaKill = true;
        yield return new WaitForSeconds(powerUpTime);
        player.GetComponent<PlayerShoot>().instaKill = false;
        player.GetComponent<Melee>().instaKill = false;
    }
}
