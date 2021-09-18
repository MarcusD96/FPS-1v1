using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public float spawnTime;
    public GameObject enemyPrefab;
    public Spawn[] spawns;

    float currentSpawnTime = 0;

    private void LateUpdate() {
        if(currentSpawnTime <= 0) {
            currentSpawnTime = spawnTime;
            int r = Random.Range(0, spawns.Length);
            int i = spawns.Length;
            while(spawns[r].enemy != null) {
                r++;
                if(r >= spawns.Length)
                    r = 0;
                if(i <= 0)
                    return;
                i--;
            }
            GameObject g = Instantiate(enemyPrefab, spawns[r].GetPos(), Quaternion.identity);
            spawns[r].enemy = g;
            g.transform.SetParent(transform);
            return;
        }
        currentSpawnTime -= Time.deltaTime;
    }

}
