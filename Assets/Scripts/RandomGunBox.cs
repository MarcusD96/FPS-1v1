
using System.Collections.Generic;
using UnityEngine;

public class RandomGunBox : MonoBehaviour {

    public int cost;
    public float interactDistance;
    public List<GameObject> levelGuns;

    List<GameObject> availableGuns;

    void Start() {
        availableGuns = new List<GameObject>(levelGuns);
    }

    void LateUpdate() {
        Player p = PlayerManager.Instance.player;

        if(!p)
            return;

        if(Vector3.Distance(p.transform.position, transform.position) <= interactDistance && p.points >= cost) {
            if(Input.GetKeyDown(KeyCode.F)) {
                p.points -= cost;
                int r = Random.Range(0, availableGuns.Count);
                PlayerShoot ps = p.GetComponent<PlayerShoot>();
                var g = Instantiate(availableGuns[r], p.handPos);
                ps.GiveWeapon(g);
            }
        }
    }

    public void RemoveFromBox(Gun gun) {
        availableGuns.Remove(gun.gameObject);
    }

    public void RestoreToBox(Gun gun) {
        if(levelGuns.Contains(gun.gameObject))
            availableGuns.Add(gun.gameObject);
    }
}

public enum GunNameID {
    M9,
    GLOCK,
    M4,
    R870,
    AK47,
    KSR
}
