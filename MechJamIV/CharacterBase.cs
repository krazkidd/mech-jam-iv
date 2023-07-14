using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV {
	public abstract partial class CharacterBase : CharacterBody2D
	{

        [Signal]
        public delegate void InjuredEventHandler(int damage);
        [Signal]
	    public delegate void KilledEventHandler();
        [Signal]
        public delegate void HealedEventHandler(int amount);

        [Export]
        public int Health { get; protected set; } = 100;

        [Export]
        public abstract Vector2 FaceDirection { get; set; }
		[Export]
		public virtual float MoveAcceleration { get; set; } = 50.0f;
		[Export]
		public virtual float MaxMoveSpeed { get; set; } = 300.0f;
        [Export]
        public float JumpVelocity { get; set; } = -400.0f;

	    protected virtual Vector2 Gravity { get; set; } = ProjectSettings.GetSetting("physics/2d/default_gravity_vector").AsVector2().Normalized() * ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		protected virtual Vector2 Drag { get; set; } = Vector2.Zero;

        #region Node references

        private CharacterAnimator characterAnimator;

        #endregion

		public override void _Ready()
		{
            if (MotionMode == MotionModeEnum.Grounded)
            {
                Drag = new Vector2(MoveAcceleration / MaxMoveSpeed, 0.0f);
            }
            else
            {
                Drag = new Vector2(MoveAcceleration / MaxMoveSpeed, MoveAcceleration / MaxMoveSpeed);
            }

            characterAnimator = GetNode<CharacterAnimator>("CharacterAnimator");
		}

		protected abstract Vector2 GetMovementDirection();

		protected abstract bool IsJumping();

        public override void _Process(double delta)
        {
            if (Health <= 0)
            {
                return;
            }

            AnimateMovement();

#if DEBUG
            QueueRedraw();
#endif
        }

#if DEBUG
        public override void _Draw()
        {
            DrawLine(Vector2.Zero, GetMovementDirection() * 25, Colors.Red);
        }
#endif

		public override void _PhysicsProcess(double delta)
		{
            if (Health <= 0)
            {
                return;
            }

            FaceDirection = GetMovementDirection();

            ProcessAttack(delta);

			Velocity += MoveAcceleration * FaceDirection - Drag * Velocity + (float)delta * Gravity;

			MoveAndSlide();

			if (IsJumping())
			{
				Velocity += new Vector2(0.0f, JumpVelocity);
			}
		}

        protected abstract void ProcessAttack(double delta);

        protected void AnimateMovement() => characterAnimator.AnimateMovement(FaceDirection);

        protected abstract void AnimateInjury(int damage, Vector2 position, Vector2 normal);

        protected void AnimateDeath() => characterAnimator.AnimateDeath();

        public virtual void Hurt(int damage, Vector2 position, Vector2 normal)
        {
            if (Health <= 0)
            {
                return;
            }

            Health = Math.Max(0, Health - damage);

            AnimateInjury(damage, position, normal);

            EmitSignal(SignalName.Injured, damage);

            if (Health <= 0)
            {
                AnimateDeath();

                EmitSignal(SignalName.Killed);

                //TODO the game is reacting poorly when we free the player
                //this.TimedFree(5.0f, processInPhysics:true);
            }
        }

		public virtual void Heal(int amount)
		{
            if (Health <= 0)
            {
                return;
            }

            //TODO we need to know initial/max health
            Health = Math.Min(100, Health + amount);

            EmitSignal(SignalName.Healed, amount);
		}

	}
}
