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
        text.text = damage_.ToString();
        if(headshot_) {
            text.fontSize *= 2;
            text.color = critColor;
        }
        decreaseSpeedMultiplier = floatSpeed * 2;
        var r = transform.localEulerAngles;
        r.z = Random.Range(-45, 0);
        transform.localEulerAngles = r;
    }

    private void Update() {
        transform.position += floatSpeed * Time.deltaTime * transform.up;
        floatSpeed -= Time.deltaTime * decreaseSpeedMultiplier;
        if(floatSpeed <= 0)
            Destroy(gameObject);
    }
}
