
using UnityEngine;

public class Crosshair : MonoBehaviour {

    public PlayerShoot playerShoot;
    public RectTransform hashUp, hashLeft, hashDown, hashRight; //y, x, -y, -x

    float originalPos;

    private void Start() {
        originalPos = hashUp.localPosition.y;
    }

    private void Update() {
        Vector3 pos = hashUp.localPosition;

        if(playerShoot.zoom.maxZoom) {
            pos.y = originalPos;
            hashUp.localPosition = pos;

            pos = hashLeft.localPosition;
            pos.x = originalPos;
            hashLeft.localPosition = pos;

            pos = hashDown.localPosition;
            pos.y = -originalPos;
            hashDown.localPosition = pos;

            pos = hashRight.localPosition;
            pos.x = -originalPos;
            hashRight.localPosition = pos;
            return;
        }

        if(playerShoot.GetCurrentGun().maxSpread) {
            float spreadFactor = playerShoot.GetCurrentGun().hipFireMaxSpread * 1000;

            pos.y = originalPos + spreadFactor;
            hashUp.localPosition = pos;

            pos = hashLeft.localPosition;
            pos.x = originalPos + spreadFactor;
            hashLeft.localPosition = pos;

            pos = hashDown.localPosition;
            pos.y = -originalPos - spreadFactor;
            hashDown.localPosition = pos;

            pos = hashRight.localPosition;
            pos.x = -originalPos - spreadFactor;
            hashRight.localPosition = pos;
            return;
        }

        float spread = playerShoot.firingSpreadRadius * 1000;

        pos.y = originalPos + spread;
        hashUp.localPosition = pos;

        pos = hashLeft.localPosition;
        pos.x = originalPos + spread;
        hashLeft.localPosition = pos;

        pos = hashDown.localPosition;
        pos.y = -originalPos - spread;
        hashDown.localPosition = pos;

        pos = hashRight.localPosition;
        pos.x = -originalPos - spread;
        hashRight.localPosition = pos;
    }
}
