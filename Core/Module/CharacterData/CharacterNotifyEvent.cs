﻿using System;
using System.Threading.Tasks;
using L2Logger;

namespace Core.Module.CharacterData;

public class CharacterNotifyEvent : CharacterNotifyEventAbstract
{
    /** The flag used to indicate that a thinking action is in progress */
    private bool _thinking; // to prevent recursive thinking

    public CharacterNotifyEvent(Character character) : base(character)
    {
            
    }

    public override async Task OnEvtThinkAsync()
    {
        // Check if the actor can't use skills and if a thinking action isn't already in progress
        if (_thinking)
        {
            await Task.CompletedTask;
            return;
        }
		
        // Start thinking action
        _thinking = true;
		
        try
        {
            // Manage AI thinks of a Attackable
            if (_character.CharacterDesire().GetDesire() == Desire.ActiveDesire)
            {
                //await ThinkActive();
            }
            else if (_character.CharacterDesire().GetDesire() == Desire.AttackDesire)
            {
                await ThinkAttackAsync();
            }
        }
        finally
        {
            // Stop thinking action
            _thinking = false;
        }
    }
    private Character GetAttackTarget() => _character.CharacterDesire().AttackTarget;
    public virtual async Task ThinkAttackAsync()
    {
        var target = GetAttackTarget();
        if (target == null)
        {
            return;
        }

        if (CheckTargetLostOrDead(target))
        {
            // Notify the target
            _character.CharacterDesire().AttackTarget = null;
            return;
        }

        if (await _character.CharacterDesire().MaybeMoveToPawnAsync(target, _character.CharacterCombat().GetPhysicalAttackRange()))
        {
            return;
        }
        await _character.CharacterDesire().ClientStopMovingAsync(null);
        //LoggerManager.Info($"Start Attack Target: {target.ObjectId}");
        await _character.PhysicalAttack().DoAttackAsync(target).ContinueWith(_character.CharacterDesire().HandleException);
    }

    public override async Task OnEvtAttackedAsync(Character arg0)
    {
        await ClientStartAutoAttackAsync();
    }

    public override async Task OnEvtArrivedRevalidate()
    {
        if (_character.CharacterMovement().IsMoving)
        {
            await OnEvtThinkAsync();
        }
    }

    public override async Task OnEvtReadyToAct()
    {
        await OnEvtThinkAsync();
    }

    public override async Task OnEvtArrivedAsync()
    {
        try
        {
            _character.CharacterZone().RevalidateZone();
            // If the Intention was AI_INTENTION_MOVE_TO, set the Intention to AI_INTENTION_ACTIVE
            if (await _character.CharacterMovement().MoveToNextRoutePoint())
            {
                return;
            }
            await _character.CharacterDesire().ClientStoppedMovingAsync();
            if (_character.CharacterDesire().GetDesire() == Desire.MoveToDesire)
            {
                _character.CharacterDesire().AddDesire(Desire.ActiveDesire, null);
            }
		
            // Launch actions corresponding to the Event Think
            await OnEvtThinkAsync();
        }
        catch (Exception ex)
        {
            LoggerManager.Error(GetType().Name + ": OnEvtArrivedAsync:  " + ex.Message);
        }
    }

    public override async Task OnEvtArrivedBlockedAsync(Location location)
    {
        // If the Intention was AI_INTENTION_MOVE_TO, set the Intention to AI_INTENTION_ACTIVE
        if ((_character.CharacterDesire().GetDesire() == Desire.MoveToDesire) || (_character.CharacterDesire().GetDesire() == Desire.CastDesire))
        {
            _character.CharacterDesire().AddDesire(Desire.ActiveDesire, location);
        }
		
        // Stop the actor movement server side AND client side by sending Server->Client packet StopMove/StopRotation (broadcast)
        await _character.CharacterDesire().ClientStopMovingAsync(location);
		
        // Launch actions corresponding to the Event Think
        await OnEvtThinkAsync();
    }

    public override Task OnEvtDeadAsync()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected bool CheckTargetLostOrDead(Character target)
    {
        if ((target == null) || target.CharacterStatus().IsDead)
        {
            _character.CharacterDesire().AddDesire(Desire.ActiveDesire, null);
            return true;
        }
        return false;
    }
}