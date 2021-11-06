
using UnityEngine;

public class WallGun : MonoBehaviour {

    public string gunName;
    public int gunPrice;
    public int ammoPrice;
    public bool purchased;

    public GameObject gunModel;
    [HideInInspector]
    public Gun purchasedGun;

    private void Awake() {
        purchased = false;
    }

    private void LateUpdate() {
        if(purchased)
            if(purchasedGun == null)
                purchased = false;
    }
}
