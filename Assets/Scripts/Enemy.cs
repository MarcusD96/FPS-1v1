
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("Stats")]
    public float hp;
    public float damage;
    public float attackRate, attackRange;
    public Collider head;
    public Collider[] body;
    public Animator animator;

    [HideInInspector]
    public bool isDead = false, isAttacking = false;
    [HideInInspector]
    public float nextAttack = 0f;

    private void Update() {
        nextAttack -= Time.deltaTime;
    }

    public void Damage(float damage_) {
        hp -= damage_;
        if(hp <= 0) {
            isDead = true;
            head.enabled = false;
            foreach(var b in body) {
                b.enabled = false;
            }
            animator.SetTrigger("Die");
            Destroy(gameObject, 2f);
        }
    }
}
