
using System.Collections.Generic;
using UnityEngine;

public class RandomGunBox : MonoBehaviour {

    public int cost;
    public float interactDistance;
    public List<Gun> levelGuns;

    Player p;
    PlayerShoot ps;

    void Update() {
        if(!p || !ps) {
            p = PlayerManager.Instance.player;
            ps = p.GetComponent<PlayerShoot>();
            return;
        }

        if(Vector3.Distance(transform.position, p.transform.position) <= interactDistance && p.points >= cost) {
            if(Input.GetKeyDown(KeyCode.F)) {
                p.points -= cost;
                GetRandomGunFromList();
            }
        }
    }

    void GetRandomGunFromList() {
        int r = Random.Range(0, levelGuns.Count);

        //player has weapon, reroll
        bool hasWeapon = false;
        foreach(var g in ps.guns) {
            if(g.gunID == levelGuns[r].gunID) {
                GetRandomGunFromList();
                hasWeapon = true;
                break;
            }
        }

        //player doesnt have weapon, give weapon
        if(!hasWeapon) {
            var g = Instantiate(levelGuns[r], p.handPos);
            ps.GiveWeapon(g.gameObject);

            //check for level wall gun
            foreach(var wg in FindObjectOfType<WallGunManager>().GetWallGuns()) {
                //if the random gun id == the wallgun gun id, mark wallgun as purchased
                var gun = wg.gunModel.GetComponent<Gun>();
                if(gun.gunID == levelGuns[r].gunID) {
                    wg.purchased = true;
                    wg.purchasedGun = g;
                }
            }
        }
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
