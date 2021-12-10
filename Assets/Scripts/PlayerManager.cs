
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

    #region Controls
    [Header("Movement Controls")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode crouch = KeyCode.LeftControl;
    [Header("Gun Controls")]
    public KeyCode fire = KeyCode.Mouse0;
    public KeyCode ADS = KeyCode.Mouse1;
    public KeyCode melee = KeyCode.Mouse4;
    [Header("Other Controls")]
    public KeyCode useEqipment = KeyCode.E;
    public KeyCode interact = KeyCode.F;
    public KeyCode pause = KeyCode.Escape;
    #endregion

    public Player player;
    public int killPoints;

    private void Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 150;
    }

    public void BodyKillEnemy() {
        player.points += killPoints;
        PointsManager.Instance.SpawnPoints(killPoints);
    }

    public void HeadKillEnemy() {
        player.points += killPoints * 2;
        PointsManager.Instance.SpawnPoints(killPoints * 2);
    }

    public void MeleeKillEnemy() {
        player.points += Mathf.RoundToInt(killPoints * 2.6f);
        PointsManager.Instance.SpawnPoints(Mathf.RoundToInt(killPoints * 2.6f));
    }

    public void HitEnemy() {
        player.points += killPoints / 5;
        PointsManager.Instance.SpawnPoints(killPoints / 5);
    }
}
