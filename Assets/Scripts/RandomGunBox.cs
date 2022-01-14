
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGunBox : Interactable {

    public int cost;
    public float interactDistance;
    public float switchTime, gunRewardTime, rewardExpiryTime;
    public Animator animator;
    public Transform gunModelPos;
    public List<BoxGun> boxGuns;

    bool boxActivated = false;
    string chosenGunName;
    Player p;
    PlayerShoot ps;
    GunBoxState boxState;

    private void Start() {
        InvokeRepeating(nameof(ShakeBox), 0, 3f);
        boxState = GunBoxState.AVAILABLE;
    }

    void Update() {
        if(!p) {
            p = PlayerManager.Instance.player;
            if(p) {
                ps = p.GetComponent<PlayerShoot>();
            }
            return;
        }
    }

    void RollBox() {
        if(p.points < cost)
            return;

        p.points -= cost;
        StartCoroutine(AnimateReward());
    }

    void RetrieveRolledGun() {

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

    IEnumerator AnimateReward() {
        boxActivated = true;
        boxState = GunBoxState.ROLLING;

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
        chosenGunName = chosenGun.gun.gunID.ToString();
        boxState = GunBoxState.RETRIEVABLE;

        //allow player to take withing timeframe of retrieval
        float expiryTime = Time.time + rewardExpiryTime;
        while(Time.time < expiryTime) {
            if(Input.GetKeyDown(PlayerManager.Instance.interact)) {
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


        boxState = GunBoxState.AVAILABLE;
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

    public override void UpdateInteractText() {
        switch(boxState) {
            case GunBoxState.AVAILABLE:
                PlayerManager.Instance.player.ShowInteractionText("Get Random Gun for " + cost + " " + PlayerManager.Instance.interactKey);
                break;
            case GunBoxState.ROLLING:
                PlayerManager.Instance.player.HideInteractionText();
                ;
                break;
            case GunBoxState.RETRIEVABLE:
                PlayerManager.Instance.player.ShowInteractionText("Retrieve " + chosenGunName + " " + PlayerManager.Instance.interactKey);
                break;
            default:
                break;
        }
    }

    public override void OnInteract() {
        switch(boxState) {
            case GunBoxState.AVAILABLE:
                RollBox();
                break;
            case GunBoxState.RETRIEVABLE:
                RetrieveRolledGun();
                break;
            default:
                break;
        }
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

public enum GunBoxState {
    AVAILABLE,
    ROLLING,
    RETRIEVABLE
}