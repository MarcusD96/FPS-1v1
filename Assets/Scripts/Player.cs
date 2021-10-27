
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float hp;
    public int points;

    public bool isCrouching;
    public float healSpeed, healDelayTime;

    public Transform handPos;
    public Animator animator;
    public Image damageVignette;

    [Header("UI Elements")]
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI waveNum;
    public GameObject buyWeapon;
    public TextMeshProUGUI buyWeaponText;
    public GameObject buyDoor;
    public TextMeshProUGUI buyDoorText;

    float startHp, timeToHealStart = 0;

    private void Start() {
        startHp = hp;
    }

    public void Damage(float damage_) {
        timeToHealStart = healDelayTime;
        EZCameraShake.CameraShaker.Instance.ShakeOnce(30f, 10f, .1f, .2f);
        hp -= damage_;
        if(hp <= 0)
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void Heal() {
        if(timeToHealStart > 0f)
            return;

        if(hp >= startHp)
            return;

        hp += Time.deltaTime * healSpeed;

        if(hp > startHp)
            hp = startHp;
    }


    private void Update() {
        timeToHealStart -= Time.deltaTime;

        Color dmgColor = damageVignette.color;
        var hpPercent = hp / startHp;
        dmgColor.a = Mathf.Lerp(0.4f, 0f, hpPercent);
        damageVignette.color = dmgColor;

        pointsText.text = "Points: " + points.ToString();

        Heal();
        Crouch();
    }

    void Crouch() {
        if(!animator)
            return;
        if(Input.GetKey(KeyCode.LeftControl)) {
            if(!isCrouching) {
                isCrouching = true;
                GetComponent<PlayerMovement>().moveSpeed /= 2f;
            }
        }
        else {
            if(isCrouching) {
                isCrouching = false;
                GetComponent<PlayerMovement>().moveSpeed *= 2f;
            }
        }
        animator.SetBool("Crouch", isCrouching);
    }
}