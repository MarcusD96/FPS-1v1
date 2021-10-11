using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour {

    public TextMeshProUGUI text;
    public Color critColor;
    public float floatSpeed;

    float decreaseSpeedMultiplier;

    public void Initialize(float damage_, bool headshot_) {
        text.text = damage_.ToString("F0");
        floatSpeed = Random.Range(floatSpeed * 0.5f, floatSpeed * 2f);
        if(headshot_) {
            text.fontSize *= 2f;
            text.color = critColor;
            floatSpeed *= 4f;
        }

        decreaseSpeedMultiplier = floatSpeed * 2f;
        var r = transform.localEulerAngles;
        r.z = Random.Range(-90f, 0f);
        transform.localEulerAngles = r;
    }

    private void Update() {
        transform.position += floatSpeed * Time.deltaTime * transform.up;
        floatSpeed -= Time.deltaTime * decreaseSpeedMultiplier;
        if(floatSpeed <= 0)
            Destroy(gameObject);
    }
}
