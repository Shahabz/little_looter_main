/*
 * Date: September 16th, 2023
 * Author: Peche
 */

using UnityEngine;

namespace LittleLooters.Gameplay
{
	public class VisualEnemyController : MonoBehaviour
	{
		#region Inspector

		[SerializeField] private AudioClip[] _footstepAudioClips = default;
		[Range(0, 1)] 
		[SerializeField] private float _footstepAudioVolume = 0.5f;
		[SerializeField] private Animator _animator = default;

		#endregion

		#region Private properties

		private EnemyBehaviorData _data = default;
		private int _animSpeedId = default;
		private int _animMotionSpeedId = default;
		private int _animAttackId = default;
		private int _animIsAttackingId = default;
		private int _animTakeDamageId = default;
		private int _animDeathId = default;
		private int _animIsAimingId = default;
		private int _animIsReloadingId = default;
		private int _animReloadId = default;
		private bool _isEnabled = false;
		private bool _isReloading = false;
		private bool _isAttacking = false;

		#endregion

		#region Public methods

		public void Init(EnemyState state, EnemyBehaviorData data)
		{
			_data = data;

			// Locomotion ids
			_animSpeedId = Animator.StringToHash("Speed");
			_animMotionSpeedId = Animator.StringToHash("MotionSpeed");

			// Attack ids
			_animAttackId = Animator.StringToHash("attack");
			_animIsAttackingId = Animator.StringToHash("isAttacking");
			_animIsAimingId = Animator.StringToHash("isAiming");

			// Take damage id
			_animTakeDamageId = Animator.StringToHash("takeDamage");

			// Death id
			_animDeathId = Animator.StringToHash("dead");

			// Reload ids
			_animIsReloadingId = Animator.StringToHash("isReloading");
			_animReloadId = Animator.StringToHash("reload");

			_isEnabled = true;

			Refresh(state, false);
		}

		public void Refresh(EnemyState state, bool isPerformingAttack)
		{
			if (!_isEnabled) return;

			if (state == EnemyState.IDLE)
			{
				SetIdle();
				return;
			}

			if (state == EnemyState.CHASE)
			{
				SetChase();
				return;
			}

			if (state == EnemyState.RELOAD)
			{
				SetReload();
				return;
			}

			if (state == EnemyState.AIM)
			{
				SetAiming();
				return;
			}

			if (state == EnemyState.ATTACK)
			{
				SetAttack(isPerformingAttack);
				return;
			}

			if (state == EnemyState.DIE)
			{
				SetDeath();
				return;
			}
		}

		/// <summary>
		/// Invoked from animation event
		/// </summary>
		/// <param name="animationEvent"></param>
		public void OnFootstep(AnimationEvent animationEvent)
		{
			// Commented for performance
			/*if (animationEvent.animatorClipInfo.weight <= 0.5f) return;

			if (_footstepAudioClips.Length == 0) return;

			var index = Random.Range(0, _footstepAudioClips.Length);

			AudioSource.PlayClipAtPoint(_footstepAudioClips[index], transform.TransformPoint(transform.position), _footstepAudioVolume);*/
		}

		public void TakeDamage()
		{
			if (!_isEnabled) return;

			SetTakeDamage();
		}

		public void Attack()
		{
			_animator.SetTrigger(_animAttackId);
		}

		#endregion

		#region Private methods

		private void SetIdle()
		{
			_animator.SetFloat(_animSpeedId, 0);
			_animator.SetFloat(_animMotionSpeedId, 0);

			CancelAttacking();

			// Cancel aiming
			_animator.SetBool(_animIsAimingId, false);

			CancelReloading();
		}

		private void SetChase()
		{
			_animator.SetFloat(_animSpeedId, _data.WalkingSpeed);
			_animator.SetFloat(_animMotionSpeedId, 1);

			CancelAttacking();
		}

		private void SetAiming()
		{
			_animator.SetBool(_animIsAimingId, true);
		}

		private void SetReload()
		{
			if (!_isReloading)
			{
				_animator.SetTrigger(_animReloadId);
			}

			_isReloading = true;

			_animator.SetBool(_animIsReloadingId, true);

			_animator.SetBool(_animIsAimingId, false);

			CancelAttacking();
		}

		private void SetAttack(bool isPerformingAttack)
		{
			// Stop movement animation
			_animator.SetFloat(_animSpeedId, 0);
			_animator.SetFloat(_animMotionSpeedId, 0);

			// Cancel aiming
			_animator.SetBool(_animIsAimingId, false);

			CancelReloading();

			// Start Attacking
			_animator.SetBool(_animIsAttackingId, true);

			_isAttacking = true;
		}

		private void SetDeath()
		{
			// Stop movement animation
			_animator.SetFloat(_animSpeedId, 0);
			_animator.SetFloat(_animMotionSpeedId, 0);

			CancelAttacking();

			CancelReloading();

			// Set dead trigger
			_animator.SetTrigger(_animDeathId);
		}

		private void SetTakeDamage()
		{
			if (_isReloading) return;

			if (_isAttacking) return;

			// Set take damage trigger
			_animator.SetTrigger(_animTakeDamageId);
		}

		private void CancelReloading()
		{
			if (!_isReloading) return;

			_animator.SetBool(_animIsReloadingId, false);

			_isReloading = false;
		}

		private void CancelAttacking()
		{
			_isAttacking = false;

			_animator.SetBool(_animIsAttackingId, false);
		}

		#endregion
	}
}