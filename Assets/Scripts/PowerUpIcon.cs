
using System.Collections;
using UnityEngine;

public class PowerUpIcon : MonoBehaviour {

    public GameObject iconImage;
    public float flashLength = 0.1f;

    float timeRemaining = 0, flashInterval, lastFlashTime;

    private void Update() {
        if(timeRemaining <= 0) {
            flashInterval = 100f;  
            iconImage.SetActive(false);
            return;
        }
        if(timeRemaining > 10f) {
            flashInterval = 100f;
            iconImage.SetActive(true);  
        }
        if(timeRemaining <= 10f)
            flashInterval = 1f;
        if(timeRemaining <= 5f)
            flashInterval = 0.5f;
        if(timeRemaining <= 1f)
            flashInterval = 0.25f;

        CheckFlash();

        timeRemaining -= Time.deltaTime;
    }

    public void AddTime(float duration) {
        timeRemaining = duration;
    }

    void CheckFlash() {
        if(Time.time >= lastFlashTime + flashInterval) {
            lastFlashTime = Time.time;
            StartCoroutine(Flash());
        }
    }

    IEnumerator Flash() {
        iconImage.SetActive(false);
        yield return new WaitForSeconds(flashLength);
        iconImage.SetActive(true);
    }
}
