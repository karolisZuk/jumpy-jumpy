public class PlayerStateFactory
{
    PlayerStateMachine context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        context = currentContext;
    }

    public PlayerBaseState Idle(){
        return new PlayerIdleState();
    }

    public PlayerBaseState Walk() {
        return new PlayerWalkState();
    }

    public PlayerBaseState Run() {
        return new PlayerRunState();
    }

    public PlayerBaseState Jump() {
        return new PlayerJumpState();
    }

    public PlayerBaseState Grounded() {
        return new PlayerGroundedState();
    }
}
