using System.Collections;
using System.Collections.Generic;
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
    #endregion

    public Player player;

}
