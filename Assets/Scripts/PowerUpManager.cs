
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

    #region Singleton
    public static PowerUpManager Instance;

    private void Awake() {
        if(Instance) {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }
    #endregion

    public PowerUp[] powerUps;

    float chanceToDrop;
    float startCTD;
    int totalWeights = 0;
    int numToSpawn = 0;
    int roundEnemies = 0;

    private void Start() {
        for(int i = 0; i < powerUps.Length; i++) {
            totalWeights += powerUps[i].weightChance;
        }
    }

    public void TryToDropPowerUp(Transform transform) {
        if(numToSpawn <= 0)
            return;

        int r = Random.Range(0, Mathf.RoundToInt(1f / chanceToDrop));
        if(r == 0)
            DropPowerUp(transform);
        else {
            float addChance = (1f - startCTD) / (float) roundEnemies;
            chanceToDrop += addChance;
        }
    }

    public void DropPowerUp(Transform enemy) {
        numToSpawn--;
        int r = Random.Range(0, totalWeights);
        foreach(var p in powerUps) {
            if(r < p.weightChance) {
                var powerup = Instantiate(p, enemy.position, Quaternion.identity);
                return;
            }
            r -= p.weightChance;
        }
    }

    public void InitializeChanceToSpawn(int waveNum, int spawnNum) {
        roundEnemies = spawnNum;

        if(waveNum <= 5) {
            numToSpawn = 1;
            chanceToDrop = 1f / (float) spawnNum;
        }
        else if(waveNum > 5 && waveNum < 15) {
            numToSpawn = 3;
            chanceToDrop = 1f / ((float) spawnNum * 1.5f);
        }
        else {
            numToSpawn = 5;
            chanceToDrop = 1f / ((float) spawnNum * 2f);
        }
        startCTD = chanceToDrop;
    }
}
