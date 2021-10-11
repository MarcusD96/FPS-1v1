using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public float spawnTime;
    public GameObject enemyPrefab;
    public Spawn[] spawns;
    public bool waitForKill;

    float currentSpawnTime = 0;

    private void LateUpdate() {
        if(Settings.Paused)
            return;

        if(currentSpawnTime <= 0) {
            currentSpawnTime = spawnTime;
            int r = Random.Range(0, spawns.Length);
            int i = spawns.Length;
            if(waitForKill) {
                while(spawns[r].enemy != null) {
                    r++;
                    if(r >= spawns.Length)
                        r = 0;
                    if(i <= 0)
                        return;
                    i--;
                }
            }
            GameObject g = Instantiate(enemyPrefab, spawns[r].GetPos(), Quaternion.identity);
            spawns[r].enemy = g;
            g.transform.SetParent(transform);
            return;
        }
        currentSpawnTime -= Time.deltaTime;
    }

}
