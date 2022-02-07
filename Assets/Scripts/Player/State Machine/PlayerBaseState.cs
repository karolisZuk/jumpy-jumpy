public abstract class PlayerBaseState
{
    protected bool isRootState = false;

    protected PlayerStateMachine ctx;
    protected PlayerStateFactory factory;
    protected PlayerBaseState currentSubState;
    protected PlayerBaseState currentSuperState;

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) {
        ctx = currentContext;
        factory = playerStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    public void UpdateStates(){
        UpdateState();
        if(currentSubState != null)
        {
            currentSubState.UpdateStates();
        }
    }

    public void ExitStates()
    {
        ExitState();
        if(currentSubState != null)
        {
            currentSubState.ExitStates();
        }
    }

    protected void SwitchState(PlayerBaseState newState) {
        ExitState();
        newState.EnterState();

        if(isRootState)
        {
            ctx.currentState = newState;
        } else if (currentSuperState != null)
        {
            currentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(PlayerBaseState newSuperState) {
        currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseState newSubState) {
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
