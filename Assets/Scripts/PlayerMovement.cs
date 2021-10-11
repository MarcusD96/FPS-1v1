using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {

    public float moveSpeed;
    public float groundDistance;
    public float jumpHeight = 3f;
    public bool isRunning;
    public Transform groundCheck;
    public LayerMask groundMask;

    float gravity = Physics.gravity.y;
    bool isGrounded;
    Vector3 velocity;
    PlayerShoot playerShootComp;
    PlayerZoom zoomComp;
    Melee meleeComp;
    CharacterController cc;

    private void Awake() {
        cc = GetComponent<CharacterController>();
        playerShootComp = GetComponent<PlayerShoot>();
        zoomComp = GetComponent<PlayerZoom>();
        meleeComp = GetComponent<Melee>();
    }

    private void Update() {
        if(Settings.Paused)
            return;

        CheckGrounded();

        if(isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        Move();
        Jump();
        Fall();
    }

    bool CheckGrounded() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded)
            cc.stepOffset = 0.75f;
        else
            cc.stepOffset = 0.0f;

        return isGrounded;
    }

    float lastSpeed = 0;
    void Move() {
        float speed = lastSpeed;

        if(isGrounded) {
            if(Input.GetKey(KeyCode.LeftShift) && !playerShootComp.currentGun.isReloading) {
                speed = lastSpeed = moveSpeed * 2f * Time.deltaTime;
                if(!isRunning) {
                    isRunning = true;
                    Settings.FOV += 20f;
                }
            }
            else {
                speed = lastSpeed = moveSpeed * Time.deltaTime;
                if(zoomComp.isZoomingIn)
                    speed /= 2f;
                if(isRunning) {
                    isRunning = false;
                    Settings.FOV -= 20f;
                }
            }
        }
        else {
            if(Input.GetKeyUp(KeyCode.LeftShift) && isRunning) {
                Settings.FOV -= 20f;
                isRunning = false;
            }
        }

        if(meleeComp.isMeleeing && isRunning) {
            speed = lastSpeed = moveSpeed * Time.deltaTime;
            isRunning = false;
            Settings.FOV -= 20f;
        }

        playerShootComp.currentGun.animator.SetBool("IsRunning", isRunning);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        if(move.magnitude > 1)
            move.Normalize();
        cc.Move(move * speed);
        playerShootComp.currentGun.animator.SetFloat("Speed", move.magnitude);
    }

    float fallMultiplier = 2.5f;
    void Fall() {
        float dt = Time.deltaTime;
        velocity.y += gravity * dt;
        cc.Move(velocity * dt);
        if(velocity.y < 0) {
            velocity += (fallMultiplier - 1) * gravity * dt * Vector3.up;
        }
    }

    void Jump() {
        if(Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            cc.stepOffset = 0;
        }
    }

    bool CanRun() {
        //Conditions:
        //not meleeing

        //Cancels:
        //reload
        //shooting

        return true;
    }
}
