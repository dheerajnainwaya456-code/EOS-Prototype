using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    private float attackVelocityTimer;
    private float lastTimeAttacked;

    private bool comboAttackQueued;
    private int attackDir;
    private int comboIndex = 1;
    private int comboLimit = 3;
    private const int FirstComboIndex = 1;

    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        // Adjusted combo limit to match attack velocity array.
        if (comboLimit != player.attackVelocity.Length)
        {
            comboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ComboIndexCounter();

        attackDir = player.moveInput.x != 0 ? ((int)player.moveInput.x) : player.facingDir;

        if (player.groundDetacted)
        {
            anim.SetInteger("basicAttackIndex", comboIndex);
        }
        else
        {
            anim.SetInteger("basicAttackIndex", 1);
        }
        
        ApplyAttackVelocity();
    }

    public override void Update()
    {
        base.Update();
        HandleVelocity();

        if (inputVar.Player.Attack.WasPressedThisFrame() && player.groundDetacted)
        {
            QueueNextAttack();
        }

        if (triggerCalled)
        {
            HandleStateExit();
        }
    }

    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackState();
        }
        else
        {
            if (player.groundDetacted)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (rb.linearVelocityY  == 0)
        {
            comboIndex++;   
        }
        lastTimeAttacked = Time.time;

    }

    private void QueueNextAttack()
    {
        if (comboIndex < comboLimit)
        {
            comboAttackQueued = true;
        }
    }

    private void ComboIndexCounter()
    {
        if (Time.time > lastTimeAttacked + player.comboResetTime)
        {
            comboIndex = FirstComboIndex;
        }
        if (comboIndex == comboLimit + 1)
        {
            comboIndex = FirstComboIndex;
        }
    }

    private void HandleVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if (attackVelocityTimer < 0)
        {
            player.setVelocity(0, rb.linearVelocity.y);
        }

    }

    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex-1];

        attackVelocityTimer = player.attackVelocityDuration;
        player.setVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }
}
