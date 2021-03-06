
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float hp;
    public int points;

    public float healSpeed, healDelayTime;

    public Transform hand;
    public Image damageVignette;

    [Header("UI Elements")]
    public GameObject textContainer;
    public TextMeshProUGUI interactText;

    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI waveNum;

    public PowerUpIcon doublePointsIcon, instaKillIcon;

    float startHp, timeToHealStart = 0f;
    float damageCooldown = 0.5f, nextDamage = 0f;

    private void Start() {
        startHp = hp;
    }

    private void Update() {
        timeToHealStart -= Time.deltaTime;
        nextDamage -= Time.deltaTime;

        Color dmgColor = damageVignette.color;
        var hpPercent = hp / startHp;
        dmgColor.a = Mathf.Lerp(0.4f, 0f, hpPercent);
        damageVignette.color = dmgColor;

        pointsText.text = points.ToString();

        Heal();
    }

    public void Damage(float damage_) {
        if(nextDamage > 0f)
            return;

        nextDamage = damageCooldown;

        timeToHealStart = healDelayTime;
        EZCameraShake.CameraShaker.Instance.ShakeOnce(30f, 10f, .1f, .2f);
        hp -= damage_;
        if(hp <= 0)
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
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

    public void ShowInteractionText(string text_) {
        textContainer.SetActive(true);
        interactText.text = text_;
    }

    public void HideInteractionText() {
        textContainer.SetActive(false);
    }
}