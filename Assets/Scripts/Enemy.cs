
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

    AudioSource growlSound;

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
        Growl();
    }

    private void Update() {
        nextAttack -= Time.deltaTime;
        if(isDead && growlSound != null)
            growlSound.Stop();
    }

    public void DamageBody(float damage_, bool headShot) {
        hp -= damage_;
        if(hp <= 0) {
            if(headShot) {
                PlayerManager.Instance.HeadKillEnemy();
                animator.SetTrigger("Headshot");
                if(Random.Range(0, 10) == 0) 
                    AudioManager.instance.PlaySound("Head Shot", AudioManager.instance.effects);
            }
            else {
                PlayerManager.Instance.BodyKillEnemy();
                AudioManager.instance.PlayAtLocation("Zombie Death", AudioManager.instance.effects, transform);
                animator.SetTrigger("Die");
            }

            isDead = true;
            head.enabled = false;
            foreach(var b in body) {
                b.enabled = false;
            }
            if(FindObjectOfType<WaveSpawner>() != null)
                FindObjectOfType<WaveSpawner>().RemoveEnemy(gameObject);
             
            PowerUpManager.Instance.TryToDropPowerUp(transform);
            Destroy(gameObject, 2f);
            return;
        }
        PlayerManager.Instance.HitEnemy();
    }

    public void DamageMelee(float damage_) {
        hp -= damage_;
        if(hp <= 0) {
            PlayerManager.Instance.MeleeKillEnemy();
            AudioManager.instance.PlayAtLocation("Zombie Death", AudioManager.instance.effects, transform);
            animator.SetTrigger("Melee");
            isDead = true;
            head.enabled = false;
            foreach(var b in body) {
                b.enabled = false;
            }
            if(FindObjectOfType<WaveSpawner>() != null)
                FindObjectOfType<WaveSpawner>().RemoveEnemy(gameObject);

            PowerUpManager.Instance.TryToDropPowerUp(transform);
            Destroy(gameObject, 2f);
            return;
        }
        PlayerManager.Instance.player.points += 10;
    }

    void Growl() {
        if(Vector3.Distance(PlayerManager.Instance.player.transform.position, transform.position) < 10f) {
            growlSound = AudioManager.instance.PlayAtLocation("Zombie Growl", AudioManager.instance.effects, transform);
        }
        Invoke(nameof(Growl), Random.Range(2f, 7f));
    }
}
