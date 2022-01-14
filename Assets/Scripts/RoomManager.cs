
using UnityEngine;

public class RoomManager : MonoBehaviour {

    #region Singleton
    public static RoomManager Instance;

    private void Awake() {
        if(Instance) {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion

    Room[] rooms;

    private void Start() {
        rooms = new Room[transform.childCount];
        int i = 0;
        foreach(Transform c in transform) {
            rooms[i] = c.GetComponent<Room>();
            i++;
        }
    }

    public Room[] GetRooms() {
        return rooms;
    }

}
