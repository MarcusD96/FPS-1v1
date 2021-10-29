﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour {

    public float spawnTime, startDelay;
    public int numSpawn;

    public int maxEnemies;

    public GameObject enemyPrefab;
    public Spawn[] spawns;

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

            //last to spawn must be runner...if after round 3
            if(i < numSpawn - 1) {
                g.GetComponent<Enemy>().Initialize(waveNum, false);
            }
            else
                g.GetComponent<Enemy>().Initialize(waveNum, true);

            yield return spawnDelay;
        }

        //wait for enemies to be killed
        while(spawnedEnemies.Count > 0)
            yield return null;

        IncreaseDifficulty();

        waveStarted = false;
    }

    void IncreaseDifficulty() {
        waveNum++;

        if(waveNum < 10)
            numSpawn += 2;
        else if(waveNum < 20) {
            numSpawn += 5;
        }
        else if(waveNum >= 30) {
            numSpawn += 10;
        }

        if(spawnTime > 1.0f) {
            spawnTime -= 0.2f;
        }
    }

    public void RemoveEnemy(GameObject e) {
        spawnedEnemies.Remove(e);
    }
}