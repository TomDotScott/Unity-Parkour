namespace PlayerStates
{
    public enum MovementState
    {
        Idle,
        Moving,
        Crouching,
        Sliding,
        ClimbingLadder,
        WallRunning,
        GrabbedLedge,
        ClimbingLedge,
        Vaulting,
        HookShotThrowing,
        HookShotFlying
    }

    public enum GrowShrinkState
    {
        GrowingToStandard,
        GrowingToGiant,
        Giant,
        ShrinkingToStandard,
        ShrinkingToTiny,
        Tiny,
        Standard
    }
}

