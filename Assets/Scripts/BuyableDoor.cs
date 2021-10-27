
using UnityEngine;

public class BuyableDoor : MonoBehaviour {

    public int cost;

    public void BuyDoor() {
        Destroy(gameObject);
    }

}
