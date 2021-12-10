using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {

    public float walkSpeed, runSpeed, crouchSpeed;
    public float groundDistance;
    public float jumpHeight = 3f;
    public bool isRunning;

    public LayerMask groundMask;
    public Transform groundCheck, body, hand;

    float gravity = Physics.gravity.y;
    bool isGrounded, isCrouching;

    Vector3 velocity;

    PlayerShoot playerShootComp;
    PlayerZoom zoomComp;
    Player player;
    Melee meleeComp;
    Animator animator;
    CharacterController cc;

    private void Awake() {
        cc = GetComponent<CharacterController>();
        playerShootComp = GetComponent<PlayerShoot>();
        zoomComp = GetComponent<PlayerZoom>();
        meleeComp = GetComponent<Melee>();
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        lastPos = transform.position;
        lastTime = Time.time;
    }

    Vector3 lastPos = Vector3.zero;
    float lastTime = 0f, actualSpeed = 0f;
    void CalculateActualSpeed() {
        actualSpeed = (transform.position - lastPos).magnitude / (Time.time - lastTime);
        lastTime = Time.time;
        lastPos = transform.position;
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
        Crouch();
        CalculateActualSpeed();
    }

    bool CheckGrounded() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded)
            cc.stepOffset = 0.75f;
        else
            cc.stepOffset = 0.0f;

        return isGrounded;
    }

    float airBorneSpeed = 0;
    void Move() {
        float speed = airBorneSpeed;

        if(isGrounded) {
            if(Input.GetKey(KeyCode.LeftShift) && !playerShootComp.currentGun.isReloading && !isCrouching && actualSpeed > 0.01f) {
                speed = airBorneSpeed = runSpeed * Time.deltaTime;
                isRunning = true;
                Settings.FOV_Current = Mathf.Lerp(Settings.FOV_Current, Settings.FOV_Base + 20, 10f * Time.deltaTime);
            }
            else {
                speed = airBorneSpeed = walkSpeed * Time.deltaTime;
                if(zoomComp.isZoomingIn)
                    speed /= 2f;
                isRunning = false;
                Settings.FOV_Current = Mathf.Lerp(Settings.FOV_Current, Settings.FOV_Base, 10f * Time.deltaTime);
            }
        }
        else {
            if(Input.GetKeyUp(KeyCode.LeftShift)) {
                Settings.FOV_Current = Mathf.Lerp(Settings.FOV_Current, Settings.FOV_Base, 10f * Time.deltaTime);
                isRunning = false;
            }
        }

        if(meleeComp.isMeleeing && isRunning) {
            speed = airBorneSpeed = walkSpeed * Time.deltaTime;
            isRunning = false;
            Settings.FOV_Current = Settings.FOV_Base;
        }

        playerShootComp.currentGun.animator.SetBool("IsRunning", isRunning);

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if(z <= 0) {
            isRunning = false;
            playerShootComp.currentGun.animator.SetBool("IsRunning", isRunning);
            speed = walkSpeed * 0.9f * Time.deltaTime;
        }

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
        if(velocity.y < 0)
            velocity += (fallMultiplier - 1) * gravity * dt * Vector3.up;
    }

    void Jump() {
        if(Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            cc.stepOffset = 0;
        }
    }

    void Crouch() {
        if(!animator)
            return;
        if(Input.GetKey(KeyCode.LeftControl)) {
            if(!isCrouching) {
                isCrouching = true;
                walkSpeed /= 2f;
            }
            body.localPosition = Vector3.Lerp(body.localPosition, Vector3.down / 2f, crouchSpeed * Time.deltaTime);
        }
        else {
            if(isCrouching) {
                isCrouching = false;
                walkSpeed *= 2f;
            }
            body.localPosition = Vector3.Lerp(body.localPosition, Vector3.zero, crouchSpeed * Time.deltaTime);
        }
    }
}