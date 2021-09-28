
using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    public float hp;
    public Collider head, body;
    public Animator weaponAnimator;

    [Header("Player States")]
    public bool isRunning = false;
    public bool isShooting = false;
    public bool isMeleeing = false;
    public bool isReloading = false;
    public bool isAiming = false;

    public void Damage(float damage_) {
        hp -= damage_;
        if(hp <= 0)
            Destroy(gameObject);
    }

    private void LateUpdate() {
        UpdateAnims();
    }

    private void UpdateAnims() {
        
    }
}