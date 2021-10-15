
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour {

    [Header("Sampling")]
    public int sampleSize;
    public bool doSamples;
    List<float> times = new List<float>();

    private void Start() {
        InvokeRepeating(nameof(AverageFPS), 0, 5f / sampleSize);
    }

    void AverageFPS() {
        if(!doSamples)
            return;
        if(times.Count == sampleSize) {
            float total = 0;
            foreach(var t in times) {
                total += t;
            }
            total /= sampleSize;
            print("Average DT over 5 seconds: " + total);
            times = new List<float>();
            return;
        }
        times.Add(Time.deltaTime);
    }
}
