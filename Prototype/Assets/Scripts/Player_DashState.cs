using UnityEngine;

public class Player_DashState : EntityState
{
    private float originalGravityScale;
    private int dashDir;

    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dashDir = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDir;
        stateTimer = player.dashDuration;

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
    }

    public override void Update()
    {
        base.Update();
        CanceLDashIfNeeded();
        player.setVelocity(player.dashSpeed * dashDir, 0);

        if (stateTimer < 0)
        {
            if (player.groundDetacted)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.fallState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.setVelocity(0, 0);
        rb.gravityScale = originalGravityScale;
    }

    private void CanceLDashIfNeeded()
    {
        if (player.wallDetacted)
        {
            if (player.groundDetacted)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.wallSlideState);
            }
        }
    }
}
