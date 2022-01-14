
using UnityEngine;

public class PowerUp : MonoBehaviour {
    [Header("Base Settings")]
    public Transform model;
    public Transform particles;
    public float despawnTime = 20f;
    public float rotateSpeed = 20f;
    public float collectionDistance = 1f;
    [Range(1, 10)]
    public int weightChance = 10;

    [Header("PowerUp Settings")]
    public string powerUpSoundName;
    public float powerUpTime;

    protected Player player;

    float despawnEndTime;
    bool collected = false;
    AudioSource sound;

    private void Start() {
        despawnEndTime = Time.time + despawnTime;
        transform.position += Vector3.up;
        sound  = AudioManager.instance.PlayAtLocation("Shine", AudioManager.instance.powerUps, transform);
    }

    protected void Update() {
        if(!collected) {
            RotatePowerUp();
            CheckCollection(); 
        }
        if(Time.time >= despawnEndTime && !collected) { 
            sound.Stop();
            Destroy(gameObject);
        }
    }

    void RotatePowerUp() {
        var euler = model.rotation.eulerAngles;
        euler.y += rotateSpeed * Time.deltaTime;
        model.eulerAngles = euler;
    }

    void CheckCollection() {
        if(!player) {
            player = PlayerManager.Instance.player;
            return;
        }
        if(Vector3.Distance(transform.position, player.transform.position) <= collectionDistance) {
            CollectPowerUp();
            collected = true;
        }
    }

    protected virtual void CollectPowerUp() {
        foreach(Transform c in transform) {
            Destroy(c.gameObject);
        }
        AudioManager.instance.PlaySound(powerUpSoundName, AudioManager.instance.powerUps);
        sound.Stop();
        Destroy(gameObject, powerUpTime + 1f);
    }
}