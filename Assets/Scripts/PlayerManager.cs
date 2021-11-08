
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    #region Singleton
    public static PlayerManager Instance;

    private void Awake() {
        if(Instance) {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate() {
        if(player == null)
            player = FindObjectOfType<Player>();
    }
    #endregion

    public Player player;

    private void Start() {
        Application.targetFrameRate = 150;
    }
}
