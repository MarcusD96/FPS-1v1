using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    public float hp;
    public Collider head, body;

    public void Damage(float damage_) {
        hp -= damage_;
        if(hp <= 0)
            Destroy(gameObject);
    }
}