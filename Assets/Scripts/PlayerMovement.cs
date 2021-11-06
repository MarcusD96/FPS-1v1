using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {

    public float moveSpeed;
    public float groundDistance;
    public float jumpHeight = 3f;
    public bool isRunning;

    public LayerMask groundMask;

    public Transform groundCheck;


    float gravity = Physics.gravity.y;
    bool isGrounded;

    Vector3 velocity;

    PlayerShoot playerShootComp;
    PlayerZoom zoomComp;
    Player player;
    Melee meleeComp;
    CharacterController cc;

    private void Awake() {
        cc = GetComponent<CharacterController>();
        playerShootComp = GetComponent<PlayerShoot>();
        zoomComp = GetComponent<PlayerZoom>();
        meleeComp = GetComponent<Melee>();
        player = GetComponent<Player>();
        lastPos = transform.position;
        lastTime = Time.time;
        //InvokeRepeating(nameof(GetLastPos), 0, 0.5f);
    }

    Vector3 lastPos;
    float lastTime;
    void GetLastPos() {
        print("Current Speed: " + ((transform.position - lastPos).magnitude / (Time.time - lastTime)) );
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
            if(Input.GetKey(KeyCode.LeftShift) && !playerShootComp.currentGun.isReloading && !player.isCrouching && speed > 0.01f) {
                speed = lastSpeed = moveSpeed * 1.65f * Time.deltaTime;
                isRunning = true;
                Settings.FOV_Current = Mathf.Lerp(Settings.FOV_Current, Settings.FOV_Base + 20, 10f * Time.deltaTime);
            }
            else {
                speed = lastSpeed = moveSpeed * Time.deltaTime;
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
            speed = lastSpeed = moveSpeed * Time.deltaTime;
            isRunning = false;
            Settings.FOV_Current = Settings.FOV_Base;
        }

       playerShootComp.currentGun.animator.SetBool("IsRunning", isRunning);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if(z <= 0) {
            isRunning = false;
            playerShootComp.currentGun.animator.SetBool("IsRunning", isRunning);
            speed = moveSpeed * 0.9f * Time.deltaTime;
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
}
