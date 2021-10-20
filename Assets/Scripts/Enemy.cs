
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("Stats")]
    public float hp;
    public int pointsWorth;
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
        PlayerManager.Instance.player.points += 10;
        if(hp <= 0) {
            PlayerManager.Instance.player.points += pointsWorth;
            isDead = true;
            head.enabled = false;
            foreach(var b in body) {
                b.enabled = false;
            }
            animator.SetTrigger("Die");
            if(FindObjectOfType<WaveSpawner>() != null)
                FindObjectOfType<WaveSpawner>().RemoveEnemy(gameObject);
            Destroy(gameObject, 2f);
        }
    }
}
