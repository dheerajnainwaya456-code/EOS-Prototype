using UnityEngine;

public class PlayerGroundedState : EntityState
{
    public PlayerGroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (!player.groundDetacted)
        {
            stateMachine.ChangeState(player.fallState);
        }

        if (player.moveInput.y > 0 && player.stairsDetacted)
        {
            stateMachine.ChangeState(player.climbingState);
        }

        if (!player.groundDetacted && player.stairsDetacted)
        {
            stateMachine.ChangeState(player.climbingState);
        }

        if (player.jumpBuffered)
        {
            stateMachine.ChangeState(player.jumpState);
            player.ConsumeJumpBuffer();
            return;
        }

        if (inputVar.Player.Attack.WasPerformedThisFrame())
        {
            stateMachine.ChangeState(player.basicAttackState);
        }
    }
}
