
using UnityEngine;

public class Crosshair : MonoBehaviour {

    public PlayerShoot playerShoot;
    public RectTransform hashUp, hashLeft, hashDown, hashRight; //y, x, -y, -x

    float centerdPos;

    private void Start() {
        centerdPos = hashUp.localPosition.y;
    }

    private void Update() {
        Vector3 pos = hashUp.localPosition;

        //is zoomed in
        if(playerShoot.zoom.maxZoom) {
            pos.y = centerdPos;
            hashUp.localPosition = pos;

            pos = hashLeft.localPosition;
            pos.x = centerdPos;
            hashLeft.localPosition = pos;

            pos = hashDown.localPosition;
            pos.y = -centerdPos;
            hashDown.localPosition = pos;

            pos = hashRight.localPosition;
            pos.x = -centerdPos;
            hashRight.localPosition = pos;
            return;
        }

        //is sniper
        if(playerShoot.GetCurrentGun().maxSpread) {
            float spreadFactor = playerShoot.GetCurrentGun().hipFireBaseSpread * 1000;

            pos.y = centerdPos + spreadFactor;
            hashUp.localPosition = pos;

            pos = hashLeft.localPosition;
            pos.x = centerdPos + spreadFactor;
            hashLeft.localPosition = pos;

            pos = hashDown.localPosition;
            pos.y = -centerdPos - spreadFactor;
            hashDown.localPosition = pos;

            pos = hashRight.localPosition;
            pos.x = -centerdPos - spreadFactor;
            hashRight.localPosition = pos;
            return;
        }

        //increasing hash spread
        {
            float spread = playerShoot.firingSpreadRadius * 1000;
            float baseSpread = playerShoot.currentGun.hipFireBaseSpread * 1000;

            pos.y = baseSpread + spread;
            hashUp.localPosition = pos;

            pos = hashLeft.localPosition;
            pos.x = baseSpread + spread;
            hashLeft.localPosition = pos;

            pos = hashDown.localPosition;
            pos.y = -baseSpread - spread;
            hashDown.localPosition = pos;

            pos = hashRight.localPosition;
            pos.x = -baseSpread - spread;
            hashRight.localPosition = pos;
        }
    }
}
