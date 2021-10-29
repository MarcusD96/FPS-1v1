
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour {

    #region Singleton
    public static NavMeshBaker Instance;

    void Awake() {
        if(Instance) {
            Debug.LogWarning("more than 1 NavMeshBaker, deleting new one");
            BuildMesh();
            Destroy(gameObject);
            return;
        }
        Instance = this;
        BuildMesh();
    }
    #endregion

    public NavMeshSurface navMeshSurface;
    bool buildMesh = false;

    private void FixedUpdate() {
        if(buildMesh)
            BuildMesh();
    }

    public void TriggerBuildMesh() {
        buildMesh = true;
    }

    void BuildMesh() {
        navMeshSurface.BuildNavMesh();
        buildMesh = false;
    }
}
