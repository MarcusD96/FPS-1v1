using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour {

    public float spawnTime, startDelay;
    public int numSpawn;

    public int maxEnemies;

    public GameObject enemyPrefab;
    public Spawn[] spawns;

    float currentSpawnTime = 0;
    bool waveStarted = false;
    int waveNum = 1;

    List<GameObject> spawnedEnemies = new List<GameObject>();

    private void LateUpdate() {
        if(waveStarted == false)
            StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave() {
        FindObjectOfType<Player>().waveNum.text = waveNum.ToString();

        //start with delay
        waveStarted = true;
        yield return new WaitForSeconds(startDelay);

        //start spawning
        WaitForSeconds spawnDelay = new WaitForSeconds(spawnTime);
        for(int i = 0; i < numSpawn; i++) {
            //check if max 
            while(spawnedEnemies.Count >= maxEnemies)
                yield return null;
            int r = Random.Range(0, spawns.Length);
            GameObject g = Instantiate(enemyPrefab, spawns[r].GetPos(), spawns[r].transform.rotation);
            spawnedEnemies.Add(g);
            yield return spawnDelay;
        }

        //wait for enemies to be killed
        while(spawnedEnemies.Count <= 0)
            yield return null;

        waveStarted = false;
        //increase difficulty
        waveNum++;
    }

    public void RemoveEnemy(GameObject e) {
        spawnedEnemies.Remove(e);
    }
}
