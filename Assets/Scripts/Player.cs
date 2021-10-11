
using System;
using UnityEngine;

public class Player : MonoBehaviour {

    public float hp;
    public Animator animator;

    public void Damage(float damage_) {
        hp -= damage_;
        if(hp <= 0)
            Destroy(gameObject);
    }

    private void Update() {
        if(!animator)
            return;

        if(Input.GetKey(KeyCode.LeftControl))
            animator.SetBool("Crouch", true);
        else
            animator.SetBool("Crouch", false);
    }
}