
using UnityEngine;
using UnityEngine.ProBuilder;

public class BuyableDoor : Interactable {

    public int cost;

    [HideInInspector]
    public bool purchased;

    ProBuilderMesh mesh;
    MeshRenderer meshRenderer;
    Collider col;

    private void Start() {
        mesh = GetComponent<ProBuilderMesh>();
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
    }

    public void BuyDoor() {
        Destroy(mesh);
        Destroy(meshRenderer);
        Destroy(col);
        purchased = true;
    }

    public override void UpdateInteractText() {
        PlayerManager.Instance.player.ShowInteractionText("Buy door for " + cost + PlayerManager.Instance.interactKey);
    }

    public override void OnInteract() {
        Player p = PlayerManager.Instance.player;
        if(p.points < cost)
            return;

        p.points -= cost;
        AudioManager.instance.PlaySound("$$$", AudioManager.instance.effects);
        BuyDoor();
        foreach(var r in RoomManager.Instance.GetRooms()) {
            if(!r.unlocked) {
                r.CheckRoomAccess();
            }
        }
        NavMeshBaker.Instance.TriggerBuildMesh();
    }
}
