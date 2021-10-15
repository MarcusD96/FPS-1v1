
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    #region Singleton
    public static PlayerManager Instance;

    private void Awake() {
        if(Instance) {
            Debug.LogError("More than 1 PlayerManager...deleting...");
            Destroy(this);
            return;
        }
        Instance = this;    
    }

    private void LateUpdate() {
        if(!player)
            player = FindObjectOfType<Player>();
    }
    #endregion

    public Player player;

}
