using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Current Values")]
    public float walkSpeed = 0;
    public float runSpeed = 0;
    public float slideSpeed = 0;
    public float crouchSpeed = 0;
    [SerializeField] private float jumpSpeed = 0;
    
    [Header("Standard Values")]
    public float standardWalkSpeed = 0;
    public float standardRunSpeed = 0;
    public float standardSlideSpeed = 0;
    public float standardCrouchSpeed = 0;
    [SerializeField] private float standardJumpSpeed = 0;

    [Header("Giant Values")]
    public float giantWalkSpeed = 0;
    public float giantRunSpeed = 0;
    public float giantSlideSpeed = 0;
    public float giantCrouchSpeed = 0;
    [SerializeField] private float giantJumpSpeed = 0;

    [Header("Tiny Values")]
    public float tinyWalkSpeed = 0;
    public float tinyRunSpeed = 0;
    public float tinySlideSpeed = 0;
    public float tinyCrouchSpeed = 0;
    [SerializeField] private float tinyJumpSpeed = 0;

    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    [HideInInspector] public CharacterController characterController;

    [Header("Other Values")]
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float antiBumpFactor = .75f;
    public bool grounded;
    public Vector3 jump = Vector3.zero;

    private RaycastHit hit;
    private Vector3 force;
    private bool forceGravity;
    private float forceTime = 0;

    private void Awake()
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

    private void UpdateJump()
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
