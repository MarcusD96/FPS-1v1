using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour {

    public float spawnTime, startDelay;
    public int numSpawn;

    public int maxEnemies;

    public GameObject enemyPrefab;
    Spawn[] spawns;

    bool waveStarted = false;
    [SerializeField]
    int waveNum = 29;

    List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start() {
        spawns = new Spawn[transform.childCount];
        for(int i = 0; i < spawns.Length; i++) {
            spawns[i] = transform.GetChild(i).GetComponent<Spawn>();
        }
    }

    private void LateUpdate() {
        if(waveStarted == false)
            StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave() {
        FindObjectOfType<Player>().waveNum.text = waveNum.ToString();

        //start with delay
        waveStarted = true;
        yield return new WaitForSeconds(startDelay);

        CalculateNextRound();
        FindObjectOfType<Player>().waveNum.text = waveNum.ToString();

        //start spawning
        WaitForSeconds spawnDelay = new WaitForSeconds(spawnTime);
        for(int i = 0; i < numSpawn; i++) {
            //check if max 
            while(spawnedEnemies.Count >= maxEnemies)
                yield return null;
            int r = Random.Range(0, spawns.Length);
            //only spawn in rooms that are unlocked
            while(!spawns[r].roomAccess.unlocked) {
                r++;
                if(r >= spawns.Length)
                    r = 0;
            }
            print(r);
            
            GameObject g = Instantiate(enemyPrefab, spawns[r].GetPos(), spawns[r].transform.rotation);
            spawnedEnemies.Add(g);

            //last to spawn must be runner...if after round 3
            if(i < numSpawn - 1) {
                g.GetComponent<Enemy>().Initialize(waveNum, false);
            }
            else
                g.GetComponent<Enemy>().Initialize(waveNum, true);

            yield return spawnDelay;
        }

        //wait for enemies to be killed
        bool lastRunning = false;
        while(spawnedEnemies.Count > 0) {
            if(spawnedEnemies.Count == 1 && !lastRunning) {
                lastRunning = true;
                spawnedEnemies[0].GetComponent<EnemyAI>().ChangeToRunner();
            }
            yield return null;
        }


        waveStarted = false;
    }

    void CalculateNextRound() {
        waveNum++;

        if(waveNum < 10) {
            numSpawn = 6 + ((waveNum - 1) * 2);
            spawnTime -= 0.2f;
        }
        else if(waveNum < 25) {
            numSpawn = 22 + ((waveNum - 1) * 3);
            spawnTime = 1.5f;
        }

        else if(waveNum >= 25) {
            numSpawn = 137 + ((waveNum - 1) * 5);
            spawnTime = 0.75f;
        }
    }

    public void RemoveEnemy(GameObject e) {
        spawnedEnemies.Remove(e);
    }
}
