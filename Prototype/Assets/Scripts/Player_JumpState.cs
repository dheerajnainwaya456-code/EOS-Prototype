using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.inAirCounter--;

        if (player.groundDetacted)
            player.setVelocity(rb.linearVelocity.x * player.facingDir, player.jumpForce);
        else
            player.setVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();
        if (inputVar.Player.Jump.WasPerformedThisFrame() && player.inAirCounter > 3)
        {
            stateMachine.ChangeState(player.jumpState);
        }

        if (rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
        }
    }
}
