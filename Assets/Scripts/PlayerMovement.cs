using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {

    public float moveSpeed;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    public float jumpHeight = 3f;

    float gravity = Physics.gravity.y;
    bool isGrounded;
    Vector3 velocity;
    CharacterController cc;

    private void Awake() {
        cc = GetComponent<CharacterController>();
    }

    private void Update() {
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
            if(Input.GetKey(KeyCode.LeftShift))
                speed = lastSpeed = moveSpeed * 1.5f * Time.deltaTime;
            else
                speed = lastSpeed = moveSpeed * Time.deltaTime;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        cc.Move(move * speed);
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
