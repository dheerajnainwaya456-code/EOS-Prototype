using UnityEngine;

public class Player_WallSlideState : EntityState
{
    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        HandleWallSlide();

        if (inputVar.Player.Jump.WasPressedThisFrame() && player.inAirCounter != 0)
        {
            stateMachine.ChangeState(player.wallJumpState);
        }

        if (player.groundDetacted)
        {
            stateMachine.ChangeState(player.idleState);
            player.inAirCounter = 5;
            player.Flip();
        }

        if (!player.groundDetacted && !player.wallDetacted)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }

    private void HandleWallSlide()
    {
        if (player.moveInput.y < 0)
        {
            player.setVelocity(player.moveInput.x, rb.linearVelocity.y);
        }
        else
        {
            player.setVelocity(player.moveInput.x, rb.linearVelocity.y * player.wallSlideMultiplier);
        }
    }
}
