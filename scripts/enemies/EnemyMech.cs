using Godot;
using System;
using MechJamIV;

public partial class EnemyMech : EnemyBase
{

    private DateTime lastTimePlayerSeen = DateTime.MinValue;

    #region Node references

    private WeaponManager weaponManager;

    #endregion

    public override void _Ready()
    {
        base._Ready();

        weaponManager = GetNode<WeaponManager>("WeaponManager");
        weaponManager.SetBodiesToExclude(this.Yield());
    }

    protected override Vector2 GetMovementDirectionForIdleState()
    {
        if (GD.Randf() < 0.01f)
        {
            return FaceDirection.IsEqualApprox(Vector2.Left) ? Vector2.Right : Vector2.Left;
        }

        return FaceDirection;
    }

    protected override Vector2 GetMovementDirectionForChaseState()
    {
        if (CharacterTracker.Target == null)
        {
            return Vector2.Zero;
        }

        return new Vector2(CharacterTracker.GetDirectionToTarget().X, 0.0f).Normalized();
    }

    protected override Vector2 GetMovementDirectionForAttackState()
    {
        return GetMovementDirectionForChaseState();
    }

    protected override bool IsJumping()
    {
        return false;
    }

    protected override void ProcessActionForIdleState()
    {
        if (CharacterTracker.Target == null)
        {
            return;
        }
        else if (CharacterTracker.IsTargetInFieldOfView(FaceDirection, FieldOfView) && CharacterTracker.IsTargetInLineOfSight())
        {
            State = EnemyState.Chase;

            lastTimePlayerSeen = DateTime.Now;
        }
    }

    protected override void ProcessActionForChaseState()
    {
        if (CharacterTracker.Target == null)
        {
            State = EnemyState.Idle;
        }
        else if (CharacterTracker.IsTargetInLineOfSight())
        {
            State = EnemyState.Attack;

            lastTimePlayerSeen = DateTime.Now;
        }
        else if ((DateTime.Now - lastTimePlayerSeen).Seconds >= ChaseDuration)
        {
            State = EnemyState.Idle;
        }
    }

    protected override void ProcessActionForAttackState()
    {
        if (CharacterTracker.Target == null)
        {
            State = EnemyState.Idle;
        }
        else if (!CharacterTracker.IsTargetInLineOfSight())
        {
            State = EnemyState.Chase;

            return;
        }

        //TODO we only want to fire machine gun if player is within attack range
        //weaponManager.Fire(FireMode.PrimarySustained, Target.GlobalPosition, CharacterTracker.Target);
        weaponManager.Fire(FireMode.Secondary, ToGlobal(Vector2.Up), CharacterTracker.Target);
    }

    protected override void AnimateInjury(int damage, Vector2 globalPos, Vector2 normal)
    {
        this.EmitParticlesOnce(PointDamageEffect.Instantiate<GpuParticles2D>(), globalPos);
    }

}
