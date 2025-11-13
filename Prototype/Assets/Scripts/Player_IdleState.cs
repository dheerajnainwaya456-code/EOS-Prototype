using UnityEngine;

public class Player_IdleState : PlayerGroundedState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.setVelocity(0, rb.linearVelocity.y);
        player.inAirCounter = 5;
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.x == player.facingDir && player.wallDetacted)
        {
            return;
        }

        if (player.moveInput.x != 0)
            {
                stateMachine.ChangeState(player.moveState);
            }

    }
}