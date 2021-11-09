
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour {

    public float interactDistance;
    public Room[] rooms;

    List<BuyableDoor> doors = new List<BuyableDoor>();
    BuyableDoor activeDoor;
    Player player;

    private void Start() {
        player = PlayerManager.Instance.player;
        for(int i = 0; i < transform.childCount; i++) {
            doors.Add(transform.GetChild(i).GetComponent<BuyableDoor>());
        }
        InvokeRepeating(nameof(FindClosestDoor), 0, 0.25f);
    }

    void FindClosestDoor() {
        if(player == null) {
            player = PlayerManager.Instance.player;
            return;
        }

        var closestDist = float.MaxValue;
        BuyableDoor thisDoor = null;
        foreach(var g in doors) {
            float d = Vector3.Distance(g.transform.position, player.transform.position);
            if(d < closestDist) {
                closestDist = d;
                thisDoor = g;
            }
        }

        if(closestDist > interactDistance) {
            player.buyDoor.SetActive(false);
            activeDoor = null;
        }
        else {
            activeDoor = thisDoor;
            player.buyDoor.SetActive(true);
            player.buyDoorText.text = "Buy door for: " + activeDoor.cost + " (F)";
        }
    }

    private void Update() {
        if(activeDoor) {
            if(Input.GetKeyDown(KeyCode.F))
                BuyDoor();
        }
    }

    void BuyDoor() {
        if(player.points < activeDoor.cost)
            return;

        player.points -= activeDoor.cost;
        doors.Remove(activeDoor);
        activeDoor.BuyDoor();
        foreach(var r in rooms) {
            if(!r.unlocked) {
                r.CheckAccess();
            }
        }
        NavMeshBaker.Instance.TriggerBuildMesh();
    }
}
