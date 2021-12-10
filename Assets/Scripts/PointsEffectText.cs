
using System.Collections;
using TMPro;
using UnityEngine;

public class PointsEffectText : MonoBehaviour {

    public float riseSpeed;
    public float riseDirVariance;
    public float riseTime;
    public TextMeshProUGUI text;

    Vector3 riseDirection;

    public void InitializeText(int points_, Vector2 pos_) {
        text.text = "+" + points_.ToString();
        transform.position = pos_;
        riseDirection = new Vector3(Random.Range(-riseDirVariance, riseDirVariance), 1f, 0f).normalized;
        gameObject.SetActive(true);
        StartCoroutine(Rise());
    }

    IEnumerator Rise() {
        float endTime = Time.time + riseTime;
        while(Time.time < endTime) {
            transform.position = Vector3.Lerp(transform.position, transform.position + riseDirection, Time.deltaTime * riseSpeed);
            yield return null;
        }
        gameObject.SetActive(false);
    } 

}
