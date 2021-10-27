
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

    public void Initialize(int waveNum, bool lastEnemy_) {
        if(waveNum < 10) {
            hp += 100 * (waveNum - 1);
        }
        else {
            hp = 950 * Mathf.Pow(1.1f, waveNum - 9);
        }
        EnemyAI ai;
        if(TryGetComponent(out ai)) {
            if(waveNum > 3) {
                if(lastEnemy_)
                    ai.chanceToRun = 1f;
                else {
                    ai.chanceToRun += 0.1f * (waveNum - 3);
                    ai.chanceToRun = Mathf.Clamp(ai.chanceToRun, 0f, 1f);
                }
            }
        }
    }

    private void Update() {
        nextAttack -= Time.deltaTime;
    }

    public void DamageBody(float damage_, bool headShot) {
        hp -= damage_;
        if(hp <= 0) {
            if(headShot)
                PlayerManager.Instance.player.points += pointsWorth * 2;
            else
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
            return;
        }
        PlayerManager.Instance.player.points += 10;
    }

    public void DamageMelee(float damage_) {
        hp -= damage_;
        if(hp <= 0) {
            PlayerManager.Instance.player.points += (pointsWorth * 2) + 30;
            isDead = true;
            head.enabled = false;
            foreach(var b in body) {
                b.enabled = false;
            }
            animator.SetTrigger("Die");
            if(FindObjectOfType<WaveSpawner>() != null)
                FindObjectOfType<WaveSpawner>().RemoveEnemy(gameObject);
            Destroy(gameObject, 2f);
            return;
        }
        PlayerManager.Instance.player.points += 10;
    }
}
