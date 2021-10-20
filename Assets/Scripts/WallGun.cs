
using UnityEngine;

public class WallGun : MonoBehaviour {

    public string gunName;
    public int gunPrice;
    [HideInInspector]
    public bool purchased;
    
    public GameObject gunModel;
    [HideInInspector]
    public Gun purchasedGun;

    private void Awake() {
        purchased = false;
    }
}
