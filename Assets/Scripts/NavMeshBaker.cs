
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour {

    #region Singleton
    public static NavMeshBaker Instance;

    void Awake() {
        if(Instance) {
            Debug.LogError("more than 1 NavMeshBaker, deleting new one");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    public NavMeshSurface navMeshSurface;
    bool buildMesh = false;

    private void Start() {
        buildMesh = true;
    }

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
