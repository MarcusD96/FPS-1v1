using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour {

    public float lifeTime;

    GameObject hitObject;

    public void SetHitObject(GameObject g) {
        hitObject = g;
        transform.SetParent(g.transform);
    }

    private void LateUpdate() {
        if(hitObject == null)
            Destroy(gameObject);
    }
}