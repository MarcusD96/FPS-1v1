
using UnityEngine;
using UnityEngine.ProBuilder;

public class BuyableDoor : MonoBehaviour {

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
}
