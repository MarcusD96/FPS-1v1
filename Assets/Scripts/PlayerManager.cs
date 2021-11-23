
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
    [Header("Movement Controls")]
    public KeyCode forward;
    public KeyCode back, left, right, sprint, crouch;
    [Header("Gun Controls")]
    public KeyCode fire;
    public KeyCode ADS, melee;
    [Header("Other Controls")]
    public KeyCode useEqipment;
    public KeyCode interact, pause;

    private void Start() {
        Application.targetFrameRate = 150;
    }
}
