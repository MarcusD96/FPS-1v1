
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

    public float lookRadius;
    public LayerMask playerLayer;

    public float chanceToRun = 0;

    Transform target;
    NavMeshAgent agent;
    Enemy enemy;

    IEnumerator attack = null;

    private void Start() {
        enemy = GetComponent<Enemy>();
        agent = GetComponent<NavMeshAgent>();
        target = PlayerManager.Instance.player.transform;

        if(Random.Range(0, Mathf.RoundToInt(1f/chanceToRun)) == 0) {
            agent.speed = agent.speed * 2;
            enemy.animator.SetBool("IsRunner", true);
        }
        else
            enemy.animator.SetBool("IsRunner", false);
    }

    private void Update() {
        if(enemy.isDead) {
            agent.velocity = Vector3.zero;
            return;
        }

        enemy.animator.SetFloat("Speed", agent.velocity.magnitude);

        float distance = Vector3.Distance(target.position, transform.position);
        if(distance <= lookRadius) {
            agent.SetDestination(target.position);
            if(attack != null)
                return;
            if(distance <= enemy.attackRange) {
                attack = AttackTarget();
                StartCoroutine(attack);
                FaceTarget();
            }
        }

    }
    void FaceTarget() {
        Vector3 dir = (target.position - transform.position).normalized;
        Quaternion look = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
    }

    public void Attack() {
        if(enemy.nextAttack <= 0f) {
            if(Physics.CheckSphere(transform.position + transform.up + transform.forward, 2, playerLayer))
                PlayerManager.Instance.player.Damage(enemy.damage);
            enemy.nextAttack = 1 / enemy.attackRate;
            return;
        }
    }

    IEnumerator AttackTarget() {
        enemy.animator.SetTrigger("Attack");
        float spd = agent.speed;
        agent.speed = 0;

        yield return new WaitForSeconds(0.53f);
        if(enemy.isDead)
            yield break;
        Attack();

        yield return new WaitForSeconds((1f / enemy.attackRate) - 0.53f);


        agent.speed = spd;
        attack = null;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemy.attackRange);
    }

}
