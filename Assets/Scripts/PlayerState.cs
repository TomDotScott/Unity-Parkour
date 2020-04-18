namespace PlayerStates
{
    public enum MovementState
    {
        idle,
        moving,
        crouching,
        sliding,
        climbingLadder,
        wallRunning,
        grabbedLedge,
        climbingLedge,
        vaulting,
        hookShotThrowing,
        hookShotFlying
    }

    public enum GrowShrinkState
    {
        growingToStandard,
        growingToGiant,
        giant,
        shrinkingToStandard,
        shrinkingToTiny,
        tiny,
        standard
    }
}

