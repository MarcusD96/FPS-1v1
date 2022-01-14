
using System.Collections;
using UnityEngine;

public class DoublePoints : PowerUp {

    protected override void CollectPowerUp() {
        base.CollectPowerUp();
        StartCoroutine(StartDoublePoints());
    }

    IEnumerator StartDoublePoints() {
        PlayerManager.Instance.killPoints *= 2;
        player.doublePointsIcon.AddTime(powerUpTime);
        yield return new WaitForSeconds(powerUpTime);
        PlayerManager.Instance.killPoints /= 2;
    }
}
