using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (inputVar.Player.Jump.WasPerformedThisFrame() && player.inAirCounter > 3)
        {
            stateMachine.ChangeState(player.jumpState);
        }
        
        if (player.groundDetacted)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (player.stairsDetacted)
        {
            stateMachine.ChangeState(player.climbingState);
        }

        if (player.wallDetacted)
        {
            stateMachine.ChangeState(player.wallSlideState);
        }
    }
}
