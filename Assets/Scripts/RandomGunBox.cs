
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGunBox : MonoBehaviour {

    public int cost;
    public float interactDistance;
    public float switchTime, gunRewardTime, rewardExpiryTime;
    public Animator animator;
    public Transform gunModelPos;
    public List<Gun> levelGuns;

    bool boxActivated = false;
    Player p;
    PlayerShoot ps;

    private void Start() {
        InvokeRepeating(nameof(ShakeBox), 0, 3f);
    }

    void Update() {
        if(!p) {
            p = PlayerManager.Instance.player;
            if(p) {
                ps = p.GetComponent<PlayerShoot>();
            }
            return;
        }

        if(Vector3.Distance(transform.position, p.transform.position) <= interactDistance && p.points >= cost && !boxActivated) {
            if(Input.GetKeyDown(KeyCode.F)) {
                p.points -= cost;
                StartCoroutine(AnimateReward());
            }
        }
    }

    IEnumerator AnimateReward() {
        boxActivated = true;

        //Choose random gun
        int r = Random.Range(0, levelGuns.Count);
        var randGun = GetRandomGun(r);
        int chosenIndex = actualInt;

        float endTime = Time.time + rewardExpiryTime;
        //keep switching guns for total time with switch time intervals
        while(Time.time < endTime) {
            var g = Instantiate(levelGuns[r], gunModelPos);
            foreach(Transform c in g.GetComponentsInChildren<Transform>()) {
                c.gameObject.layer = 0;
            }
            Destroy(g.animator);
            r++;
            if(r >= levelGuns.Count)
                r = 0;
            yield return new WaitForSeconds(switchTime);
            Destroy(g.gameObject);

        }

        var finalGun = Instantiate(randGun, gunModelPos);
        Destroy(finalGun.animator);
        foreach(Transform c in finalGun.GetComponentsInChildren<Transform>()) {
            c.gameObject.layer = 0;
        }

        //offer gun to player until expiry time, replace current gun if max amount of guns are held or offer new gun if not
        float expiryTime = Time.time + rewardExpiryTime;
        while(Time.time < expiryTime) {
            if(Input.GetKeyDown(KeyCode.F)) {
                var playerGun = Instantiate(levelGuns[chosenIndex]);
                ps.GiveWeapon(playerGun.gameObject);

                //check for level wallgun
                foreach(var wg in FindObjectOfType<WallGunManager>().GetWallGuns()) {
                    //if the random gun id == the wallgun gun id, mark wallgun as purchased
                    var gun = wg.gunModel.GetComponent<Gun>();
                    if(gun.gunID == playerGun.gunID) {
                        wg.purchased = true;
                        wg.purchasedGun = playerGun;
                    }
                }
                break;
            }
            yield return null;
        }
        Destroy(finalGun.gameObject);

        boxActivated = false;
    }

    int actualInt;
    Gun GetRandomGun(int randInt) {
        actualInt = randInt;
        Gun randomGun = null;

        //check if the player already owns that gun, if so try again until not
        bool ownsWeapon = false;
        foreach(var g in ps.guns) {
            if(g.gunID == levelGuns[randInt].gunID) {
                ownsWeapon = true;
                int newR = Random.Range(0, levelGuns.Count);
                actualInt = newR;
                randomGun = GetRandomGun(newR);
                break;
            }
        }
        if(!ownsWeapon) {
            randomGun = levelGuns[randInt];
        }

        //return the unique gun
        return randomGun;
    }

    void ShakeBox() {
        int r = Random.Range(0, 3);
        animator.SetInteger("Idle Num", r);
        animator.SetTrigger("Shake");
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
