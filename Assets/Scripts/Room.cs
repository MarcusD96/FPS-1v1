
using UnityEngine;

public class Room : MonoBehaviour {

    public bool unlocked;
    public BuyableDoor[] accessDoors;

    //private void Start() {
    //    InvokeRepeating(nameof(SetAccess), 0f, 0.25f);
    //}

    public void CheckRoomAccess() {
        foreach(var d in accessDoors) {
            if(d.purchased) {
                unlocked = true;
                CancelInvoke();
                break;
            }
        }
    }
}
