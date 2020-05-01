using PlayerStates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Layer Masks")]
    [SerializeField] private LayerMask vaultLayer;
    [SerializeField] private LayerMask ledgeLayer;
    [SerializeField] private LayerMask ladderLayer;
    [SerializeField] private LayerMask wallrunLayer;

    [Header("Enum States")]
    private MovementState state;
    private GrowShrinkState growShrinkState;

    [Header("Hook Shot Mechanics")]
    [SerializeField] private Camera gameCamera = null;
    [SerializeField] private Transform hookShotTransform = null;
    [SerializeField] private GameObject hookShotGun = null;

    [Header("Growing and Shrinking Attributes")]
    [SerializeField] private float growthRate = 0;
    [SerializeField] private float shrinkRate = 0;
    [SerializeField] private float standardSize = 0;
    [SerializeField] private float standardRadius = 0;
    [SerializeField] private float giantSize = 0;
    [SerializeField] private float giantRadius = 0;
    [SerializeField] private float tinySize = 0;
    [SerializeField] private float tinyRadius = 0;

    private GameObject vaultHelper;

    private Vector3 wallNormal = Vector3.zero;
    private Vector3 ladderNormal = Vector3.zero;
    private Vector3 pushFrom;
    private Vector3 slideDir;
    private Vector3 vaultOver;
    private Vector3 vaultDir;
    private Vector3 hookShotPos;

    private PlayerMovement movement;
    public PlayerInput playerInput;
    private AnimateLean animateLean;

    private bool canInteract;
    private bool canGrabLedge;

    private bool canHookshot = false;
    private bool canGrow = false;
    private bool canShrink = false;
    private bool isDead = false;

    private bool controlledSlide;

    private float rayDistance = 0;
    private float slideLimit = 0;
    private float slideTime = 0;
    private float radius = 0;
    private float height = 0;
    private float halfradius = 0;
    private float quarterHeight = 0;
    private float hookShotSize = 0;


    private int wallDir = 1;

    public MovementState State { get => state; set => state = value; }
    public GrowShrinkState GrowShrinkState { get => growShrinkState; set => growShrinkState = value; }

    public bool CanHookshot
    {
        get => canHookshot;
        set
        {
            hookShotGun.SetActive(true);
            canHookshot = value;
        }
    }

    public bool CanGrow { get => canGrow; set => canGrow = value; }
    public bool CanShrink { get => canShrink; set => canShrink = value; }
    public bool IsDead { get => isDead; set => isDead = value; }

    private void Start()
    {
        CreateVaultHelper();
        playerInput = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();

        hookShotGun.SetActive(false);
        hookShotTransform.gameObject.SetActive(false);

        if (GetComponentInChildren<AnimateLean>())
        {
            animateLean = GetComponentInChildren<AnimateLean>();
        }

        slideLimit = movement.characterController.slopeLimit - .1f;
        radius = movement.characterController.radius;
        height = movement.characterController.height;
        halfradius = radius / 2f;
        quarterHeight = height / 4f;
        rayDistance = quarterHeight + radius + .1f;
    }

    /******************************* UPDATE ******************************/
    void Update()
    {
        if (!GameManager.Instance.IsPaused)
        {
            //Updates
            UpdateInteraction();
            UpdateMovingStatus();


            //Check for movement updates
            if (GrowShrinkState == GrowShrinkState.standard)
            {
                CheckSliding();
                CheckCrouching();
                CheckForWallrun();
                CheckLadderClimbing();
                UpdateLedgeGrabbing();
                CheckForVault();
            }

            //Check for item updates
            if (CanHookshot)
            {
                CheckHookShot();
            }

            if (CanGrow || GrowShrinkState == GrowShrinkState.tiny)
            {
                CheckForGrowing();
            }

            if (CanShrink || GrowShrinkState == GrowShrinkState.giant)
            {
                CheckForShrinking();
            }


            //Misc
            UpdateLean();
        }
    }

    void UpdateInteraction()
    {
        if (!canInteract)
        {
            if (movement.grounded || movement.moveDirection.y < 0)
            {
                canInteract = true;
            }
        }
        else if ((int)State >= 6)
        {
            canInteract = false;
        }
    }

    void UpdateMovingStatus()
    {
        if ((int)State <= 1)
        {
            State = MovementState.idle;
            if (playerInput.GetInput().magnitude > 0.02f)
            {
                State = MovementState.moving;
            }
        }
    }

    void UpdateLean()
    {
        if (animateLean != null)
        {
            Vector2 lean = Vector2.zero;
            if (State == MovementState.wallRunning)
            {
                lean.x = wallDir;
            }

            if (State == MovementState.sliding && controlledSlide)
            {
                lean.y = -1;
            }
            else if (State == MovementState.grabbedLedge || State == MovementState.vaulting)
            {
                lean.y = 1;
            }
            animateLean.SetLean(lean);
        }
    }
    /*********************************************************************/


    /******************************** MOVE *******************************/
    void FixedUpdate()
    {
        switch (State)
        {
            case MovementState.sliding:
                SlideMovement();
                break;
            case MovementState.climbingLadder:
                LadderMovement();
                break;
            case MovementState.grabbedLedge:
                GrabbedLedgeMovement();
                break;
            case MovementState.climbingLedge:
                ClimbLedgeMovement();
                break;
            case MovementState.wallRunning:
                WallrunningMovement();
                break;
            case MovementState.vaulting:
                VaultMovement();
                break;
            case MovementState.hookShotThrowing:
                HookShotThrow();
                break;
            case MovementState.hookShotFlying:
                HookShotMovement();
                break;
            default:
                DefaultMovement();
                break;
        }

        if (GrowShrinkState != GrowShrinkState.standard && GrowShrinkState != GrowShrinkState.giant && GrowShrinkState != GrowShrinkState.tiny)
        {
            GrowShrinkAnimation();
        }
    }

    void DefaultMovement()
    {
        if (playerInput.GetRun() && State == MovementState.crouching)
        {
            Uncrouch();
        }
        movement.Move(playerInput.GetInput(), playerInput.GetRun(), (State == MovementState.crouching));
        if (movement.grounded && playerInput.Jump())
        {
            if (State == MovementState.crouching)
            {
                Uncrouch();
            }

            movement.Jump(Vector3.up, 1f);
            playerInput.ResetJump();
        }
    }
    /*********************************************************************/

    /****************************** SLIDING ******************************/
    void SlideMovement()
    {
        if (movement.grounded && playerInput.Jump())
        {
            if (controlledSlide)
            {
                slideDir = transform.forward;
            }

            movement.Jump(slideDir + Vector3.up, 1f);
            playerInput.ResetJump();
            slideTime = 0;
        }

        movement.Move(slideDir, movement.slideSpeed, 1f);
        if (slideTime <= 0)
        {
            if (playerInput.GetCrouching())
            {
                Crouch();
            }
            else
            {
                Uncrouch();
            }
        }
    }

    void CheckSliding()
    {
        //Check to slide when running
        if (playerInput.GetCrouch() && canSlide())
        {
            slideDir = transform.forward;
            movement.characterController.height = quarterHeight;
            controlledSlide = true;
            slideTime = 1f;
        }

        //Lower slidetime
        if (slideTime > 0)
        {
            State = MovementState.sliding;
            slideTime -= Time.deltaTime;
        }

        if (Physics.Raycast(transform.position, Vector3.up, out var hit, rayDistance))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle > slideLimit && movement.moveDirection.y < 0)
            {
                Vector3 hitNormal = hit.normal;
                slideDir = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref slideDir);
                controlledSlide = false;
                State = MovementState.sliding;
            }
        }
    }

    bool canSlide()
    {
        return !movement.grounded || playerInput.GetInput().magnitude <= 0.02f || !playerInput.GetRun()
            ? false
            : slideTime <= 0 && State != MovementState.sliding;
    }

    bool CanFit(RaycastHit hit)
    {
        //Check above the point to make sure the player can fit
        return !Physics.SphereCast(hit.point + (Vector3.up * radius), radius, Vector3.up, out _, height - radius);
    }

    /*********************************************************************/

    /***************************** CROUCHING *****************************/
    void CheckCrouching()
    {
        if (movement.grounded && (int)State <= 2)
        {
            if (playerInput.GetCrouch())
            {
                if (State != MovementState.crouching)
                {
                    Crouch();
                }
                else
                {
                    Uncrouch();
                }
            }
        }
    }

    void Crouch()
    {
        movement.characterController.height = quarterHeight;
        State = MovementState.crouching;
    }

    void Uncrouch()
    {
        movement.characterController.height = height;
        State = MovementState.moving;
    }
    /*********************************************************************/

    /************************** LADDER CLIMBING **************************/
    void LadderMovement()
    {
        Vector3 input = playerInput.GetInput();
        Vector3 move = Vector3.Cross(Vector3.up, ladderNormal).normalized;
        move *= input.x;
        move.y = input.y * movement.walkSpeed;

        bool goToGround = move.y < -0.02f && movement.grounded;

        if (playerInput.Jump())
        {
            movement.Jump((-ladderNormal + Vector3.up * 2f).normalized, 1f);
            playerInput.ResetJump();
            State = MovementState.moving;
        }

        if (!hasObjectInfront(0.05f, ladderLayer) || goToGround)
        {
            State = MovementState.moving;
            Vector3 pushUp = ladderNormal;
            pushUp.y = 0.25f;

            movement.ForceMove(pushUp, movement.walkSpeed, 0.25f, true);
        }
        else
            movement.Move(move, 1f, 0f);
    }

    void CheckLadderClimbing()
    {
        if (canInteract)
        {
            //Check for ladder all across player (so they cannot use the side)
            bool right = Physics.Raycast(transform.position + (transform.right * halfradius), transform.forward, radius + 0.125f, ladderLayer);
            bool left = Physics.Raycast(transform.position - (transform.right * halfradius), transform.forward, radius + 0.125f, ladderLayer);

            if (Physics.Raycast(transform.position, transform.forward, out var hit, radius + 0.125f, ladderLayer)
                && right
                && left)
            {
                if (hit.normal == hit.transform.forward)
                {
                    ladderNormal = -hit.normal;
                    if (hasObjectInfront(0.05f, ladderLayer) && playerInput.GetInput().y > 0.02f)
                    {
                        canInteract = false;
                        State = MovementState.climbingLadder;
                    }
                }
            }
        }
    }
    /*********************************************************************/

    /**************************** WALLRUNNING ****************************/
    void WallrunningMovement()
    {
        Vector3 input = playerInput.GetInput();
        float speed = (input.y > 0) ? input.y : 0;

        Vector3 move = wallNormal * speed;

        if (playerInput.Jump())
        {
            movement.Jump(((Vector3.up * (speed + 0.5f)) + (wallNormal * 2f * speed) + (transform.right * -wallDir * 1.25f)).normalized, speed + 0.5f);
            playerInput.ResetJump();
            State = MovementState.moving;
        }

        if (!HasWallToSide(wallDir) || movement.grounded)
        {
            State = MovementState.moving;
        }

        movement.Move(move, movement.runSpeed, (1f - speed) + (speed / 4f));
    }

    void CheckForWallrun()
    {
        if (canInteract && !movement.grounded && movement.moveDirection.y < 0)
        {
            int wall = 0;
            if (HasWallToSide(1))
            {
                wall = 1;
            }
            else if (HasWallToSide(-1))
            {
                wall = -1;
            }

            if (wall != 0)
            {
                if (Physics.Raycast(transform.position + (transform.right * wall * radius), transform.right * wall, out var hit, halfradius, wallrunLayer))
                {
                    wallDir = wall;
                    wallNormal = Vector3.Cross(hit.normal, Vector3.up) * -wallDir;
                    State = MovementState.wallRunning;
                }
            }
        }
    }

    bool HasWallToSide(int dir)
    {
        //Check for ladder in front of player
        Vector3 top = transform.position + (transform.right * 0.25f * dir);
        Vector3 bottom = top - (transform.up * radius);
        top += transform.up * radius;

        return (Physics.CapsuleCastAll(top, bottom, 0.25f, transform.right * dir, 0.05f, wallrunLayer).Length >= 1);
    }
    /*********************************************************************/

    /******************** LEDGE GRABBING AND CLIMBING ********************/
    void GrabbedLedgeMovement()
    {
        if (playerInput.Jump())
        {
            movement.Jump((Vector3.up - transform.forward).normalized, 1f);
            playerInput.ResetJump();
            State = MovementState.moving;
        }

        movement.Move(Vector3.zero, 0f, 0f); //Stay in place
    }

    void ClimbLedgeMovement()
    {
        Vector3 dir = pushFrom - transform.position;
        Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
        Vector3 move = Vector3.Cross(dir, right).normalized;

        movement.Move(move, movement.walkSpeed, 0f);
        if (new Vector2(dir.x, dir.z).magnitude < 0.125f)
        {
            State = MovementState.idle;
        }
    }

    void CheckLedgeGrab()
    {
        //Check for ledge to grab onto 
        Vector3 dir = transform.TransformDirection(new Vector3(0, -0.5f, 1).normalized);
        Vector3 pos = transform.position + (Vector3.up * height / 3f) + (transform.forward * radius / 2f);
        bool right = Physics.Raycast(pos + (transform.right * radius / 2f), dir, radius + 0.125f, ledgeLayer);
        bool left = Physics.Raycast(pos - (transform.right * radius / 2f), dir, radius + 0.125f, ledgeLayer);

        if (Physics.Raycast(pos, dir, out var hit, radius + 0.125f, ledgeLayer)
            && right
            && left)
        {
            Vector3 rotatePos = transform.InverseTransformPoint(hit.point);
            rotatePos.x = 0; rotatePos.z = 1;
            pushFrom = transform.position + transform.TransformDirection(rotatePos); //grab the position with local z = 1
            rotatePos.z = radius * 2f;

            Vector3 checkCollisions = transform.position + transform.TransformDirection(rotatePos); //grab it closer now

            //Check if you would be able to stand on the ledge
            if (!Physics.SphereCast(checkCollisions, radius, Vector3.up, out hit, height - radius))
            {
                canInteract = false;
                State = MovementState.grabbedLedge;
            }
        }
    }

    void UpdateLedgeGrabbing()
    {
        if (movement.grounded || movement.moveDirection.y > 0)
        {
            canGrabLedge = true;
        }

        if (State != MovementState.climbingLedge)
        {
            if (canGrabLedge
                && !movement.grounded
                && movement.moveDirection.y < 0)
            {
                CheckLedgeGrab();
            }

            if (State == MovementState.grabbedLedge)
            {
                canGrabLedge = false;
                Vector2 down = playerInput.GetDown();
                if (down.y == -1)
                    State = MovementState.moving;
                else if (down.y == 1)
                    State = MovementState.climbingLedge;
            }
        }
    }
    /*********************************************************************/

    /***************************** VAULTING ******************************/
    void VaultMovement()
    {
        Vector3 localPos = vaultHelper.transform.InverseTransformPoint(transform.position);
        Vector3 move = (vaultDir + (Vector3.up * -(localPos.z - radius) * height)).normalized;

        if (localPos.z > quarterHeight)
        {
            movement.characterController.height = height;
            State = MovementState.moving;
        }

        movement.Move(move, movement.runSpeed, 0f);
    }

    void CheckForVault()
    {
        if (State != MovementState.vaulting)
        {
            float checkDis = 0.05f;
            checkDis += (movement.characterController.velocity.magnitude / 16f); //Check farther if moving faster
            if (hasObjectInfront(checkDis, vaultLayer) && playerInput.Jump()
                && Physics.SphereCast(transform.position + (transform.forward * (radius - 0.25f)), 0.25f, transform.forward, out var sphereHit, checkDis, vaultLayer)
                && Physics.SphereCast(sphereHit.point + (Vector3.up * quarterHeight), radius, Vector3.down, out var hit, quarterHeight - radius, vaultLayer)
                && CanFit(hit))
            {

                vaultOver = hit.point;
                vaultDir = transform.forward;
                SetVaultHelper();

                canInteract = false;
                State = MovementState.vaulting;
                movement.characterController.height = radius;
            }
        }
    }

    void CreateVaultHelper()
    {
        vaultHelper = new GameObject();
        vaultHelper.transform.name = "Vault Helper";
    }

    void SetVaultHelper()
    {
        vaultHelper.transform.position = vaultOver;
        vaultHelper.transform.rotation = Quaternion.LookRotation(vaultDir);
    }
    /*********************************************************************/

    bool hasObjectInfront(float dis, LayerMask layer)
    {
        Vector3 top = transform.position + (transform.forward * 0.25f);
        Vector3 bottom = top - (transform.up * quarterHeight);

        return (Physics.CapsuleCastAll(top, bottom, 0.25f, transform.forward, dis, layer).Length >= 1);
    }

    /*********************************************************************/

    /***************************** HOOK SHOT *****************************/

    private void HookShotThrow()
    {
        hookShotTransform.gameObject.SetActive(true);

        //Look at the position and scale toward it
        hookShotTransform.LookAt(hookShotPos);

        float hookShotThrowSpeed = 60f;

        hookShotSize += hookShotThrowSpeed * Time.deltaTime;
        hookShotTransform.localScale = new Vector3(0.1f, 0.1f, hookShotSize);

        if (hookShotSize >= Vector3.Distance(gameCamera.transform.position, hookShotPos))
        {
            State = MovementState.hookShotFlying;
        }
    }

    private void CheckHookShot()
    {
        if (playerInput.GetLeftClick()
            && Physics.Raycast(gameCamera.transform.position, gameCamera.transform.forward, out RaycastHit raycastHit)
            && Vector3.Distance(gameCamera.transform.position, raycastHit.point) <= 35)
        {
            //If we hit something within range....
            hookShotPos = raycastHit.point;
            State = MovementState.hookShotThrowing;
            hookShotSize = 0;
        }
    }

    private void HookShotMovement()
    {
        hookShotTransform.LookAt(hookShotPos);

        Vector3 hookShotDir = (hookShotPos - transform.position).normalized;
        float distance = Vector3.Distance(gameCamera.transform.position, hookShotPos);
        float hookSpeed = distance * 10;
        movement.Move(hookShotDir, hookSpeed, 0);
        if (distance <= 1 || playerInput.GetLeftClick())
        {
            hookShotTransform.gameObject.SetActive(false);
            State = MovementState.moving;
        }
    }

    /*******************************************************************************/

    /***************************** GROWING / SHRINKING *****************************/

    private void CheckForGrowing()
    {
        if (playerInput.GetGrowButton())
        {
            if (GrowShrinkState == GrowShrinkState.tiny)
            {
                GrowShrinkState = GrowShrinkState.growingToStandard;
            }
            if (GrowShrinkState == GrowShrinkState.standard)
            {
                GrowShrinkState = GrowShrinkState.growingToGiant;
            }
        }
    }

    private void CheckForShrinking()
    {
        if (playerInput.GetShrinkButton())
        {
            if (GrowShrinkState == GrowShrinkState.giant)
            {
                GrowShrinkState = GrowShrinkState.shrinkingToStandard;
            }
            if (GrowShrinkState == GrowShrinkState.standard)
            {
                GrowShrinkState = GrowShrinkState.shrinkingToTiny;
            }
        }
    }

    private void GrowShrinkAnimation()
    {
        switch (GrowShrinkState)
        {
            case GrowShrinkState.growingToStandard:

                movement.characterController.height = Mathf.Clamp(movement.characterController.height + growthRate * Time.deltaTime, tinySize, standardSize);
                movement.characterController.radius = Mathf.Clamp(movement.characterController.radius + growthRate * Time.deltaTime, tinyRadius, standardRadius);

                if (movement.characterController.height == standardSize)
                {
                    GrowShrinkState = GrowShrinkState.standard;
                    movement.SetStandard();
                }
                break;
            case GrowShrinkState.growingToGiant:

                movement.characterController.height = Mathf.Clamp(movement.characterController.height + growthRate * Time.deltaTime, standardSize, giantSize);
                movement.characterController.radius = Mathf.Clamp(movement.characterController.radius + growthRate * Time.deltaTime, standardRadius, giantRadius);

                if (movement.characterController.height == giantSize)
                {
                    GrowShrinkState = GrowShrinkState.giant;
                    movement.SetGiant();
                }
                break;
            case GrowShrinkState.shrinkingToStandard:

                movement.characterController.height = Mathf.Clamp(movement.characterController.height - growthRate * Time.deltaTime, standardSize, giantSize);
                movement.characterController.radius = Mathf.Clamp(movement.characterController.radius - growthRate * Time.deltaTime, standardRadius, giantRadius);

                if (movement.characterController.height == standardSize)
                {
                    GrowShrinkState = GrowShrinkState.standard;
                    movement.SetStandard();
                }
                break;
            case GrowShrinkState.shrinkingToTiny:

                movement.characterController.height = Mathf.Clamp(movement.characterController.height - growthRate * Time.deltaTime, tinySize, standardSize);
                movement.characterController.radius = Mathf.Clamp(movement.characterController.radius - growthRate * Time.deltaTime, tinyRadius, standardRadius);

                if (movement.characterController.height == tinySize)
                {
                    GrowShrinkState = GrowShrinkState.tiny;
                    movement.SetTiny();
                }
                break;
        }
    }
}