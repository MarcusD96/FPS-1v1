
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour {

    public float interactDistance;
    public List<Interactable> interactables;

    Interactable closestInteractable;

    private void Start() {
        foreach(var i in FindObjectsOfType<Interactable>()) {
            interactables.Add(i);
        }
        InvokeRepeating(nameof(FindClosestInteractable), 0.1f, 0.1f);
    }

    private void Update() {
        if(CheckInteractVisible()) {
            Interact();
        }
    }

    void FindClosestInteractable() {
        float dist = float.MaxValue;
        Interactable closest = null;
        foreach(var itbl in interactables) {
            float d = Vector3.Distance(itbl.transform.position, PlayerManager.Instance.player.transform.position);
            if(d <= dist) {
                dist = d;
                closest = itbl;
            }
        }
        if(closest)
            closestInteractable = closest;
        else {
            closestInteractable = null;
        }
    }

    bool CheckInteractVisible() {
        if(closestInteractable) {
            if(Vector3.Distance(PlayerManager.Instance.player.transform.position, closestInteractable.transform.position) <= interactDistance) {
                closestInteractable.UpdateInteractText();
                return true;
            }
            else {
                PlayerManager.Instance.player.HideInteractionText();
                return false;
            }
        }
        return false;
    }
    void Interact() {
        if(Input.GetKeyDown(PlayerManager.Instance.interact))
            closestInteractable.OnInteract();
    }
}
