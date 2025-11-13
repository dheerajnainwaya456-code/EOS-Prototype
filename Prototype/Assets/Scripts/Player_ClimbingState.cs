using UnityEngine;

public class Player_ClimbingState : EntityState
{

    float baseAnimspeed;

    private float originalGravityScale;
    public Player_ClimbingState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        baseAnimspeed = player.anim.speed;
    }

    public override void Enter()
    {
        base.Enter();

        player.setVelocity(0, 0);
        player.inAirCounter = 5;

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.y == 0)
        {
            stopAnimation();
        }
        else
        {
            playAnimation();
        }

        if (player.jumpBuffered)
        {
            stateMachine.ChangeState(player.jumpState);
            player.ConsumeJumpBuffer();
            return;
        }

        if (!player.stairsDetacted && !player.groundDetacted)
        {
            stateMachine.ChangeState(player.fallState);
        }
        
        player.setVelocity(player.moveInput.x * 5, player.moveInput.y * player.climbingSpeed);

        if (player.groundDetacted && rb.linearVelocity.y == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (player.anim.speed == 0)
        {
            playAnimation();
        }

        rb.gravityScale = originalGravityScale;
    }

    private void stopAnimation()
    {
        player.anim.speed = 0;
    }

    private void playAnimation()
    {
        player.anim.speed = baseAnimspeed;
    }
}