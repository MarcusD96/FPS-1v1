using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    [Tooltip("Which room does this spawn have access to?")]
    public Room roomAccess;
    public GameObject enemy;

    public Vector3 GetPos() {
        return transform.position;
    }
}
