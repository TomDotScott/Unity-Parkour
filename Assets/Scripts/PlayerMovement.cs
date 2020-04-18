using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Current Values")]
    public float walkSpeed;
    public float runSpeed;
    public float slideSpeed = 10.0f;
    public float crouchSpeed = 2f;
    [SerializeField] private float jumpSpeed = 8.0f;
    
    [Header("Standard Values")]
    public float standardWalkSpeed;
    public float standardRunSpeed;
    public float standardSlideSpeed = 10.0f;
    public float standardCrouchSpeed = 2f;
    [SerializeField] private float standardJumpSpeed = 8.0f;

    [Header("Giant Values")]
    public float giantWalkSpeed;
    public float giantRunSpeed;
    public float giantSlideSpeed = 10.0f;
    public float giantCrouchSpeed = 2f;
    [SerializeField] private float giantJumpSpeed = 8.0f;

    [Header("Tiny Values")]
    public float tinyWalkSpeed;
    public float tinyRunSpeed;
    public float tinySlideSpeed = 10.0f;
    public float tinyCrouchSpeed = 2f;
    [SerializeField] private float tinyJumpSpeed = 8.0f;

    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    [HideInInspector] public Vector3 contactPoint;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public bool playerControl = false;

    [Header("Other Values")]
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float antiBumpFactor = .75f;
    public bool grounded = false;
    public Vector3 jump = Vector3.zero;

    private RaycastHit hit;
    private Vector3 force;
    private bool forceGravity;
    private float forceTime = 0;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (forceTime > 0)
        {
            forceTime -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (forceTime > 0)
        {
            if (forceGravity)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            grounded = (characterController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        }
    }

    public void Move(Vector2 input, bool sprint, bool crouching)
    {
        if (forceTime <= 0)
        {
            float speed = sprint ? runSpeed : walkSpeed;
            if (crouching)
            {
                speed = crouchSpeed;
            }

            if (grounded)
            {
                moveDirection = new Vector3(input.x, -antiBumpFactor, input.y);
                moveDirection = transform.TransformDirection(moveDirection) * speed;
                UpdateJump();
            }

            // Apply gravity
            moveDirection.y -= gravity * Time.deltaTime;
            // Move the controller, and set grounded true or false depending on whether we're standing on something
            grounded = (characterController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        }
    }

    public void Move(Vector3 direction, float speed, float appliedGravity)
    {
        if (forceTime <= 0)
        {
            Vector3 move = direction * speed;
            if (appliedGravity > 0)
            {
                moveDirection.x = move.x;
                moveDirection.y -= gravity * Time.deltaTime * appliedGravity;
                moveDirection.z = move.z;
            }
            else
            {
                moveDirection = move;
            }

            UpdateJump();

            grounded = (characterController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
        }
    }

    public void Jump(Vector3 dir, float mult) => jump = dir * mult;

    public void UpdateJump()
    {
        if (jump != Vector3.zero)
        {
            Vector3 dir = (jump * jumpSpeed);
            if (dir.x != 0)
            {
                moveDirection.x = dir.x;
            }
            if (dir.y != 0)
            {
                moveDirection.y = dir.y;
            }
            if (dir.z != 0)
            {
                moveDirection.z = dir.z;
            }
        }
        jump = Vector3.zero;
    }

    public void ForceMove(Vector3 direction, float speed, float time, bool applyGravity)
    {
        forceTime = time;
        forceGravity = applyGravity;
        moveDirection = direction * speed;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPoint = hit.point;
    }


    public void SetStandard()
    {
        walkSpeed = standardWalkSpeed;
        runSpeed = standardRunSpeed;
        slideSpeed = standardSlideSpeed;
        crouchSpeed = standardCrouchSpeed;
        jumpSpeed = standardJumpSpeed;
    }

    public void SetGiant()
    {
        walkSpeed = giantWalkSpeed;
        runSpeed = giantRunSpeed;
        slideSpeed = giantSlideSpeed;
        crouchSpeed = giantCrouchSpeed;
        jumpSpeed = giantJumpSpeed;
    }

    public void SetTiny()
    {
        walkSpeed = tinyWalkSpeed;
        runSpeed = tinyRunSpeed;
        slideSpeed = tinySlideSpeed;
        crouchSpeed = tinyCrouchSpeed;
        jumpSpeed = tinyJumpSpeed;
    }
}
