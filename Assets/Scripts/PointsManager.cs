
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour {

    #region Singleton
    public static PointsManager Instance;

    private void Awake() {
        if(Instance) {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }
    #endregion

    public PointsEffectText pointsPrefab;
    public RectTransform pointsTransform;
    public int numToSpawn;

    List<PointsEffectText> pointTexts = new List<PointsEffectText>();

    private void Start() {
        StartCoroutine(SpawnTextsPool());
    }

    IEnumerator SpawnTextsPool() {
        for(int i = 0; i < numToSpawn; i++) {
            var g = Instantiate(pointsPrefab, transform);
            g.gameObject.SetActive(false);
            pointTexts.Add(g);
            yield return null;
        }
    }

    public void SpawnPoints(int points_) {
        foreach(var p in pointTexts) {
            if(!p.gameObject.activeSelf) {
                p.InitializeText(points_, pointsTransform.position);
                return;
            }
        }
    }

}