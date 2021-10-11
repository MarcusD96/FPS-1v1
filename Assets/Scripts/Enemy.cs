using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    [Header("Stats")]
    public float hp;
    public float damage;
    public float attackRate, attackRange;
    public Collider head, body;

    float nextAttack = 0f;

    [Header("AI")]
    public float lookRadius;
    Transform target;
    NavMeshAgent agent;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        target = PlayerManager.Instance.player.transform;
    }

    private void Update() {
        nextAttack -= Time.deltaTime;
        float distance = Vector3.Distance(target.position, transform.position);
        if(distance <= lookRadius) {
            agent.SetDestination(target.position);
            if(distance <= attackRange) {
                Attack();
                FaceTarget();
            }
        }
    }

    void FaceTarget() {
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion look = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
    }


    void Attack() {
        if(nextAttack <= 0f) {
            PlayerManager.Instance.player.Damage(damage);
            nextAttack = 1 / attackRate;
        }
    }

    public void Damage(float damage_) {
        hp -= damage_;
        if(hp <= 0)
            Destroy(gameObject);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
