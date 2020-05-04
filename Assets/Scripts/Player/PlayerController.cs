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

    [Header("Hook Shot Mechanics")]
    [SerializeField] private Camera gameCamera = null;
    [SerializeField] private Transform hookShotTransform = null;
    [SerializeField] private GameObject hookShotGun = null;

    [Header("Growing and Shrinking Attributes")]
    [SerializeField] private float growthRate = 0;
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

    [field: Header("PlayerStates")] 
    public MovementState State { get; private set; }

    public GrowShrinkState GrowShrinkState { get; private set; }

    public bool CanHookshot
    {
        get => canHookshot;
        set
        {
            hookShotGun.SetActive(true);
            canHookshot = value;
        }
    }

    public bool CanGrow { get; set; } = false;
    public bool CanShrink { get; set; } = false;
    public bool IsDead { get; set; } = false;

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
            if (GrowShrinkState == GrowShrinkState.Standard)
            {
                CheckSliding();
                CheckCrouching();
                CheckForWallRun();
                CheckLadderClimbing();
                UpdateLedgeGrabbing();
                CheckForVault();
            }

            //Check for item updates
            if (CanHookshot)
            {
                CheckHookShot();
            }

            if (CanGrow || GrowShrinkState == GrowShrinkState.Tiny)
            {
                CheckForGrowing();
            }

            if (CanShrink || GrowShrinkState == GrowShrinkState.Giant)
            {
                CheckForShrinking();
            }


            //Misc
            UpdateLean();
        }
    }

    private void UpdateInteraction()
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
        if ((int) State > 1) return;
        State = MovementState.Idle;
        if (PlayerInput.GetInput().magnitude > 0.02f)
        {
            State = MovementState.Moving;
        }
    }

    void UpdateLean()
    {
        if (!animateLean) return;
        Vector2 lean = Vector2.zero;
        switch (State)
        {
            case MovementState.WallRunning:
                lean.x = wallDir;
                break;
            case MovementState.Sliding when controlledSlide:
                lean.y = -1;
                break;
            case MovementState.Vaulting:
                lean.y = 1;
                break;
        }
        animateLean.SetLean(lean);
    }
    /*********************************************************************/


    /******************************** MOVE *******************************/
    void FixedUpdate()
    {
        switch (State)
        {
            case MovementState.Sliding:
                SlideMovement();
                break;
            case MovementState.ClimbingLadder:
                LadderMovement();
                break;
            case MovementState.GrabbedLedge:
                GrabbedLedgeMovement();
                break;
            case MovementState.ClimbingLedge:
                ClimbLedgeMovement();
                break;
            case MovementState.WallRunning:
                WallRunningMovement();
                break;
            case MovementState.Vaulting:
                VaultMovement();
                break;
            case MovementState.HookShotThrowing:
                HookShotThrow();
                break;
            case MovementState.HookShotFlying:
                HookShotMovement();
                break;
            default:
                DefaultMovement();
                break;
        }

        if (GrowShrinkState != GrowShrinkState.Standard && GrowShrinkState != GrowShrinkState.Giant && GrowShrinkState != GrowShrinkState.Tiny)
        {
            GrowShrinkAnimation();
        }
    }

    private void DefaultMovement()
    {
        if (PlayerInput.GetRun() && State == MovementState.Crouching)
        {
            Uncrouch();
        }
        movement.Move(PlayerInput.GetInput(), PlayerInput.GetRun(), (State == MovementState.Crouching));
        if (movement.grounded && playerInput.Jump())
        {
            if (State == MovementState.Crouching)
            {
                Uncrouch();
            }

            movement.Jump(Vector3.up, 1f);
            playerInput.ResetJump();
        }
    }
    /*********************************************************************/

    /****************************** SLIDING ******************************/
    private void SlideMovement()
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
        if (slideTime > 0) return;
        if (PlayerInput.GetCrouching())
        {
            Crouch();
        }
        else
        {
            Uncrouch();
        }
    }

    private void CheckSliding()
    {
        //Check to slide when running
        if (PlayerInput.GetCrouch() && CanSlide())
        {
            slideDir = transform.forward;
            movement.characterController.height = quarterHeight;
            controlledSlide = true;
            slideTime = 1f;
        }

        //Lower slidetime
        if (slideTime > 0)
        {
            State = MovementState.Sliding;
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
                State = MovementState.Sliding;
            }
        }
    }

    private bool CanSlide()
    {
        return movement.grounded && !(PlayerInput.GetInput().magnitude <= 0.02f) && PlayerInput.GetRun() && (slideTime <= 0 && State != MovementState.Sliding);
    }

    private bool CanFit(RaycastHit hit)
    {
        //Check above the point to make sure the player can fit
        return !Physics.SphereCast(hit.point + (Vector3.up * radius), radius, Vector3.up, out _, height - radius);
    }

    /*********************************************************************/

    /***************************** CROUCHING *****************************/
    private void CheckCrouching()
    {
        if (movement.grounded && (int)State <= 2)
        {
            if (PlayerInput.GetCrouch())
            {
                if (State != MovementState.Crouching)
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

    private void Crouch()
    {
        movement.characterController.height = quarterHeight;
        State = MovementState.Crouching;
    }

    private void Uncrouch()
    {
        movement.characterController.height = height;
        State = MovementState.Moving;
    }
    /*********************************************************************/

    /************************** LADDER CLIMBING **************************/
    private void LadderMovement()
    {
        Vector3 input = PlayerInput.GetInput();
        Vector3 move = Vector3.Cross(Vector3.up, ladderNormal).normalized;
        move *= input.x;
        move.y = input.y * movement.walkSpeed;

        bool goToGround = move.y < -0.02f && movement.grounded;

        if (playerInput.Jump())
        {
            movement.Jump((-ladderNormal + Vector3.up * 2f).normalized, 1f);
            playerInput.ResetJump();
            State = MovementState.Moving;
        }

        if (!HasObjectInFront(0.05f, ladderLayer) || goToGround)
        {
            State = MovementState.Moving;
            Vector3 pushUp = ladderNormal;
            pushUp.y = 0.25f;

            movement.ForceMove(pushUp, movement.walkSpeed, 0.25f, true);
        }
        else
            movement.Move(move, 1f, 0f);
    }

    private void CheckLadderClimbing()
    {
        if (!canInteract) return;
        
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
                if (HasObjectInFront(0.05f, ladderLayer) && PlayerInput.GetInput().y > 0.02f)
                {
                    canInteract = false;
                    State = MovementState.ClimbingLadder;
                }
            }
        }
    }
    /*********************************************************************/

    /**************************** WALLRUNNING ****************************/
    private void WallRunningMovement()
    {
        Vector3 input = PlayerInput.GetInput();
        float speed = (input.y > 0) ? input.y : 0;

        Vector3 move = wallNormal * speed;

        if (playerInput.Jump())
        {
            movement.Jump(((Vector3.up * (speed + 0.5f)) + (wallNormal * 2f * speed) + (transform.right * -wallDir * 1.25f)).normalized, speed + 0.5f);
            playerInput.ResetJump();
            State = MovementState.Moving;
        }

        if (!HasWallToSide(wallDir) || movement.grounded)
        {
            State = MovementState.Moving;
        }

        movement.Move(move, movement.runSpeed, (1f - speed) + (speed / 4f));
    }

    private void CheckForWallRun()
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
                if (Physics.Raycast(transform.position + (transform.right * (wall * radius)), transform.right * wall, out var hit, halfradius, wallrunLayer))
                {
                    wallDir = wall;
                    wallNormal = Vector3.Cross(hit.normal, Vector3.up) * -wallDir;
                    State = MovementState.WallRunning;
                }
            }
        }
    }

    bool HasWallToSide(int dir)
    {
        //Check for ladder in front of player
        Vector3 top = transform.position + (transform.right * (0.25f * dir));
        Vector3 bottom = top - (transform.up * radius);
        top += transform.up * radius;

        return (Physics.CapsuleCastAll(top, bottom, 0.25f, transform.right * dir, 0.05f, wallrunLayer).Length >= 1);
    }
    /*********************************************************************/

    /******************** LEDGE GRABBING AND CLIMBING ********************/
    private void GrabbedLedgeMovement()
    {
        if (playerInput.Jump())
        {
            movement.Jump((Vector3.up - transform.forward).normalized, 1f);
            playerInput.ResetJump();
            State = MovementState.Moving;
        }

        movement.Move(Vector3.zero, 0f, 0f); //Stay in place
    }

    private void ClimbLedgeMovement()
    {
        Vector3 dir = pushFrom - transform.position;
        Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
        Vector3 move = Vector3.Cross(dir, right).normalized;

        movement.Move(move, movement.walkSpeed, 0f);
        if (new Vector2(dir.x, dir.z).magnitude < 0.125f)
        {
            State = MovementState.Idle;
        }
    }

    private void CheckLedgeGrab()
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
                State = MovementState.GrabbedLedge;
            }
        }
    }

    private void UpdateLedgeGrabbing()
    {
        if (movement.grounded || movement.moveDirection.y > 0)
        {
            canGrabLedge = true;
        }

        if (State != MovementState.ClimbingLedge)
        {
            if (canGrabLedge
                && !movement.grounded
                && movement.moveDirection.y < 0)
            {
                CheckLedgeGrab();
            }

            if (State == MovementState.GrabbedLedge)
            {
                canGrabLedge = false;
                Vector2 down = playerInput.GetDown();
                if (down.y == -1)
                    State = MovementState.Moving;
                else if (down.y == 1)
                    State = MovementState.ClimbingLedge;
            }
        }
    }
    /*********************************************************************/

    /***************************** VAULTING ******************************/
    private void VaultMovement()
    {
        Vector3 localPos = vaultHelper.transform.InverseTransformPoint(transform.position);
        Vector3 move = (vaultDir + (Vector3.up * (-(localPos.z - radius) * height))).normalized;

        if (localPos.z > quarterHeight)
        {
            movement.characterController.height = height;
            State = MovementState.Moving;
        }

        movement.Move(move, movement.runSpeed, 0f);
    }

    private void CheckForVault()
    {
        if (State == MovementState.Vaulting) return;
        float checkDis = 0.05f;
        checkDis += (movement.characterController.velocity.magnitude / 16f); //Check farther if moving faster
        if (HasObjectInFront(checkDis, vaultLayer) && playerInput.Jump()
                                                   && Physics.SphereCast(transform.position + (transform.forward * (radius - 0.25f)), 0.25f, transform.forward, out var sphereHit, checkDis, vaultLayer)
                                                   && Physics.SphereCast(sphereHit.point + (Vector3.up * quarterHeight), radius, Vector3.down, out var hit, quarterHeight - radius, vaultLayer)
                                                   && CanFit(hit))
        {

            vaultOver = hit.point;
            vaultDir = transform.forward;
            SetVaultHelper();

            canInteract = false;
            State = MovementState.Vaulting;
            movement.characterController.height = radius;
        }
    }

    private void CreateVaultHelper()
    {
        vaultHelper = new GameObject();
        vaultHelper.transform.name = "Vault Helper";
    }

    private void SetVaultHelper()
    {
        vaultHelper.transform.position = vaultOver;
        vaultHelper.transform.rotation = Quaternion.LookRotation(vaultDir);
    }
    /*********************************************************************/

    private bool HasObjectInFront(float dis, LayerMask layer)
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
            State = MovementState.HookShotFlying;
        }
    }

    private void CheckHookShot()
    {
        if (PlayerInput.GetLeftClick()
            && Physics.Raycast(gameCamera.transform.position, gameCamera.transform.forward, out RaycastHit raycastHit)
            && Vector3.Distance(gameCamera.transform.position, raycastHit.point) <= 35)
        {
            //If we hit something within range....
            hookShotPos = raycastHit.point;
            State = MovementState.HookShotThrowing;
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
        if (distance <= 1 || PlayerInput.GetLeftClick())
        {
            hookShotTransform.gameObject.SetActive(false);
            State = MovementState.Moving;
        }
    }

    /*******************************************************************************/

    /***************************** GROWING / SHRINKING *****************************/

    private void CheckForGrowing()
    {
        if (PlayerInput.GetGrowButton())
        {
            if (GrowShrinkState == GrowShrinkState.Tiny)
            {
                GrowShrinkState = GrowShrinkState.GrowingToStandard;
            }
            if (GrowShrinkState == GrowShrinkState.Standard)
            {
                GrowShrinkState = GrowShrinkState.GrowingToGiant;
            }
        }
    }

    private void CheckForShrinking()
    {
        if (PlayerInput.GetShrinkButton())
        {
            if (GrowShrinkState == GrowShrinkState.Giant)
            {
                GrowShrinkState = GrowShrinkState.ShrinkingToStandard;
            }
            if (GrowShrinkState == GrowShrinkState.Standard)
            {
                GrowShrinkState = GrowShrinkState.ShrinkingToTiny;
            }
        }
    }

    private void GrowShrinkAnimation()
    {
        switch (GrowShrinkState)
        {
            case GrowShrinkState.GrowingToStandard:

                movement.characterController.height = Mathf.Clamp(movement.characterController.height + growthRate * Time.deltaTime, tinySize, standardSize);
                movement.characterController.radius = Mathf.Clamp(movement.characterController.radius + growthRate * Time.deltaTime, tinyRadius, standardRadius);

                if (movement.characterController.height == standardSize)
                {
                    GrowShrinkState = GrowShrinkState.Standard;
                    movement.SetStandard();
                }
                break;
            case GrowShrinkState.GrowingToGiant:

                movement.characterController.height = Mathf.Clamp(movement.characterController.height + growthRate * Time.deltaTime, standardSize, giantSize);
                movement.characterController.radius = Mathf.Clamp(movement.characterController.radius + growthRate * Time.deltaTime, standardRadius, giantRadius);

                if (movement.characterController.height == giantSize)
                {
                    GrowShrinkState = GrowShrinkState.Giant;
                    movement.SetGiant();
                }
                break;
            case GrowShrinkState.ShrinkingToStandard:

                movement.characterController.height = Mathf.Clamp(movement.characterController.height - growthRate * Time.deltaTime, standardSize, giantSize);
                movement.characterController.radius = Mathf.Clamp(movement.characterController.radius - growthRate * Time.deltaTime, standardRadius, giantRadius);

                if (movement.characterController.height == standardSize)
                {
                    GrowShrinkState = GrowShrinkState.Standard;
                    movement.SetStandard();
                }
                break;
            case GrowShrinkState.ShrinkingToTiny:

                movement.characterController.height = Mathf.Clamp(movement.characterController.height - growthRate * Time.deltaTime, tinySize, standardSize);
                movement.characterController.radius = Mathf.Clamp(movement.characterController.radius - growthRate * Time.deltaTime, tinyRadius, standardRadius);

                if (movement.characterController.height == tinySize)
                {
                    GrowShrinkState = GrowShrinkState.Tiny;
                    movement.SetTiny();
                }
                break;
        }
    }
}