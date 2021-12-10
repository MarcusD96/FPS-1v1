
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGunBox : MonoBehaviour {

    public int cost;
    public float interactDistance;
    public float switchTime, gunRewardTime, rewardExpiryTime;
    public Animator animator;
    public Transform gunModelPos;
    public List<BoxGun> boxGuns;

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
                StartCoroutine(NewAnimateReward());
            }
        }
    }

    IEnumerator AnimateReward() {
        boxActivated = true;

        //Choose random gun
        int r = Random.Range(0, boxGuns.Count);
        var randGun = GetRandomGun(r);
        int chosenIndex = actualInt;

        float endTime = Time.time + gunRewardTime;
        //keep switching guns for total time with switch time intervals
        while(Time.time < endTime) {
            var g = Instantiate(boxGuns[r].gun, gunModelPos);
            r++;
            if(r >= boxGuns.Count)
                r = 0;
            yield return new WaitForSeconds(switchTime);
            Destroy(g.gameObject);

        }

        var finalGun = Instantiate(randGun, gunModelPos);

        //offer gun to player until expiry time, replace current gun if max amount of guns are held or offer new gun if not
        float expiryTime = Time.time + rewardExpiryTime;
        while(Time.time < expiryTime) {
            if(Input.GetKeyDown(KeyCode.F)) {
                var playerGun = Instantiate(boxGuns[chosenIndex].gun);
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
        Gun returnGun = null;

        //check if the player already owns that gun, if so try again until not
        bool ownsWeapon = false;
        foreach(var g in ps.guns) {
            if(g.gunID == boxGuns[randInt].gun.gunID) {
                ownsWeapon = true;
                int newR = Random.Range(0, boxGuns.Count);
                actualInt = newR;
                returnGun = GetRandomGun(newR);
                break;
            }
        }
        if(!ownsWeapon) {
            returnGun = boxGuns[randInt].gun;
        }

        //return the unique gun
        return returnGun;
    }

    IEnumerator NewAnimateReward() {
        boxActivated = true;

        BoxGun rewardGun = GetWeightedRandomGun();

        //animate scrolling of guns
        float endTime = Time.time + gunRewardTime;
        int i = 0;
        while(Time.time < endTime) {
            var g = Instantiate(boxGuns[i], gunModelPos);
            i++;
            if(i >= boxGuns.Count)
                i = 0;
            yield return new WaitForSeconds(switchTime);
            Destroy(g.gameObject);
        }

        //present chosen gun
        BoxGun chosenGun = Instantiate(rewardGun, gunModelPos);

        //allow player to take withing timeframe of retrieval
        float expiryTime = Time.time + rewardExpiryTime;
        while(Time.time < expiryTime) {
            if(Input.GetKeyDown(KeyCode.F)) {
                var playerGun = Instantiate(rewardGun.gun);
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
        Destroy(chosenGun.gameObject);

        boxActivated = false;
    }

    int rerollNum = 0;
    BoxGun GetWeightedRandomGun() {
        if(rerollNum > 25) {
            Debug.LogError("Rerolled too many times;");
            return null;
        }
        //get total weight
        int totalWeight = 0;
        for(int i = 0; i < boxGuns.Count; i++) {
            totalWeight += boxGuns[i].weight;
        }

        BoxGun selectedGun = null;

        //get random number between 0 and total weight
        int r = Random.Range(0, totalWeight);

        foreach(var b in boxGuns) {
            if(r < b.weight) {
                bool hasGun = false;
                //check if player has gun
                foreach(var g in ps.guns) {
                    if(g.gunID == b.gun.gunID) {
                        hasGun = true;
                        break;
                    }
                }

                //if player doesnt have gun, give this 
                if(!hasGun) {
                    selectedGun = b;
                    break;
                }
                //else reroll
                else {
                    rerollNum++;
                    selectedGun = GetWeightedRandomGun();
                    break;
                }
            }
            r -= b.weight;
        }

        rerollNum = 0;
        return selectedGun;
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
    KSR,
    AUG
}
